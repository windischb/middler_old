using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Text.RegularExpressions;
using Lexical.FileSystem;
using Lexical.FileSystem.Utility;
using LibGit2Sharp;
using Reflectensions.ExtensionMethods;

namespace middler.DataStore
{
    public class FolderObserver : IObserver<IEvent>
    {
        public void OnCompleted() => Console.WriteLine("OnCompleted");
        public void OnError(Exception error) => Console.WriteLine(error);

        private Subject<string> ChangedSubject { get; } = new Subject<string>();

        public IObservable<string> Changed => ChangedSubject.Throttle(TimeSpan.FromSeconds(1)).AsObservable();

        public void OnNext(IEvent @event)
        {
            var isDirectory = String.IsNullOrEmpty(@event.Path) || @event.Path.EndsWith("/");
            if (!isDirectory)
                return;

            if (@event.Path?.Count(c => c == '/') == 1)
            {
                this.ChangedSubject.OnNext(@event.Path);
            }

            if (@event is RenameEvent)
            {
                this.ChangedSubject.OnNext(@event.Path);
            }

            if (@event is CreateEvent)
            {
                this.ChangedSubject.OnNext(@event.Path);
            }

            if (@event is DeleteEvent)
            {
                this.ChangedSubject.OnNext(@event.Path);
            }
            
            
        }
    }

    public class DataStore: IDataStore
    {

        public IFSConfig FileSystemConfig { get; }


        public FolderObserver FolderObserver { get; } = new FolderObserver();


        private IFileSystem FileSystem { get; set; }
        private Repository GitRepository { get; set; }

        public DataStore(IFSConfig fsConfig)
        {
            FileSystemConfig = fsConfig;

            Initialize();
        }

        private void Initialize()
        {
            var path = Path.Combine(FileSystemConfig.RootPath, "DataStorage");
            FileSystem = new FileSystem(FileSystemConfig.RootPath);
            
            if(!Repository.IsValid(FileSystem.GetEntry("").PhysicalPath()))
            {
                Repository.Init(FileSystem.GetEntry("").PhysicalPath());
            }
            GitRepository = new Repository(FileSystem.GetEntry("").PhysicalPath());

            
            var obs = FileSystem.Observe("**", FolderObserver);
            
        }


        

        public void GetEntry(string path)
        {
            FileSystem.GetEntry(path);
        }

        //public IEnumerable<StoreItem> GetEntriesRecursive()
        //{
        //    FileScanner filescanner = new FileScanner(FileSystem)
        //        .AddGlobPattern("**/**")
        //        .AddGlobPattern("**")
        //        .SetDirectoryEvaluator(entry => entry.Name != ".git")
        //        .SetReturnDirectories(true)
        //        .SetReturnFiles(true);
            
        //    foreach (var entry in filescanner.Where(e => e.Name != ".git"))
        //    {
        //        yield return BuildStoreItem(entry);
        //    }
        //}

        

        private StoreItem BuildStoreItem(IEntry entry)
        {

            var fileItem = new FileInfo(entry.PhysicalPath());
            var item = new StoreItem();

            var parentPartLength = entry.Name?.LastIndexOf(".");
            var name = parentPartLength.HasValue && parentPartLength.Value > 0 ? entry.Name.Substring(0, parentPartLength.Value ) : entry.Name;
            item.IsFolder = entry.IsDirectory();
            item.Parent = ExtractParent(entry);
            item.Name = name;
            item.Extension = item.IsFolder ? null : entry.Name.Split(".").Last();
            item.CreatedAt = fileItem.CreationTimeUtc;
            item.UpdatedAt = fileItem.LastWriteTimeUtc;
            
            return item;


        }

        private string ExtractParent(IEntry entry)
        {
            int removeLength = entry.Name.Length;
            if (entry.IsDirectory())
            {
                removeLength++;
            }

            var length = entry.Path.Length - removeLength;
            if(length < 0)
            {
                return entry.Path;
            }
            return entry.Path.Substring(0, length).Trim("/");
        }


        public void Move(string path, string newPath)
        {
            var oldEntry = FileSystem.GetEntry(path).AssertExists();
            FileSystem.Move(oldEntry.Path, newPath);
            var newEntry = FileSystem.GetEntry(newPath);
            

            Commands.Stage(GitRepository, oldEntry.PhysicalPath());
            Commands.Stage(GitRepository, newEntry.PhysicalPath());
            Signature author = new Signature("Bernhard", "bwi@doob.at", DateTime.Now);
            Signature committer = author;

            GitRepository.Commit($"File Renamed: {path} -> {newPath}", author, committer);
        }

        public IEnumerable<IStoreItem> GetItemsInPath(string parent, bool recursive = default)
        {
            var p = string.Empty;
            if (!String.IsNullOrWhiteSpace(parent))
            {
                p = $"{parent.Trim("/".ToCharArray())}/";
            }

            FileScanner fileScanner = new FileScanner(FileSystem)
                .AddGlobPattern($"{p}**")
                .SetDirectoryEvaluator(entry => recursive && entry.Name != ".")
                .SetReturnDirectories(false)
                .SetReturnFiles(true);

            foreach (var entry in fileScanner.Where(e => e.Name != "."))
            {
                yield return BuildStoreItem(entry);
            }
        }

        public IStoreItem GetItem(string parent, string name)
        {


            var regexName = Regex.Escape(name);

            var fileEntry = new FileScanner(FileSystem)
                .AddRegex(parent, new Regex(regexName + @"\.[A-Za-z1-9]+"))
                .SetReturnFiles(true)
                .FirstOrDefault();

            if (fileEntry == null)
                return null;

            var item = BuildStoreItem(fileEntry);
            
            
            //var metaScanner = new FileScanner(FileSystem)
            //    .AddRegex(parent, new Regex(regexName + @"\.\[.*\]\.[A-Za-z1-9]+"))
            //    .SetReturnFiles(true);

            //var metaRegex = new Regex(@".*\.\[(?<flags>.*)\]\.[A-Za-z1-9]+");
            
            //foreach (var entry in metaScanner)
            //{
            //    var fileName = entry.Name.Split("/").Last();
            //    var match =  metaRegex.Match(fileName);
            //    if (match.Success)
            //    {
            //        var flag = match.Groups["flags"].Value;
            //        switch (flag?.ToLower())
            //        {
            //            case "notes":
            //            {
            //                item.Notes = File.ReadAllText(entry.PhysicalPath());
            //                item.NotesExtension = entry.Name.Split(".").Last();
            //                break;
            //            }
            //        }
            //    }
            //}

            return item;
        }

        public IEnumerable<IStoreItem> GetFolders(string parent = null)
        {
            FileScanner fileScanner = new FileScanner(FileSystem)
                .AddGlobPattern($"**/**")
                .SetDirectoryEvaluator(entry => entry.Name != ".")
                .SetReturnDirectories(true)
                .SetReturnFiles(false);
                

            foreach (var entry in fileScanner.Where(e => e.Name != "."))
            {
                yield return BuildStoreItem(entry);
            }

        }

        public FolderTreeNode GetFolderTree(string path = null)
        {
            path = path ?? "";
            var node = new FolderTreeNode();
            node.Path = path.Trim("/");
            node.Name = node.Path.Split("/").Last();

            var childs = FileSystem.Browse(path ?? "").Where(e => !e.Name.StartsWith(".")).Where(e => e.IsDirectory()).ToList();
            if (childs.Any())
            {
                node.Children = new List<FolderTreeNode>();
                foreach (var entry in childs)
                {
                   node.Children.Add(GetFolderTree(entry.Path));
                }
            }
            

            return node;

        }

        public void CreateDirectory(string parent, string name)
        {
            var path = $"{parent.Trim("/")}/{name.Trim("/")}/".TrimStart('/');
            FileSystem.CreateDirectory(path);
        }

        public void DeleteDirectory(string path)
        {
            path = $"{path.Trim("/")}/";
            FileSystem.Delete(path, true);
        }
    }
}

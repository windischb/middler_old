using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text.RegularExpressions;
using Lexical.FileSystem;
using Lexical.FileSystem.Utility;
//using LibGit2Sharp;
using middler.Variables.HelperClasses;
using Reflectensions;
using Reflectensions.ExtensionMethods;

namespace middler.Variables
{
    public class FolderObserver : IObserver<IEvent>
    {
        public void OnCompleted() => Console.WriteLine("OnCompleted");
        public void OnError(Exception error) => Console.WriteLine(error);

        private Subject<string> ChangedSubject { get; } = new Subject<string>();

        public IObservable<string> Changed => ChangedSubject.Throttle(TimeSpan.FromMilliseconds(200)).AsObservable();

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

    public class VariablesStore: IVariablesStore
    {

        public IFSConfig FileSystemConfig { get; }


        public FolderObserver FolderObserver { get; } = new FolderObserver();


        private IFileSystem FileSystem { get; set; }
        //private Repository GitRepository { get; set; }
        private Regex allowedCharacters { get; } = new Regex("^[a-zA-Z0-9_]+$");
        public VariablesStore(IFSConfig fsConfig)
        {
            FileSystemConfig = fsConfig;

            Initialize();
        }

        private void Initialize()
        {
            //var path = Path.Combine(FileSystemConfig.RootPath, "DataStorage");
            
            if (!Directory.Exists(FileSystemConfig.RootPath))
            {
                Directory.CreateDirectory(FileSystemConfig.RootPath);
            }

            FileSystem = new FileSystem(FileSystemConfig.RootPath);
            //if(!Repository.IsValid(FileSystem.GetEntry("").PhysicalPath()))
            //{
            //    Repository.Init(FileSystem.GetEntry("").PhysicalPath());
            //}
            //GitRepository = new Repository(FileSystem.GetEntry("").PhysicalPath());

            
            var obs = FileSystem.Observe("**", FolderObserver);
            
        }


        

        private VariableInfo BuildVariableInfo(IEntry entry)
        {

            var fileItem = new FileInfo(entry.PhysicalPath());
            var item = new VariableInfo();

            var parentFolderPartLength = entry.Name?.LastIndexOf(".");
            var name = parentFolderPartLength.HasValue && parentFolderPartLength.Value > 0 ? entry.Name.Substring(0, parentFolderPartLength.Value ) : entry.Name;
            item.IsFolder = entry.IsDirectory();
            item.Parent = ExtractParent(entry);
            item.Name = name;
            item.FullPath =  entry.Path;
            item.Extension = item.IsFolder ? null : $".{entry.Name.Split(".").Last().ToLower()}";
            item.CreatedAt = fileItem.CreationTimeUtc;
            item.UpdatedAt = fileItem.LastWriteTimeUtc;

            return item;

        }

        public IVariable BuildVariable(IEntry entry)
        {
            var item = BuildVariableInfo(entry);

            var content = File.ReadAllText(entry.PhysicalPath());

            try
            {
                switch (item.Extension?.ToLower())
                {
                    case ".credential":
                    {
                        var creds = Converter.Json.ToObject<SimpleCredentials>(content);
                        return item.ToVariable(creds);
                    }
                    case ".string":
                    {
                        return item.ToVariable(content);
                    }
                    case ".number":
                    {
                        return item.ToVariable(content.ToDecimal());
                    }
                    case ".boolean":
                    {
                        return item.ToVariable(content.ToBoolean());
                    }
                    default:
                    {
                        return item.ToVariable(content);
                    }
                }
            }
            catch
            {
                return item.ToVariable(content);
            }
            

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
            

            //Commands.Stage(GitRepository, oldEntry.PhysicalPath());
            //Commands.Stage(GitRepository, newEntry.PhysicalPath());
            //Signature author = new Signature("Bernhard", "bwi@doob.at", DateTime.Now);
            //Signature committer = author;

            //GitRepository.Commit($"File Renamed: {path} -> {newPath}", author, committer);
        }

        public IEnumerable<IVariableInfo> GetVariableInfosInFolder(string parentFolder, bool recursive = default)
        {
            var p = string.Empty;
            if (!String.IsNullOrWhiteSpace(parentFolder))
            {
                p = $"{parentFolder.Trim("/".ToCharArray())}/";
            }

            FileScanner fileScanner = new FileScanner(FileSystem)
                .AddGlobPattern($"{p}**")
                .SetDirectoryEvaluator(entry => recursive && entry.Name != ".")
                .SetReturnDirectories(false)
                .SetReturnFiles(true);

            foreach (var entry in fileScanner.Where(e => !e.Name.StartsWith(".")))
            {
                yield return BuildVariableInfo(entry);
            }
        }

        public IVariableInfo GetVariableInfo(string parentFolder, string name)
        {


            var regexName = Regex.Escape(name);

            var fileEntry = new FileScanner(FileSystem)
                .AddRegex(parentFolder, new Regex(regexName + @"\.[A-Za-z1-9]+"))
                .SetReturnFiles(true)
                .FirstOrDefault(e => !e.Name.StartsWith("."));

            if (fileEntry == null)
                return null;

            var item = BuildVariableInfo(fileEntry);
            return item;
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


        
        public void NewFolder(string parentFolder, string name)
        {
            if(!allowedCharacters.IsMatch(name))
                throw new Exception("Only following Characters allowed: 'a-zA-Z0-9_'");

            var path = BuildFolderPath(parentFolder, name);
            if (FolderExists(path))
                throw new Exception($"Folder '{path}' already exists!");
            
            FileSystem.CreateDirectory(path);
        }

        public void RenameFolder(string path, string name)
        {
            if(!allowedCharacters.IsMatch(name))
                throw new Exception("Only following Characters allowed: 'a-zA-Z0-9_'");

            if (!FolderExists(path))
                throw new Exception($"Folder '{path}' doesn't exists!");


            var pathparts = path.Trim("/").Split("/");
            var newPath = $"{String.Join("/", pathparts.Take(pathparts.Length - 1))}/{name.Trim("/")}/";

            if (path == newPath)
                return;

            if (FolderExists(newPath))
                throw new Exception($"Folder '{path}' already exists!");

            FileSystem.Move(path, newPath);
            
        }

        public void RemoveFolder(string path)
        {
            path = $"{path.Trim("/")}/";
            FileSystem.Delete(path, true);
        }


        public IVariable GetVariable(string path)
        {
            var (parentFolder, name, extension) = SplitVariablePath(path);
            return GetVariable(parentFolder, name, extension);
        }

        public IVariable GetVariable(string parentFolder, string name, string extension = null)
        {
            var entry = FindVariableEntry(parentFolder, name, extension);
            
            if (entry == null)
                return null;
            

            return BuildVariable(entry);

        }

        private (string parentFolder, string name, string extension) SplitVariablePath(string path)
        {
            string parentFolder = String.Empty;
            string name = String.Empty;
            string extension = String.Empty;

            var parts = path.Split("/");
            if (parts.Length == 0)
            {
                name = parts[0];
            }
            else
            {
                parentFolder = String.Join("/", parts.Take(parts.Length - 1));
                name = parts.Last();
            }

            if (name.Contains("."))
            {
                extension = name.Split('.')[1];
                name = name.Split('.')[0];
            }

            return (parentFolder, name, extension);
        }
        private IEntry FindVariableEntry(string path)
        {
            var (parentFolder, name, extension) = SplitVariablePath(path);

            return FindVariableEntry(parentFolder, name, extension);

        }
        private IEntry FindVariableEntry(string parentFolder, string name, string extension = null)
        {
            parentFolder = parentFolder ?? String.Empty;
            name = name ?? String.Empty;
            extension = extension?.Trim('.');

            Regex regex;
            if (string.IsNullOrWhiteSpace(extension))
            {
                regex = new Regex($"{name}\\.[A-Za-z1-9]+", RegexOptions.IgnoreCase);
            }
            else
            {
                regex = new Regex($"{name}\\.{extension}", RegexOptions.IgnoreCase);
            }
            

            
            var scanner = new FileScanner(FileSystem)
                .AddRegex(parentFolder, regex)
                .SetReturnFiles(true)
                .SetReturnDirectories(false)
                .SetDirectoryEvaluator((entry) => false);

                return scanner.FirstOrDefault();

        }

        private bool VariableExists(string path)
        {
            return FindVariableEntry(path) != null;
        }

        private bool VariableExists(string parentFolder, string name)
        {
            return FindVariableEntry(parentFolder, name) != null;
        }


        private string BuildFolderPath(string parentFolder, string name)
        {
            return $"{parentFolder?.Trim("/")}/{name.Trim("/")}/".TrimStart('/');
        }

        private IEntry FindFolderEntry(string parentFolder, string name)
        {
            var path = BuildFolderPath(parentFolder, name);
            return FindFolderEntry(path);
        }

        private IEntry FindFolderEntry(string path)
        {
            path = path?.TrimStart('/') ?? String.Empty;
            return FileSystem.GetEntry(path);
        }

        private bool FolderExists(string parentFolder, string name)
        {
            return FindFolderEntry(parentFolder, name) != null;
        }

        private bool FolderExists(string path)
        {
            return FindFolderEntry(path) != null;
        }


        public void UpdateVariableContent(string path, string content)
        {
            var entry = FindVariableEntry(path);
            if (entry == null)
                throw new Exception($"Variable '{path}' doesn't exists!");

            File.WriteAllText(entry.PhysicalPath(), content);
        }

        public void CreateVariable(IVariable variable)
        {
            if(!allowedCharacters.IsMatch(variable.Name))
                throw new Exception("Only following Characters allowed: 'a-zA-Z0-9_'");

            

            if (VariableExists(variable.Parent, variable.Name))
                throw new Exception($"Variable '{variable.Name}' already exists in Folder '{variable.Parent}'!");
            
            var path = $"{variable.Parent.Trim("/")}/{variable.Name}.{variable.Extension.Trim('.')}";
            FileSystem.CreateFile(path);
            var entry = FileSystem.GetEntry(path);
            File.WriteAllText(entry.PhysicalPath(), variable.Content?.ToString() ?? "");

        }
        public void RemoveVariable(string path)
        {
            path = path.Trim("/");
            FileSystem.Delete(path, false);
        }
    }
}

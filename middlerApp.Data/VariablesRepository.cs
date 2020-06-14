using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Reflectensions.ExtensionMethods;

namespace middlerApp.Data
{
    public class VariablesRepository
    {
        private readonly MiddlerDbContext _middlerDbContext;

        public IObservable<VariableStorageEvent> EventObservable => this.EventSubject.AsObservable();

        private Subject<VariableStorageEvent> EventSubject { get; } = new Subject<VariableStorageEvent>();


        public VariablesRepository(MiddlerDbContext middlerDbContext)
        {
            _middlerDbContext = middlerDbContext;
            _middlerDbContext.Database.EnsureCreated();

            //_middlerDbContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        public async Task<bool> ItemExists(string parent, string name)
        {
            return await GetItemAsync(parent, name) != null;
        }

        private async Task<TreeNode> GetItemAsync(string parent, string name)
        {
            return await _middlerDbContext.Variables.FirstOrDefaultAsync(it => it.Parent == parent && it.Name == name);
        }

        private TreeNode GetItem(string parent, string name)
        {
            return _middlerDbContext.Variables.FirstOrDefault(it => it.Parent == parent && it.Name == name);
        }

        private async Task DeleteItem(string parent, string name)
        {
            var item = await GetItemAsync(parent, name);
            _middlerDbContext.Variables.Remove(item);
            await _middlerDbContext.SaveChangesAsync();
        }

        private async Task CreateItem(TreeNode node)
        {
            node.CreatedAt = DateTime.Now;
            await _middlerDbContext.Variables.AddAsync(node);
            await _middlerDbContext.SaveChangesAsync();
        }

        private async Task UpdateItem(Expression<Func<TreeNode, bool>> expression, Action<TreeNode> update)
        {
            var items = await _middlerDbContext.Variables.Where(expression).ToListAsync();
            items.ForEach(f =>
            {
                update?.Invoke(f);
               
            });
            await _middlerDbContext.SaveChangesAsync();
        }

        private async Task UpdateItem(string parent, string name, Action<TreeNode> update)
        {
            await UpdateItem(it => it.Parent == parent && it.Name == name, update);
        }

        public async Task UpdateVariableContent(string parent, string name, object content)
        {

            //var variable = await GetVariableAsync(parent, name);

            //switch (variable.Extension)
            //{
            //    case "credential":
            //        {
            //            content = JsonSerializer.Deserialize(content.ToString());
            //            break;
            //        }
            //    default:
            //        {
            //            content = content.ToString();
            //            break;
            //        }
            //}

            await UpdateItem(parent, name, node =>
            {
                node.Content = JToken.FromObject(content);
                node.UpdatedAt = DateTime.Now;
            });
        }

        #region Folders

        public async Task<TreeNode> GetFolderTree()
        {
            var items = await _middlerDbContext.Variables.Where(it => it.IsFolder).OrderBy(it => it.Parent).ThenBy(it => it.Name).ToListAsync();
            var rootItem = new TreeNode
            {
                IsFolder = true
            };

            foreach (var treeNode in items)
            {
                var parent = treeNode.Parent.ToNull() == null ? rootItem : rootItem.GetNodeByPath(treeNode.Parent);
                if (parent == null)
                {
                    throw new Exception("Error...");
                }
                else
                {
                    if (parent.Children == null)
                    {
                        parent.Children = new List<TreeNode>();
                    }
                    parent.Children.Add(treeNode);
                }
            }

            return rootItem;
        }

        public async Task NewFolder(string parent, string name)
        {
            var item = new TreeNode
            {
                Parent = parent.Trim('/').ToNull(),
                Name = name.Trim('/'),
                IsFolder = true
            };

            await CreateItem(item);
            EventSubject.OnNext(new VariableStorageEvent(VariableStorageAction.Insert, null));
        }

        public async Task RenameFolder(string parent, string oldName, string newName)
        {
            var oldpath = $"{parent}/{oldName}".Trim('/');
            var newPath = $"{parent}/{newName}".Trim('/');
            var startsWithOldPath = $"{oldpath}/";
            var startsWithNewPath = $"{newPath}/";

            await UpdateItem(parent, oldName, item => item.Name = newName);
            await UpdateItem(item => item.Parent == oldpath, item => item.Parent = newPath);
            await UpdateItem(item => item.Parent.StartsWith(startsWithOldPath), item => item.Parent = item.Parent.Replace(startsWithOldPath, startsWithNewPath));
            EventSubject.OnNext(new VariableStorageEvent(VariableStorageAction.Update, null));
        }

        public async Task RemoveFolder(string parent, string name)
        {
            var parentPath = $"{parent}/{name}".Trim('/');
            var startsWithParentPath = $"{parentPath}/";
            var found = await _middlerDbContext.Variables.Where(it =>
                (it.Parent == parent && it.Name == name) || it.Parent == parentPath ||
                it.Parent.StartsWith(startsWithParentPath)).ToListAsync();
           
            _middlerDbContext.Variables.RemoveRange(found);
            await _middlerDbContext.SaveChangesAsync();
            EventSubject.OnNext(new VariableStorageEvent(VariableStorageAction.Delete, null));
        }

        #endregion

        #region Variables

        public async Task<List<TreeNode>> GetVariablesInParent(string parent)
        {
            return await _middlerDbContext.Variables.Where(it => it.Parent == parent && it.IsFolder == false).ToListAsync();
        }

        public async Task<TreeNode> GetVariableAsync(string parent, string name)
        {
            return await GetItemAsync(parent, name);
        }

        public TreeNode GetVariable(string parent, string name)
        {
            return GetItem(parent, name);
        }

        #endregion

        public async Task CreateVariable(TreeNode variable)
        {
            variable.IsFolder = false;
            await CreateItem(variable);
        }

        public async Task RemoveVariable(string parent, string name)
        {
            await DeleteItem(parent, name);
        }

    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using LiteDB;
using middler.Common.Variables;
using Newtonsoft.Json.Linq;

namespace middler.Variables.LiteDB
{
    public class VariableStore
    {

        public IObservable<VariableStorageEvent> EventObservable => this.EventSubject.AsObservable();

        private Subject<VariableStorageEvent> EventSubject { get; } = new Subject<VariableStorageEvent>();

        private LiteRepository Repository { get; }

        public VariableStore(string connectionString)
        {

            var con = new ConnectionString(connectionString);

            var fi = new FileInfo(con.Filename);
            if (!fi.Directory.Exists)
            {
                fi.Directory.Create();
            }

            Repository = new LiteRepository(con);
            Repository.Database.Mapper.Entity<TreeNode>()
                .Id(emp => emp.Id, true)
                .Ignore(it => it.Children);

        }
        

        public bool ItemExists(string parent, string name)
        {
            return GetItem(parent, name) != null;
        }

        private TreeNode GetItem(string parent, string name)
        {
            return Repository.First<TreeNode>(it => it.Parent == parent && it.Name == name);
        }

        private void DeleteItem(string parent, string name)
        {
            Repository.DeleteMany<TreeNode>(it => it.Parent == parent && it.Name == name);
        }

        private void CreateItem(TreeNode node)
        {
            node.CreatedAt = DateTime.Now;
            Repository.Insert(node);
        }

        private void UpdateItem(Expression<Func<TreeNode, bool>> expression, Action<TreeNode> update)
        {
            var found = Repository.Fetch(expression);
            found.ForEach(f =>
            {
                update?.Invoke(f);
                Repository.Update(f);
            });
        }

        private void UpdateItem(string parent, string name, Action<TreeNode> update)
        {
            UpdateItem(it => it.Parent == parent && it.Name == name, update);
        }

        #region Folders

        public TreeNode GetFolderTree()
        {
            var items = Repository.Fetch<TreeNode>(it => it.IsFolder).OrderBy(it => it.Parent).ThenBy(it => it.Name);
            var rootItem = new TreeNode
            {
                IsFolder = true
            };

            foreach (var treeNode in items)
            {
                var parent = treeNode.Parent == null ? rootItem : rootItem.GetNodeByPath(treeNode.Parent);
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

        public void NewFolder(string parent, string name)
        {
            var item = new TreeNode
            {
                Parent = parent.Trim('/'), 
                Name = name.Trim('/'),
                IsFolder = true
            };

            CreateItem(item);
            EventSubject.OnNext(new VariableStorageEvent(VariableStorageAction.Insert, null));
        }

        public void RenameFolder(string parent, string oldName, string newName)
        {
            var oldpath = $"{parent}/{oldName}".Trim('/');
            var newPath = $"{parent}/{newName}".Trim('/');
            var startsWithOldPath = $"{oldpath}/";
            var startsWithNewPath = $"{newPath}/";
            UpdateItem(parent, oldName, item => item.Name = newName);
            UpdateItem(item => item.Parent == oldpath, item => item.Parent = newPath);
            UpdateItem(item => item.Parent.StartsWith(startsWithOldPath), item => item.Parent = item.Parent.Replace(startsWithOldPath, startsWithNewPath));
            EventSubject.OnNext(new VariableStorageEvent(VariableStorageAction.Update, null));
        }

        public void RemoveFolder(string parent, string name)
        {
            var parentPath = $"{parent}/{name}".Trim('/');
            var startsWithParentPath = $"{parentPath}/";
            Repository.DeleteMany<TreeNode>(it => it.Parent == parent && it.Name == name);
            Repository.DeleteMany<TreeNode>(it => it.Parent == parentPath);
            Repository.DeleteMany<TreeNode>(it => it.Parent.StartsWith(startsWithParentPath));

            EventSubject.OnNext(new VariableStorageEvent(VariableStorageAction.Delete, null));
        }

        #endregion

        #region Variables

        public List<TreeNode> GetVariablesInParent(string parent)
        {
            return Repository.Fetch<TreeNode>(it => it.Parent == parent && it.IsFolder == false);
        }

        public TreeNode GetVariable(string parent, string name)
        {
            return GetItem(parent, name);
        }

        #endregion

        public void CreateVariable(TreeNode variable)
        {
            if (variable.Content is JObject jo)
            {
                variable.Content = JsonSerializer.Deserialize(jo.ToString());
            }
            variable.IsFolder = false;
            CreateItem(variable);
        }

        public void RemoveVariable(string parent, string name)
        {
            DeleteItem(parent, name);
        }
    }
    
}

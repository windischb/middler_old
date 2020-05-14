using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using middler.Common.Variables;
using middler.Variables;
using middler.Variables.LiteDB;
using middlerApp.API.Helper;
using Newtonsoft.Json.Linq;
using SignalARRR.Attributes;
using SignalARRR.Server;


namespace middlerApp.API.HubMethods
{
    [MessageName("Variables")]
    public class VariablesServerMethods : ServerMethods<UIHub>
    {
        public VariableStore VariablesStore { get; }

        public VariablesServerMethods(VariableStore variablesStore)
        {
            VariablesStore = variablesStore;
        }

        public TreeNode GetFolderTree()
        {
            return VariablesStore.GetFolderTree();
        }

        public IEnumerable<TreeNode> GetVariablesInParent(string parent)
        {
            return VariablesStore.GetVariablesInParent(parent);
        }

        public void NewFolder(string parent, string name)
        {
            VariablesStore.NewFolder(parent, name);
        }
        public void RenameFolder(string parent, string oldName, string newName)
        {
            VariablesStore.RenameFolder(parent, oldName, newName);
        }

        public void RemoveFolder(string parent, string name)
        {
            VariablesStore.RemoveFolder(parent, name);
        }


        public TreeNode GetVariable(string parent, string name)
        {
            return VariablesStore.GetVariable(parent, name);
        }

        //public void UpdateVariableContent(string path, object content)
        //{
        //    string contentString = null;
        //    switch (content)
        //    {
        //        case String str:
        //            {
        //                contentString = content.ToString();
        //                break;
        //            }

        //        default:
        //            {
        //                contentString = Converter.Json.ToJson(content);
        //                break;
        //            }
        //    }

        //    VariablesStore.UpdateVariableContent(path, contentString);
        //}


        public void CreateVariable(TreeNode variable)
        {
            VariablesStore.CreateVariable(variable);
        }

        public void RemoveVariable(string parent, string name)
        {
            VariablesStore.RemoveVariable(parent, name);
        }

        //public List<KeyValuePair<string, string>> GetTypings()
        //{
        //    var typings =
        //        Directory.GetFileSystemEntries(PathHelper.GetFullPath("TypeDefinitions"))
        //            .Select(fe =>
        //            {
        //                var f = new FileInfo(fe);
        //                return new KeyValuePair<string, string>(f.Name, System.IO.File.ReadAllText(fe));
        //            }).ToList();

        //    return typings;
        //}

        public IObservable<string> Subscribe()
        {
            return (VariablesStore).EventObservable.Select(ev => ev.Action.ToString());
        }
    }
}

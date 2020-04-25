using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using middler.Variables;
using middlerApp.API.Helper;
using Newtonsoft.Json.Linq;
using SignalARRR.Attributes;
using SignalARRR.Server;
using Converter = middler.Variables.Converter;

namespace middlerApp.API.HubMethods
{
    [MessageName("Variables")]
    public class VariablesServerMethods : ServerMethods<UIHub>
    {
        public IVariablesStore VariablesStore { get; }

        public VariablesServerMethods(IVariablesStore variablesStore)
        {
            VariablesStore = variablesStore;
        }

        public FolderTreeNode GetFolderTree()
        {
            return VariablesStore.GetFolderTree();
        }

        public IEnumerable<IVariableInfo> GetVariableInfosInParent(string parent)
        {
            return VariablesStore.GetVariableInfosInFolder(parent);
        }

        public void NewFolder(string parent, string name)
        {
            VariablesStore.NewFolder(parent, name);
        }
        public void RenameFolder(string path, string name)
        {
            VariablesStore.RenameFolder(path, name);
        }

        public void RemoveFolder(string path)
        {
            VariablesStore.RemoveFolder(path);
        }


        public IVariable GetVariable(string parent, string name)
        {
            return VariablesStore.GetVariable(parent, name);
        }

        public void UpdateVariableContent(string path, object content)
        {
            string contentString = null;
            switch (content)
            {
                case String str:
                    {
                        contentString = content.ToString();
                        break;
                    }

                default:
                    {
                        contentString = Converter.Json.ToJson(content);
                        break;
                    }
            }

            VariablesStore.UpdateVariableContent(path, contentString);
        }


        public void CreateVariable(Variable variable)
        {
            VariablesStore.CreateVariable(variable);
        }

        public void RemoveVariable(string path)
        {
            VariablesStore.RemoveVariable(path);
        }

        public List<KeyValuePair<string, string>> GetTypings()
        {
            var typings =
                Directory.GetFileSystemEntries(PathHelper.GetFullPath("TypeDefinitions"))
                    .Select(fe =>
                    {
                        var f = new FileInfo(fe);
                        return new KeyValuePair<string, string>(f.Name, System.IO.File.ReadAllText(fe));
                    }).ToList();

            return typings;
        }

        public IObservable<string> Subscribe()
        {
            return ((VariablesStore)VariablesStore).FolderObserver.Changed;
        }
    }
}

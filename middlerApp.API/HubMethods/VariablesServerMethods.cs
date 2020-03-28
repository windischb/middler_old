using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using middler.DataStore;
using SignalARRR.Attributes;
using SignalARRR.Server;

namespace middlerApp.API.HubMethods
{
    [MessageName("Variables")]
    public class VariablesServerMethods: ServerMethods<UIHub>
    {
        public IDataStore DataStore { get; }

        public VariablesServerMethods(IDataStore dataStore)
        {
            DataStore = dataStore;
        }

        public FolderTreeNode GetFolderTree()
        {
            return DataStore.GetFolderTree();
        }

        public IEnumerable<IStoreItem> GetItemsInPath(string parent)
        {
            return DataStore.GetItemsInPath(parent);
        }

        public void CreateDirectory(string parent, string name)
        {
            DataStore.CreateDirectory(parent, name);
        }

        public void DeleteDirectory(string path)
        {
            DataStore.DeleteDirectory(path);
        }

        public IObservable<string> Subscribe() {
            return ((DataStore)DataStore).FolderObserver.Changed;
        }
    }
}

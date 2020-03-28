using System;
using System.Collections.Generic;
using System.Text;

namespace middler.DataStore
{
    public interface IDataStore
    {

        IEnumerable<IStoreItem> GetItemsInPath(string parent, bool recursive = default);

        IStoreItem GetItem(string parent, string name);


        IEnumerable<IStoreItem> GetFolders(string parent = null);

        FolderTreeNode GetFolderTree(string path = null);

        void CreateDirectory(string parent, string name);

        void DeleteDirectory(string path);
    }
}

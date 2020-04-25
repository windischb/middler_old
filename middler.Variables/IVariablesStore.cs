using System.Collections.Generic;

namespace middler.Variables
{
    public interface IVariablesStore
    {

        #region Folders
        FolderTreeNode GetFolderTree(string path = null);

        void NewFolder(string folder, string name);

        void RenameFolder(string path, string name);

        void RemoveFolder(string path);

        #endregion


        #region VariableInfos
        IEnumerable<IVariableInfo> GetVariableInfosInFolder(string folder, bool recursive = default);

        IVariableInfo GetVariableInfo(string folder, string name);

        #endregion

        
        #region Variables

        IVariable GetVariable(string folder, string name, string extension = null);

        IVariable GetVariable(string path);

        void CreateVariable(IVariable variable);

        void UpdateVariableContent(string path, string content);

        void RemoveVariable(string path);

        #endregion

        
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using LiteDB;

namespace middler.Variables
{
    public class VariableLiteDbStore: IVariablesStore
    {
        private LiteRepository Repository { get; }

        public VariableLiteDbStore(string connectionString)
        {

            var con = new ConnectionString(connectionString);

            var fi = new FileInfo(con.Filename);
            if (!fi.Directory.Exists)
            {
                fi.Directory.Create();
            }

            Repository = new LiteRepository(con);
            Repository.Database.Mapper.Entity<Variable>().Id(emp => emp.FullPath, true);
        }
        public FolderTreeNode GetFolderTree(string path = null)
        {
            throw new NotImplementedException();
        }

        public void NewFolder(string folder, string name)
        {
            throw new NotImplementedException();
        }

        public void RenameFolder(string path, string name)
        {
            throw new NotImplementedException();
        }

        public void RemoveFolder(string path)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IVariableInfo> GetVariableInfosInFolder(string folder, bool recursive = default)
        {
            throw new NotImplementedException();
        }

        public IVariableInfo GetVariableInfo(string folder, string name)
        {
            throw new NotImplementedException();
        }

        public IVariable GetVariable(string folder, string name, string extension = null)
        {
            throw new NotImplementedException();
        }

        public IVariable GetVariable(string path)
        {
            throw new NotImplementedException();
        }

        public void CreateVariable(IVariable variable)
        {
            throw new NotImplementedException();
        }

        public void UpdateVariableContent(string path, string content)
        {
            throw new NotImplementedException();
        }

        public void RemoveVariable(string path)
        {
            throw new NotImplementedException();
        }
    }
}

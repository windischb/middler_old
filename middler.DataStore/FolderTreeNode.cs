using System;
using System.Collections.Generic;
using System.Text;

namespace middler.DataStore
{

   
    public class FolderTreeNode
    {
        public string Path { get; set; }
        public string Name { get; set; }
        public ICollection<FolderTreeNode> Children { get; set; } = new List<FolderTreeNode>();
    }
}

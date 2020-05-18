using System.Collections.Generic;

namespace middler.Variables
{

   
    public class FolderTreeNode
    {
        public string Path { get; set; }
        public string Name { get; set; }
        public ICollection<FolderTreeNode> Children { get; set; } = new List<FolderTreeNode>();
    }
}

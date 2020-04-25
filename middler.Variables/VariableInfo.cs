using System;

namespace middler.Variables
{
    public class VariableInfo : IVariableInfo
    {

        public string Parent { get; set; }
        public string Name { get; set; }
        public string FullPath { get; set; }  // => $"{Parent?.Trim('/')}/{Name?.Trim('/')}" + $".{Extension}";

        public string Extension { get; set; }

        public bool IsFolder { get; set; }

        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public VariableFlags? Flags { get; set; }

        /// <summary>
        /// comes from a File with same Name in same Path but with extension "\.\[desc\]\.[html|md|txt]"
        /// </summary>
        //public string Notes { get; set; }

        //public string NotesExtension { get; set; }
        
        /// <summary>
        /// if Name matches ".*\[.*sec.*\]\..*"
        /// </summary>
        //public bool Secure { get; set; }
    }
}

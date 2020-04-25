using System;

namespace middler.Variables
{
    public interface IVariableInfo
    {
        string Parent { get; set; }
        string Name { get; set; }

        string FullPath { get; }

        string Extension { get; set; }
        bool IsFolder { get; set; }
        DateTime? CreatedAt { get; set; }
        DateTime? UpdatedAt { get; set; }

        VariableFlags? Flags { get; set; }
        
    }
}
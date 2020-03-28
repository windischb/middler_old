using System;

namespace middler.DataStore
{
    public interface IStoreItem
    {
        string Parent { get; set; }
        string Name { get; set; }

        string Extension { get; set; }
        bool IsFolder { get; set; }
        DateTime CreatedAt { get; set; }
        DateTime UpdatedAt { get; set; }

        //string Notes { get; set; }
        //string NotesExtension { get; set; }
        //bool Secure { get; set; }
    }
}
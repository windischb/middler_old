using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace middler.DataStore
{
    public class StoreItem : IStoreItem
    {

        public string Parent { get; set; }

        public string Name { get; set; }

        public string Extension { get; set; }

        public bool IsFolder { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
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

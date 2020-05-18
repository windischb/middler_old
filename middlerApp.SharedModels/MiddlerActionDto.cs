using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace middler.Hosting.Models {
    public class MiddlerActionDto {

        public virtual bool Terminating { get; set; }
        public virtual bool WriteStreamDirect { get; set; }
        public virtual bool Enabled { get; set; }
        public string ActionType { get; set; }
        public Dictionary<string, object> Parameters { get; set; } = new Dictionary<string, object>();

    }
}

using System.Collections.Generic;
using middler.Common.Interfaces;

namespace middler.Common.Models {
    public class MiddlerAction: IMiddlerAction {

        public virtual bool ContinueAfterwards { get; set; } = false;
        public virtual bool WriteStreamDirect { get; set; } = false;

        public string ActionType { get; set; }

        public Dictionary<string, object> Parameters { get; set; } = new Dictionary<string, object>();

    }

    public class MiddlerAction<T> : IMiddlerAction where T : class, new() {
       

        public virtual bool ContinueAfterwards { get; set; } = false;
        public virtual bool WriteStreamDirect { get; set; } = false;

        //public string ActionType { get; }

        public T Parameters { get; set; } = new T();

    }

}

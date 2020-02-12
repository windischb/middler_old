using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace middler.Hosting.Models {
    public class MiddlerActionDto {

        public string ActionType { get; set; }
        public object Parameters { get; set; }

    }
}

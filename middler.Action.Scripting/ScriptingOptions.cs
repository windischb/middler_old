using System;
using System.Collections.Generic;
using System.Text;
using middler.Action.Scripting.Shared;

namespace middler.Action.Scripting
{
    public class ScriptingOptions : IScriptingOptions
    {
        public ScriptLanguage Language { get; set; }

        public string SourceCode { get; set; }

        public string CompiledCode { get; set; }
    }
}

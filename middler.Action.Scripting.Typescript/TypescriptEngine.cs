using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Esprima;
using Jint;
using middler.Action.Scripting.Javascript;
using middler.Action.Scripting.Shared;

namespace middler.Action.Scripting.Typescript
{
    public class TypescriptEngine: IScriptEngine
    {
        public System.Action<IScriptingOptions> CompileScript => CompileScriptInternal;


        private JavascriptEngine _javascriptEngine;
        
        public static ParserOptions EsprimaOptions = new ParserOptions {
            Tolerant = true
        };

       
        public TypescriptEngine()
        {
            _javascriptEngine = new JavascriptEngine();
        }

        public void Initialize()
        {
            _javascriptEngine.Initialize();


        }

        
        public void Stop()
        {
            _javascriptEngine.Stop();
        }


        public void SetValue(string name, object value)
        {
            _javascriptEngine.SetValue(name, value);

        }

        public string GetValue(string name)
        {
            return _javascriptEngine.GetValue(name);
        }


        public Task Execute(string script)
        {
            return _javascriptEngine.Execute(script);

        }

        public string Invoke(string script)
        {
            return _javascriptEngine.Invoke(script);

        }


        private static Esprima.Ast.Program TypeScriptProgram;
        private void CompileScriptInternal(IScriptingOptions options)
        {
            if (String.IsNullOrWhiteSpace(options.SourceCode))
                return;

            if (TypeScriptProgram == null) {
                var tsLib = GetFromResources("typescript.min.js");
                var parser = new JavaScriptParser(tsLib, EsprimaOptions);
                
                TypeScriptProgram = parser.ParseProgram();
            }

           
            
            var _engine = new Engine();
            
            _engine.Execute(TypeScriptProgram);


           
            _engine.SetValue("src", options.SourceCode);


            var transpileOtions = "{\"compilerOptions\": {\"target\":\"ES5\"}}";

            var output = _engine.Execute($"ts.transpileModule(src, {transpileOtions})", EsprimaOptions).GetCompletionValue().AsObject();
            var result = output.Get("outputText").AsString();


            options.CompiledCode = result;


        }

        public static string GetFromResources(string resourceName)
        {
            var type = typeof(TypescriptEngine);

            using (Stream stream = type.Assembly.GetManifestResourceStream($"{type.Namespace}.{resourceName}"))
            {
                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }

            }
        }


        public void Dispose()
        {
            _javascriptEngine.Dispose();
        }
    }
}

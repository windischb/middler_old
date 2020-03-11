using System;
using System.Threading.Tasks;

namespace middler.Action.Scripting.Shared
{
    public interface IScriptEngine: IDisposable
    {

        System.Action<IScriptingOptions> CompileScript { get; }

        bool NeedsCompiledScript => this.CompileScript != null;

        void Initialize();

        void SetValue(string name, object value);
        string GetValue(string name);

        Task Execute(string script);

        string Invoke(string script);

        void Stop();
    }
}

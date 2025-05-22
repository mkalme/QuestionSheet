using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace CodeExecutor
{
    public class Executor
    {
        private readonly static ScriptOptions OPTIONS;

        static Executor() {
            OPTIONS = ScriptOptions.Default
            .AddReferences(AppDomain.CurrentDomain.GetAssemblies())
            .AddImports("System");
        }

        public double Execute(string code) {
            var scriptState = CSharpScript.RunAsync(code, OPTIONS).Result;
            return scriptState.ContinueWithAsync<double>("Calculator.Calculate()").Result.ReturnValue;
        }
    }
}
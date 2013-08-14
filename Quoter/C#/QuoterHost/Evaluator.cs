using Roslyn.Compilers.Common;
using Roslyn.Compilers.CSharp;
using Roslyn.Scripting;
using Roslyn.Scripting.CSharp;

public class Evaluator
{
    private ScriptEngine engine;
    private Session session;

    public Evaluator()
    {
        engine = new ScriptEngine();
        session = engine.CreateSession();
        session.ImportNamespace("Roslyn.Compilers");
        session.ImportNamespace("Roslyn.Compilers.CSharp");
        session.AddReference(typeof(CommonSyntaxNode).Assembly);
        session.AddReference(typeof(SyntaxNode).Assembly);
    }

    public object Evaluate(string code)
    {
        var result = session.Execute(code);
        return result;
    }
}

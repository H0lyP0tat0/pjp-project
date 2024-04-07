using Antlr4.Runtime;

namespace AntlrTest6._0;

public static class ErrorController
{
    public static readonly List<string> errors = new List<string>();

    public static void AddError(ParserRuleContext context, string error = "")
    {
        error = error switch
        {
            "" => "is not a valid operation",
            "type" => "is not a valid type",
            "bool" => "is not a boolean expression",
            "num" => "is not a number",
            "assign" => "invalid assignment",
            _ => error
        };

        errors.Add(context.GetText() + " at line " + context.Start.Line + " " + error);
    }

    public static void PrintErrors()
    {
        foreach (var error in errors)
        {
            Console.WriteLine(error);
        }
    }
}
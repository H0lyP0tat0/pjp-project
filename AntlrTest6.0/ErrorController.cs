using Antlr4.Runtime;

namespace AntlrTest6._0;

public static class ErrorController
{
    public static readonly List<string> errors = new List<string>();

    public static void AddError(ParserRuleContext context, string error = "")
    {
        error = error switch
        {
            "" => "invalid operation",
            "type" => "invalid type",
            "bool" => "is not a boolean expression",
            "num" => "is not a number",
            "assign" => "invalid assignment",
            _ => error
        };

        errors.Add(context.GetText() + " at line " + context.Start.Line + " " + error);
    }

    public static void PrintErrors()
    {
        Console.WriteLine("\n" + "\u001B[31m" + "### Error Controller ###");
        if (errors.Count == 0) Console.WriteLine("No errors found.");
        
        foreach (var error in errors)
        {
            Console.WriteLine("\u001B[31m" + error);
        }
    }
}
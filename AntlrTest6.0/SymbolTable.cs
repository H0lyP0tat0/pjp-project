using Antlr4.Runtime;

namespace AntlrTest6._0;

public static class SymbolTable
{
    private static readonly Dictionary<string, Type> table = new Dictionary<string, Type>();

    public static Type Add(IToken var, Type type)
    {
        var name = var.Text.Trim();
        if (table.TryAdd(name, type)) return Type.NULL;
        
        ErrorController.errors.Add($"Variable {name} already declared");
        return Type.ERROR;
    }
    
    public static Type Get(IToken var)
    {
        var name = var.Text.Trim();
        return table.GetValueOrDefault(name, Type.ERROR);
    }

    public static void PrintTable()
    {
        Console.WriteLine("\n### Symbol Table ###");
        foreach (var symbol in table)
        {
            Console.WriteLine(symbol.Key + ": " + symbol.Value);
        }
    }
}
namespace AntlrTest6._0;

public class VirtualMachine
{
    public static Stack<string> stack = new Stack<string>();
    public static List<string> code = new List<string>();
    public static int labelCount = 0; 

    public static void PrintInstructions()
    {
        Console.WriteLine("\n### Instructions ###");
        foreach (var line in VirtualMachine.code)
        {
            Console.WriteLine(line);
        }
        
        File.WriteAllLines("output.txt", VirtualMachine.code);
    }
    
    public static void Push(Type type, string value = "")
    {
        string typeText = type switch
        {
            Type.INT => "I",
            Type.FLOAT => "F",
            Type.BOOL => "B",
            Type.STRING => "S",
            _ => throw new Exception("Unexpected type")
        };
        
        if (value != "")
        {
            code.Add($"push {typeText} {value}");
            return;
        }
        
        string valueText = type switch
        {
            Type.INT => "0",
            Type.FLOAT => "0.0",
            Type.BOOL => "true",
            Type.STRING => "\"\"",
            _ => throw new Exception("Unexpected type")
        };
        
        code.Add($"push {typeText} {valueText}");
    }

    public static void Itof()
    {
        code.Add("itof");
    }

    public static void Expression(string op)
    {
        string opText = op switch
        {
            "+" => "add",
            "-" => "sub",
            "*" => "mul",
            "/" => "div",
            "%" => "mod",
            "." => "concat",
            _ => throw new Exception("Unexpected operation")
        };
        
        code.Add(opText);
    }
    
    public static void Equal(string op)
    {
        string[] opText = op switch
        {
            "==" => ["eq"],
            "!=" => ["eq", "not"],
            _ => throw new Exception("Unexpected operation")
        };
        
        code.AddRange(opText);
    }

    public static void LessGreater(string op)
    {
        string opText = op switch
        {
            "<" => "lt",
            ">" => "gt",
            _ => throw new Exception("Unexpected operation")
        };
        
        code.Add(opText);
    }

    public static void AndOrNot(string op)
    {
        string opText = op switch
        {
            "&&" => "and",
            "||" => "or",
            "!" => "not",
            _ => throw new Exception("Unexpected operation")
        };
        
        code.Add(opText);
    }

    public static void SaveLoad(string action, string name)
    {
        code.Add($"{action} {name}");
    }

    public static void Pop()
    {
        code.Add("pop");
    }

    public static void Print(int length)
    {
        code.Add($"print {length}");
    }
}
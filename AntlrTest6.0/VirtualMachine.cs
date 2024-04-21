namespace AntlrTest6._0;

public class VirtualMachine
{
    public static Stack<object> stack = new Stack<object>();
    static Dictionary<string, object> memory = new Dictionary<string, object>();
    public static List<string> code = new List<string>();
    private static int labelCount = -1;
    private static bool firstStatement = true;

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

    public static void Expression(string op, string type="I")
    {
        string opText = op switch
        {
            "+" => $"add {type}",
            "-" => $"sub {type}",
            "*" => $"mul {type}",
            "/" => $"div {type}",
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

    public static void Label()
    {
        if (labelCount != -1)
            code.Add($"label {labelCount}");
        labelCount++;
    }

    public static int NextLabel()
    {
        return labelCount;
    }

    public static void Jump(int label, string type = "")
    {
        switch (type)
        {
            case "":
                code.Add($"jmp {label}");
                break;
            case "f":
                code.Add($"fjmp {label}");
                break;
        }
    }

    public static void Run(string fileName)
    {
        var lines = File.ReadAllLines(fileName);
        for (var i = 0; i < lines.Length; i++)
        {
            var instruction = lines[i].Split(' ');
            if (instruction.Length > 3)
            {
                instruction[2] = string.Join(" ", instruction.Skip(2));
            }
            
            switch (instruction[0])
            {
                case "push":
                    RunPush(instruction);
                    break;
                case "print":
                    RunPrint(int.Parse(instruction[1]));
                    break;
                case "pop":
                    stack.Pop();
                    break;
                case "save":
                    RunSave(instruction);
                    break;
                case "load":
                    RunLoad(instruction);
                    break;
                case "read":
                    RunRead(instruction);
                    break;
                case "label":
                    break;
                case "jmp":
                    i = RunJmp(lines, instruction);
                    break;
                case "fjmp":
                    i = RunFjmp(i, instruction, lines);
                    break;
                default:
                    RunExpression(instruction);
                    break;
            }
        }
        
        Console.WriteLine("\n### Stack ###");
        foreach (var item in stack)
        {
            Console.WriteLine(item + " - " + item.GetType());
        }
        
        Console.WriteLine("\n### Memory ###");
        foreach (var item in memory)
        {
            Console.WriteLine($"{item.Key}: {item.Value} - {item.Value.GetType()}");
        }
    }
    
    private static int RunFjmp(int i, string[] instruction, string[] lines)
    {
        if (!(bool)stack.Pop())
        {
            return RunJmp(lines, instruction);
        }
        return i;
    }
    
    private static int RunJmp(string[] lines, string[] instruction)
    {
        foreach (var line in lines)
        {
            var lIn = line.Split(' ');
            if (lIn[0] == "label" && lIn[1] == instruction[1])
            {
                return Array.IndexOf(lines, line);
            }
        }
        return -1;
    }

    private static void RunPush(string[] instruction)
    {
        switch (instruction[1])
        {
            case "I":
                stack.Push(int.Parse(instruction[2]));
                break;
            case "F":
                stack.Push(float.Parse(instruction[2]));
                break;
            case "B":
                stack.Push(bool.Parse(instruction[2]));
                break;
            case "S":
                stack.Push(instruction[2]);
                break;
        }
    }
    
    private static void RunPrint(int length)
    {
        var elements = new List<object>();
        
        for (var i = 0; i < length; i++)
        {
            elements.Add(stack.Pop());
        }
        elements.Reverse();

        foreach (var element in elements)
        {
            Console.Write(element + " ");
        }
        Console.WriteLine();
    }
    
    private static void RunSave(string[] instruction)
    {
        var value = stack.Pop();
        memory[instruction[1]] = value;
    }
    
    private static void RunLoad(string[] instruction)
    {
        stack.Push(memory[instruction[1]]);
    }

    private static void RunRead(string[] instruction)
    {
        switch (instruction[1])
        {
            case "I":
                stack.Push(int.Parse(Console.In.ReadLine()!));
                break;
            case "F":
                stack.Push(float.Parse(Console.ReadLine()!));
                break;
            case "B":
                stack.Push(bool.Parse(Console.ReadLine()!));
                break;
            case "S":
                stack.Push(Console.ReadLine()!);
                break;
        }
        
    }

    private static void RunExpression(string[] instruction)
    {
        var single = new List<String> {"uminus", "not", "itof"};
        var right = stack.Pop();

        object left = 0;
        if (!single.Contains(instruction[0], StringComparer.OrdinalIgnoreCase))
        {
            left = stack.Pop();
        }

        switch (instruction[0])
        {
            case "add" when left is int && right is int: stack.Push((int)left + (int)right); break;
            case "add": stack.Push((left is int? (int)left : (float)left) + (right is int ? (int)right : (float)right)); break;
            
            case "sub" when left is int && right is int: stack.Push((int)left - (int)right); break;
            case "sub": stack.Push((left is int? (int)left : (float)left) - (right is int ? (int)right : (float)right)); break;
            
            case "mul" when left is int && right is int: stack.Push((int)left * (int)right); break;
            case "mul": stack.Push((left is int? (int)left : (float)left) * (right is int ? (int)right : (float)right)); break;
            
            case "div" when left is int && right is int: stack.Push((int)left / (int)right); break;
            case "div": stack.Push((left is int? (int)left : (float)left) / (right is int ? (int)right : (float)right)); break;
            
            case "mod": stack.Push((int)left % (int)right); break;
            
            case "uminus" when right is int: stack.Push(-(int)right); break;
            case "uminus": stack.Push(-(float)right); break;
            
            case "concat": stack.Push((string)left + (string)right); break;
            
            case "and": stack.Push((bool)left && (bool)right); break;
            
            case "or": stack.Push((bool)left || (bool)right); break;
            
            case "gt" when left is int && right is int: stack.Push((int)left > (int)right); break;
            case "gt": stack.Push((float)left > (float)right); break;
            
            case "lt" when left is int && right is int: stack.Push((int)left < (int)right); break;
            case "lt": stack.Push((float)left < (float)right); break;
            
            case "eq" when left is int && right is int: stack.Push((int)left == (int)right); break;
            case "eq" when left is string && right is string: stack.Push((string)left == (string)right); break;
            case "eq": stack.Push((float)left == (float)right); break;
            
            case "not": stack.Push(!(bool)right); break;
            
            case "itof": stack.Push(Convert.ToSingle(right)); break;
        }
    }
}
    
namespace AntlrTest6._0;
using NCalc;

public class TypeCheckVisitor : MFLBaseVisitor<Type>
{
    public override Type VisitType(MFLParser.TypeContext context)
    {
        switch (context.GetText())
        {
            case "int": return Type.INT;
            case "float": return Type.FLOAT;
            case "bool": return Type.BOOL;
            case "string": return Type.STRING;
            default:
                ErrorController.AddError(context, "type");
                return Type.ERROR;
        }
    }

    public override Type VisitBool(MFLParser.BoolContext context)
    {
        VirtualMachine.Push(Type.BOOL, context.BOOL().GetText());
        return Type.BOOL;
    }

    public override Type VisitWhile(MFLParser.WhileContext context)
    {
        int l1 = VirtualMachine.NextLabel();
        VirtualMachine.Label();
        if (Visit(context.expr()) != Type.BOOL)
        {
            ErrorController.AddError(context, "bool");
            return Type.ERROR;
        }

        int l2 = VirtualMachine.NextLabel();
        VirtualMachine.Jump(l2, "f");
        Visit(context.statement());
        
        VirtualMachine.Jump(l1);
        VirtualMachine.Label();
        return Type.NULL;
    }

    public override Type VisitIf(MFLParser.IfContext context)
    {
        VirtualMachine.Label();
        if (Visit(context.expr()) != Type.BOOL)
        {
            ErrorController.AddError(context, "bool");
            return Type.ERROR;
        }
        
        // bool result = EvaluateBool(context.expr().GetText());
        int l1 = VirtualMachine.NextLabel();
        VirtualMachine.Jump(l1, "f");
        Visit(context.statement()[0]);

        
        int l2 = VirtualMachine.NextLabel();
        VirtualMachine.Jump(l2 + 1);
        VirtualMachine.Label();
        if (context.statement().Length > 1)
        {
            Visit(context.statement()[1]);
        }
        else
        {
            VirtualMachine.Label();
        }
        
        return Type.NULL;
    }

    public override Type VisitDeclaration(MFLParser.DeclarationContext context)
    {
        Type type = Visit(context.type());
        if (type == Type.ERROR) return Type.ERROR;

        foreach (var id in context.ID())
        {
            VirtualMachine.Push(type);
            VirtualMachine.SaveLoad("save", id.GetText());
        }
        
        return context.ID().Any(element => SymbolTable.Add(element.Symbol, type) == Type.ERROR)
            ? Type.ERROR
            : Type.NULL;
    }

    public override Type VisitUnMinus(MFLParser.UnMinusContext context)
    {
        Type type = Visit(context.expr());

        if (type == Type.INT || type == Type.FLOAT)
        {
            VirtualMachine.code.Add("uminus");
            return type;
        }

        ErrorController.AddError(context, "num");
        return Type.ERROR;
    }

    public override Type VisitNot(MFLParser.NotContext context)
    {
        if (Visit(context.expr()) == Type.BOOL)
        {
            VirtualMachine.AndOrNot("!");
            return Type.BOOL;
        }

        ErrorController.AddError(context, "bool");
        return Type.ERROR;
    }

    public override Type VisitMulDivMod(MFLParser.MulDivModContext context)
    {
        Type left = Visit(context.expr(0));
        Type right = Visit(context.expr(1));
        string op = context.op.Text;
        
        if (op == "%")
        {
            if (left == Type.INT && right == Type.INT)
            {
                VirtualMachine.Expression(op);
                return Type.INT;
            }

            ErrorController.AddError(context);
            return Type.ERROR;
        }

        if (left == Type.INT)
        {
            if (right == Type.INT)
            {
                VirtualMachine.Expression(op, "I");
                return Type.INT;
            }
            if (right == Type.FLOAT)
            {
                string lastInstruction = VirtualMachine.code[^1]; 
                VirtualMachine.code.RemoveAt(VirtualMachine.code.Count - 1);
                VirtualMachine.Itof();
                VirtualMachine.code.Add(lastInstruction);
                VirtualMachine.Expression(op, "F");
                return Type.FLOAT;
            }
        }
        else if (left == Type.FLOAT)
        {
            if (right == Type.INT)
            {
                VirtualMachine.Itof();
                VirtualMachine.Expression(op, "F");
                return Type.FLOAT;
            }

            if (right == Type.FLOAT)
            {
                VirtualMachine.Expression(op, "F");
                return Type.FLOAT;
            }
        }

        VirtualMachine.code.RemoveAt(VirtualMachine.code.Count - 1);
        ErrorController.AddError(context);
        return Type.ERROR;
    }

    public override Type VisitAddSubCon(MFLParser.AddSubConContext context)
    {
        Type left = Visit(context.expr(0));
        Type right = Visit(context.expr(1));
        string op = context.op.Text;

        if (op == ".")
        {
            if (left == Type.STRING && right == Type.STRING)
            {
                VirtualMachine.Expression(op);
                return Type.STRING;
            }

            ErrorController.AddError(context);
            return Type.ERROR;
        }

        if (left == Type.INT)
        {
            if (right == Type.INT)
            {
                VirtualMachine.Expression(op, "I");
                return Type.INT;
            }
            if (right == Type.FLOAT)
            {
                string lastInstruction = VirtualMachine.code[^1]; 
                VirtualMachine.code.RemoveAt(VirtualMachine.code.Count - 1);
                VirtualMachine.Itof();
                VirtualMachine.code.Add(lastInstruction);
                VirtualMachine.Expression(op, "F");
                return Type.FLOAT;
            }
        }
        else if (left == Type.FLOAT)
        {
            if (right == Type.INT)
            {
                VirtualMachine.Itof();
                VirtualMachine.Expression(op, "F");
                return Type.FLOAT;
            }

            if (right == Type.FLOAT)
            {
                VirtualMachine.Expression(op, "F");
                return Type.FLOAT;
            }
        }
        
        ErrorController.AddError(context);
        return Type.ERROR;
    }

    public override Type VisitLessGreater(MFLParser.LessGreaterContext context)
    {
        Type left = Visit(context.expr(0));
        Type right = Visit(context.expr(1));
        
        if (left == Type.INT)
        {
            if (right == Type.INT)
            {
                VirtualMachine.LessGreater(context.op.Text);
                return Type.BOOL;
            }
            if (right == Type.FLOAT)
            {
                VirtualMachine.Itof();
                VirtualMachine.LessGreater(context.op.Text);
                return Type.BOOL;
            }
        }
        else if (left == Type.FLOAT)
        {
            if (right == Type.INT)
            {
                VirtualMachine.Itof();
                VirtualMachine.LessGreater(context.op.Text);
                return Type.BOOL;
            }

            if (right == Type.FLOAT)
            {
                VirtualMachine.LessGreater(context.op.Text);
                return Type.BOOL;
            }
        }

        ErrorController.AddError(context);
        return Type.ERROR;
    }

    public override Type VisitEqualNotEqual(MFLParser.EqualNotEqualContext context)
    {
        Type left = Visit(context.expr(0));
        Type right = Visit(context.expr(1));

        if (left == Type.BOOL || right == Type.BOOL)
        {
            ErrorController.AddError(context);
            return Type.ERROR;
        }

        if (left == right)
        {
            VirtualMachine.Equal(context.op.Text);
            return Type.BOOL;
        }

        if (left == Type.INT && right == Type.FLOAT)
        {
            VirtualMachine.Itof();
            VirtualMachine.Equal(context.op.Text);
            return Type.BOOL;
        }

        if (left == Type.FLOAT && right == Type.INT)
        {
            VirtualMachine.Itof();
            VirtualMachine.Equal(context.op.Text);
            return Type.BOOL;
        }

        ErrorController.AddError(context);
        return Type.ERROR;
    }
    
    public override Type VisitAnd(MFLParser.AndContext context)
    {
        Type left = Visit(context.expr(0));
        Type right = Visit(context.expr(1));

        if (left == Type.BOOL && right == Type.BOOL)
        {
            VirtualMachine.AndOrNot("&&");
            return Type.BOOL;
        }

        ErrorController.AddError(context, "bool");
        return Type.ERROR;
    }

    public override Type VisitOr(MFLParser.OrContext context)
    {
        Type left = Visit(context.expr(0));
        Type right = Visit(context.expr(1));

        if (left == Type.BOOL && right == Type.BOOL)
        {
            VirtualMachine.AndOrNot("||");
            return Type.BOOL;
        }

        ErrorController.AddError(context, "bool");
        return Type.ERROR;
    }

    public override Type VisitAssign(MFLParser.AssignContext context)
    {
        Type left = SymbolTable.Get(context.ID().Symbol);
        Type right = Visit(context.expr());
        
        if (left != Type.ERROR && right != Type.ERROR)
        {
            if (left == Type.FLOAT && right == Type.INT)
            {
                VirtualMachine.Itof();
                VirtualMachine.SaveLoad("save", context.ID().GetText());
                VirtualMachine.SaveLoad("load", context.ID().GetText());
                VirtualMachine.Pop();
                
                return left;
            }

            if (left == right)
            {
                VirtualMachine.SaveLoad("save", context.ID().GetText());
                VirtualMachine.SaveLoad("load", context.ID().GetText());
                VirtualMachine.Pop();

                return left;
            }
        }
        
        ErrorController.AddError(context, "assign");
        return Type.ERROR;
    }

    public override Type VisitId(MFLParser.IdContext context)
    {
        Type type = SymbolTable.Get(context.ID().Symbol);
        if (type == Type.ERROR) ErrorController.AddError(context);
        
        if (type != Type.ERROR)
        {
            VirtualMachine.SaveLoad("load", context.ID().GetText());
        }
        
        return type;
    }

    public override Type VisitInt(MFLParser.IntContext context)
    {
        VirtualMachine.Push(Type.INT, context.INT().GetText());
        return Type.INT;
    }

    public override Type VisitFloat(MFLParser.FloatContext context)
    {
        VirtualMachine.Push(Type.FLOAT, context.FLOAT().GetText());
        return Type.FLOAT;
    }

    public override Type VisitStr(MFLParser.StrContext context)
    {
        VirtualMachine.Push(Type.STRING, context.STRING().GetText());
        return Type.STRING;
    }

    public override Type VisitBrackets(MFLParser.BracketsContext context)
    {
        return Visit(context.expr());
    }

    public override Type VisitWrite(MFLParser.WriteContext context)
    {
        foreach (var expr in context.expr())
        {
            Visit(expr);
        }
        
        VirtualMachine.Print(context.expr().Length);
        return Type.NULL;
    }

    public override Type VisitRead(MFLParser.ReadContext context)
    {
        foreach (var id in context.ID())
        {
            var type = SymbolTable.Get(id.Symbol);
            string typeText = type switch {
                Type.INT => "I",
                Type.FLOAT => "F",
                Type.BOOL => "B",
                Type.STRING => "S",
                _ => "E"
            };
            VirtualMachine.code.Add($"read {typeText}");
            
            if (type != Type.ERROR)
            {
                VirtualMachine.SaveLoad("save", id.GetText());
            }
        }
        
        return Type.NULL;
    }
    
    private bool EvaluateBool(string expression)
    {
        Expression e = new Expression(expression);
        return (bool)e.Evaluate();
    } 
}
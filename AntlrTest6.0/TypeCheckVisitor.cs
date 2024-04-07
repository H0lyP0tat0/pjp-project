namespace AntlrTest6._0;

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
        return Type.BOOL;
    }

    public override Type VisitWhile(MFLParser.WhileContext context)
    {
        if (Visit(context.expr()) != Type.BOOL)
        {
            ErrorController.AddError(context, "bool");
            return Type.ERROR;
        }

        Visit(context.statement());
        return Type.NULL;
    }

    public override Type VisitIf(MFLParser.IfContext context)
    {
        if (Visit(context.expr()) != Type.BOOL)
        {
            ErrorController.AddError(context, "bool");
            return Type.ERROR;
        }

        Visit(context.statement()[0]);

        if (context.statement().Length > 1)
        {
            Visit(context.statement()[1]);
        }

        return Type.NULL;
    }

    public override Type VisitDeclaration(MFLParser.DeclarationContext context)
    {
        Type type = Visit(context.type());
        if (type == Type.ERROR) return Type.ERROR;

        return context.ID().Any(element => SymbolTable.Add(element.Symbol, type) == Type.ERROR)
            ? Type.ERROR
            : Type.NULL;
    }

    public override Type VisitUnMinus(MFLParser.UnMinusContext context)
    {
        Type type = Visit(context.expr());

        if (type == Type.INT || type == Type.FLOAT) return type;

        ErrorController.AddError(context, "num");
        return Type.ERROR;
    }

    public override Type VisitNot(MFLParser.NotContext context)
    {
        if (Visit(context.expr()) == Type.BOOL) return Type.BOOL;

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
            if (left == Type.INT && right == Type.INT) return Type.INT;

            ErrorController.AddError(context);
            return Type.ERROR;
        }
        else
        {
            if (left == Type.INT)
            {
                if (right == Type.INT) return Type.INT;
                if (right == Type.FLOAT) return Type.FLOAT;
            }
            else if (left == Type.FLOAT)
            {
                if (right == Type.INT) return Type.FLOAT;
                if (right == Type.FLOAT) return Type.FLOAT;
            }
        }

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
            if (left == Type.STRING && right == Type.STRING) return Type.STRING;

            ErrorController.AddError(context);
            return Type.ERROR;
        }
        else
        {
            if (left == Type.INT)
            {
                if (right == Type.INT) return Type.INT;
                if (right == Type.FLOAT) return Type.FLOAT;
            }
            else if (left == Type.FLOAT)
            {
                if (right == Type.INT) return Type.FLOAT;
                if (right == Type.FLOAT) return Type.FLOAT;
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
            if (right == Type.INT) return Type.BOOL;
            if (right == Type.FLOAT) return Type.BOOL;
        }
        else if (left == Type.FLOAT)
        {
            if (right == Type.INT) return Type.BOOL;
            if (right == Type.FLOAT) return Type.BOOL;
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

        if (left == right) return Type.BOOL;

        if (left == Type.INT && right == Type.FLOAT) return Type.BOOL;
        if (left == Type.FLOAT && right == Type.INT) return Type.BOOL;

        ErrorController.AddError(context);
        return Type.ERROR;
    }

    public override Type VisitAnd(MFLParser.AndContext context)
    {
        Type left = Visit(context.expr(0));
        Type right = Visit(context.expr(1));

        if (left == Type.BOOL && right == Type.BOOL) return Type.BOOL;

        ErrorController.AddError(context, "bool");
        return Type.ERROR;
    }

    public override Type VisitOr(MFLParser.OrContext context)
    {
        Type left = Visit(context.expr(0));
        Type right = Visit(context.expr(1));

        if (left == Type.BOOL && right == Type.BOOL) return Type.BOOL;

        ErrorController.AddError(context, "bool");
        return Type.ERROR;
    }

    public override Type VisitAssign(MFLParser.AssignContext context)
    {
        Type left = SymbolTable.Get(context.ID().Symbol);
        Type right = Visit(context.expr());

        if (left != Type.ERROR && right != Type.ERROR)
        {
            if (left == Type.FLOAT && right == Type.INT) return left;
            if (left == right) return left;
        }
        
        ErrorController.AddError(context, "assign");
        return Type.ERROR;
    }

    public override Type VisitId(MFLParser.IdContext context)
    {
        Type type = SymbolTable.Get(context.ID().Symbol);
        if (type == Type.ERROR) ErrorController.AddError(context);
        
        return type;
    }

    public override Type VisitInt(MFLParser.IntContext context)
    {
        return Type.INT;
    }

    public override Type VisitFloat(MFLParser.FloatContext context)
    {
        return Type.FLOAT;
    }

    public override Type VisitStr(MFLParser.StrContext context)
    {
        return Type.STRING;
    }

    public override Type VisitBrackets(MFLParser.BracketsContext context)
    {
        return Visit(context.expr());
    }
}
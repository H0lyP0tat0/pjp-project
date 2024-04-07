namespace AntlrTest6._0;

public class TypeCheckVisitor : MFLBaseVisitor<Type>
{
    public override Type VisitType(MFLParser.TypeContext context)
    {
        return context.GetText() switch
        {
            "int" => Type.INT,
            "float" => Type.FLOAT,
            "bool" => Type.BOOL,
            "string" => Type.STRING,
            _ => Type.ERROR
        };
    }
}
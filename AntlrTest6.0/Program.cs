﻿using System.Globalization;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;

namespace AntlrTest6._0
{
    public class Program
    {

        public static void Main(string[] args)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            var fileName = "input1.txt";
            Console.WriteLine("Parsing: " + fileName);
            var inputFile = new StreamReader(fileName);
            AntlrInputStream input = new AntlrInputStream(inputFile);
            MFLLexer lexer = new MFLLexer(input);
            CommonTokenStream tokens = new CommonTokenStream(lexer);
            MFLParser parser = new MFLParser(tokens);

            parser.AddErrorListener(new VerboseListener());

            IParseTree tree = parser.prog();

            if (parser.NumberOfSyntaxErrors != 0) return;
            // Console.WriteLine(tree.ToStringTree(parser));
            // ParseTreeWalker walker = new ParseTreeWalker();
            // walker.Walk(new EvalListener(), tree);

            // new TypeCheckVisitor().Visit(tree);
            // ErrorController.PrintErrors();
            // SymbolTable.PrintTable();
            // VirtualMachine.PrintInstructions();
            VirtualMachine.Run("ins3.txt");
        }
    }
}
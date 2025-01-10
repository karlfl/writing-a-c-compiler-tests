
namespace myc
{
    public static class SEM_Resolve
    {
        internal static Dictionary<string, string> variableMap = [];

        public static AST_Program Resolve(AST_Program program)
        {
            return new AST_Program(Resolve_Function(program.Function));
        }

        public static AST_Function Resolve_Function(AST_Function function)
        {
            List<AST_BlockItem> newbody = [];
            foreach (AST_BlockItem item in function.Body)
            {
                newbody.Add(Resolve_BlockItem(item));
            }

            return new AST_Function(function.Identifier, newbody);
        }

        private static AST_BlockItem Resolve_BlockItem(AST_BlockItem item)
        {
            return item switch
            {
                AST_BlockStatement stmt => new AST_BlockStatement(Resolve_Statement(stmt.Statement)),
                AST_BlockDeclaration dclr => new AST_BlockDeclaration(Resolve_Declaration(dclr.Declaration)),
                _ => throw new Exception("Resolve: Unexpected block item"),
            };
        }

        private static AST_Declaration Resolve_Declaration(AST_Declaration declaration)
        {
            if (variableMap.ContainsKey(declaration.Name))
            {
                throw new Exception("Duplicate variable declaration");
            }

            // Generate a new unique variable name
            string uniqueName = Utilities.GenerateUniqueLabel(declaration.Name);
            variableMap.Add(declaration.Name, uniqueName);

            // Resolve initializer if there is one
            AST_Factor? init = (declaration.Init != null) ? Resolve_Expression(declaration.Init) : declaration.Init;

            // Return new map and resolved declaration
            return new(uniqueName, init);
        }

        private static AST_Statement Resolve_Statement(AST_Statement statement)
        {
            switch (statement)
            {
                case AST_Return aReturn:
                    return new AST_Return(Resolve_Expression(aReturn.Expression));
                case AST_Expression aExpr:
                    return new AST_Expression(Resolve_Expression(aExpr.Expression));
                default:
                    return statement;
                    // throw new Exception("Resolve: Unexpected statement");
            }
        }

        private static AST_Factor Resolve_Expression(AST_Factor factor)
        {
            switch (factor)
            {
                case AST_Assignment asmt:
                    return Resolve_Assignment(asmt);
                case AST_Var var:
                    bool definedVar = variableMap.TryGetValue(var.Identifier.Name, out string? uniqueName);
                    if (!definedVar || uniqueName == null)
                    {
                        throw new Exception(
                            string.Format("Resolve: Undefined Variable: {0}", var.Identifier.Name)
                        );
                    }
                    else
                    {
                        return new AST_Var(new AST_Identifier(uniqueName));
                    }
                case AST_Unary unary:
                    return new AST_Unary(unary.UnaryOp, Resolve_Expression(unary.Factor));
                case AST_Binary binary:
                    return new AST_Binary(
                        Resolve_Expression(binary.LeftFactor),
                        binary.BinaryOp,
                        Resolve_Expression(binary.RightFactor)
                    );
                case AST_Int constant:
                    return constant;
                default:
                    throw new Exception(string.Format("Resolve: unknown expression type found: {0}", factor?.GetType()));

            }
        }

        private static AST_Assignment Resolve_Assignment(AST_Assignment factor)
        {
            // validate left is a lvalue
            switch (factor.LeftFactor)
            {
                case AST_Var:
                    break;
                default:
                    throw new Exception(string.Format(
                        "Resolve: Expected expression on left-hand side of assignment statement, found {0}",
                        factor.LeftFactor.GetType()
                    ));
            }
            return new(Resolve_Expression(factor.LeftFactor), Resolve_Expression(factor.RightFactor));
        }
    }

}
namespace myc
{
    public static class SEM_Resolve
    {
        public static AST_Program Resolve(AST_Program program)
        {
            VariableMap variableMap = new(); //initial variable map for program scope
            return new AST_Program(Resolve_Function(program.Function, variableMap));
        }

        public static AST_Function Resolve_Function(AST_Function function, VariableMap varMap)
        {
            VariableMap newMap = varMap.MakeCopy();   // functions require their own variable map since this is a new scope
            return new AST_Function(function.Identifier, Resolve_Block(function.Body, newMap));
        }

        private static AST_Block Resolve_Block(AST_Block block, VariableMap varMap)
        {
            List<AST_BlockItem> newItems = [];
            foreach (AST_BlockItem item in block.Items)
            {
                newItems.Add(Resolve_BlockItem(item, varMap));
            }
            return new(newItems);
        }

        private static AST_BlockItem Resolve_BlockItem(AST_BlockItem item, VariableMap varMap)
        {
            return item switch
            {
                AST_BlockStatement stmt => new AST_BlockStatement(Resolve_Statement(stmt.Statement, varMap)),
                AST_BlockDeclaration dclr => new AST_BlockDeclaration(Resolve_Declaration(dclr.Declaration, varMap)),
                _ => throw new Exception("Resolve: Unexpected block item"),
            };
        }

        private static AST_Declaration Resolve_Declaration(AST_Declaration declaration, VariableMap varMap)
        {
            if (varMap.InCurrentScope(declaration.Name))
            {
                throw new Exception("Resolve: Duplicate variable declaration");
            }

            // Generate a new unique variable name
            string uniqueName = Utilities.GenerateUniqueLabel(declaration.Name);
            // add it to the map and set the current scope = true
            varMap.Add(declaration.Name, uniqueName, true);

            // Resolve initializer if there is one
            AST_Factor? init = (declaration.Init != null) ? Resolve_Expression(declaration.Init, varMap) : declaration.Init;

            // Return new resolved declaration
            return new(uniqueName, init);
        }

        private static AST_Statement Resolve_Statement(AST_Statement statement, VariableMap varMap)
        {
            switch (statement)
            {
                case AST_Return aReturn:
                    return new AST_Return(Resolve_Expression(aReturn.Expression, varMap));
                case AST_Expression aExpr:
                    return new AST_Expression(Resolve_Expression(aExpr.Expression, varMap));
                case AST_Compound compound:
                    // New code block requires a new variable map since this is a new scope
                    VariableMap newMap = varMap.MakeCopy();
                    return new AST_Compound(Resolve_Block(compound.Block, newMap));
                case AST_If aIf:
                    AST_Statement? elseStmt = aIf.ElseStatement != null ? Resolve_Statement(aIf.ElseStatement, varMap) : null;
                    return new AST_If(
                        Resolve_Expression(aIf.Condition, varMap),
                        Resolve_Statement(aIf.ThenStatement, varMap),
                        elseStmt);
                default:
                    return statement;
                    // throw new Exception("Resolve: Unexpected statement");
            }
        }

        private static AST_Factor Resolve_Expression(AST_Factor factor, VariableMap varMap)
        {
            switch (factor)
            {
                case AST_Assignment asmt:
                    return Resolve_Assignment(asmt, varMap);
                case AST_Var var:
                    if (varMap.Exists(var.Identifier.Name))
                    {
                        string newVarName = varMap.GetUniqueName(var.Identifier.Name);
                        return new AST_Var(new AST_Identifier(newVarName));
                    }
                    else
                    {
                        throw new Exception(
                            string.Format("Resolve: Undefined Variable: {0}", var.Identifier.Name)
                        );
                    }
                case AST_Unary unary:
                    return new AST_Unary(unary.UnaryOp, Resolve_Expression(unary.Factor, varMap));
                case AST_Binary binary:
                    return new AST_Binary(
                        Resolve_Expression(binary.LeftFactor, varMap),
                        binary.BinaryOp,
                        Resolve_Expression(binary.RightFactor, varMap)
                    );
                case AST_Conditional cond:
                    return new AST_Conditional(
                        Resolve_Expression(cond.Condition, varMap),
                        Resolve_Expression(cond.ThenClause, varMap),
                        Resolve_Expression(cond.ElseClause, varMap)
                    );
                case AST_Int constant:
                    return constant;
                default:
                    throw new Exception(string.Format("Resolve: unknown expression type found: {0}", factor?.GetType()));

            }
        }

        private static AST_Assignment Resolve_Assignment(AST_Assignment factor, VariableMap varMap)
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
            return new(Resolve_Expression(factor.LeftFactor, varMap), Resolve_Expression(factor.RightFactor, varMap));
        }
    }

}
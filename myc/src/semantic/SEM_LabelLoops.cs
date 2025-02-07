namespace myc
{
    public static class SEM_LabelLoops
    {
        public static AST_Program Label(AST_Program program)
        {
            return new AST_Program(Label_Function(program.Function));
        }

        private static AST_Function Label_Function(AST_Function function)
        {
            return new AST_Function(function.Identifier, Label_Block(function.Body, ""));
        }

        private static AST_Block Label_Block(AST_Block body, string loopName)
        {
            List<AST_BlockItem> newItems = [];
            foreach (AST_BlockItem item in body.Items)
            {
                newItems.Add(Label_BlockItem(item, loopName));
            }
            return new(newItems);
        }

        private static AST_BlockItem Label_BlockItem(AST_BlockItem item, string loopName)
        {
            return item switch
            {
                AST_BlockStatement statement => new AST_BlockStatement(
                    Label_Statement(statement.Statement, loopName)
                    ),
                _ => item,
            };
        }

        private static AST_Statement Label_Statement(AST_Statement statement, string loopName)
        {
            switch (statement)
            {
                case AST_Break:
                    return loopName != "" ?
                        new AST_Break(loopName) :
                        throw new Exception("Label: Break outside of loop");
                case AST_Continue:
                    return loopName != "" ?
                        new AST_Continue(loopName) :
                        throw new Exception("Label: Continue outside of loop");
                case AST_While aWhile:
                    string whileName = Utilities.GenerateUniqueLabel("while");
                    return new AST_While(
                        aWhile.Condition,
                        Label_Statement(aWhile.Body, whileName),
                        whileName);
                case AST_DoWhile aDo:
                    string doName = Utilities.GenerateUniqueLabel("doWhile");
                    return new AST_DoWhile(
                        Label_Statement(aDo.Body, doName),
                        aDo.Condition,
                        doName);
                case AST_For aFor:
                    string forName = Utilities.GenerateUniqueLabel("for");
                    return new AST_For(
                        aFor.Initialization,
                        aFor.Condition,
                        aFor.Post,
                        Label_Statement(aFor.Body, forName),
                        forName);
                case AST_Compound aComp:
                    return new AST_Compound(
                        Label_Block(aComp.Block, loopName)
                    );
                case AST_If aIf:
                    return new AST_If(
                        aIf.Condition,
                        Label_Statement(aIf.ThenStatement, loopName),
                        aIf.ElseStatement == null ?
                            aIf.ElseStatement :
                            Label_Statement(aIf.ElseStatement, loopName)
                    );
                default:
                    return statement;
            }
        }
    }
}
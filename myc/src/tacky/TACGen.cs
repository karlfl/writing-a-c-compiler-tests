using System.Collections;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace myc
{
    public static class TACGen
    {

        public static TAC_Program Generate(AST_Program program)
        {
            return new(EmitFunction(program.Function));
        }

        static TAC_Function EmitFunction(AST_Function function)
        {
            List<TAC_Instruction> instructions = EmitForStatements(function.Body.Items);
            //Extra return;
            instructions.Add(new TAC_Return(new TAC_Constant(0)));

            return new(function.Identifier.Name, instructions);
        }

        static List<TAC_Instruction> EmitForStatements(List<AST_BlockItem> blockItems)
        {
            List<TAC_Instruction> instructions = [];

            foreach (AST_BlockItem blockItem in blockItems)
            {
                instructions.AddRange(EmitForBlockItem(blockItem));
            }

            return instructions;
        }

        private static List<TAC_Instruction> EmitForBlockItem(AST_BlockItem blockItem)
        {
            List<TAC_Instruction> instructions = [];

            switch (blockItem)
            {
                case AST_BlockStatement statement:
                    var stmtInstr = EmitForStatement(statement.Statement);
                    instructions.AddRange(stmtInstr);
                    break;
                case AST_BlockDeclaration declaration:
                    instructions.AddRange(EmitForDeclaration(declaration.Declaration));
                    break;
                default:
                    throw new ArgumentException("TAC: Unexpected AST Block Item type");
            }

            return instructions;
        }

        private static List<TAC_Instruction> EmitForDeclaration(AST_Declaration declaration)
        {
            List<TAC_Instruction> instructions = [];

            switch (declaration.Init)
            {
                case AST_Factor init:
                    // treat declaration with initializer like an assignment expression
                    AST_Assignment asmt = new(new AST_Var(new(declaration.Name)), init);
                    var (instr, _) = EmitForExpression(asmt);
                    instructions.AddRange(instr);
                    break;
                default:
                    // don't generate instructions for declaration without initializer
                    break;
            }

            return instructions;
        }

        private static List<TAC_Instruction> EmitForStatement(AST_Statement statement)
        {
            List<TAC_Instruction> instructions = [];

            switch (statement)
            {
                case AST_Return aReturn:
                    var (instr, value) = EmitForExpression(aReturn.Expression);
                    instructions.AddRange(instr);
                    instructions.Add(new TAC_Return(value));
                    break;
                case AST_Expression aExp:
                    // evaluate expression but don't use result.
                    var (expInstr, _) = EmitForExpression(aExp.Expression);
                    instructions.AddRange(expInstr);
                    break;
                case AST_If aIf:
                    instructions.AddRange(EmitForIfStatement(aIf));
                    break;
                case AST_Compound aComp:
                    instructions.AddRange(EmitForStatements(aComp.Block.Items));
                    break;
                case AST_Break aBreak:
                    instructions.Add(new TAC_Jump(EmitBreakLabel(aBreak.Identifier)));
                    break;
                case AST_Continue aCont:
                    instructions.Add(new TAC_Jump(EmitContinueLabel(aCont.Identifier)));
                    break;
                case AST_DoWhile aDo:
                    instructions.AddRange(EmitDoLoop(aDo));
                    break;
                case AST_While aWhile:
                    instructions.AddRange(EmitWhileLoop(aWhile));
                    break;
                case AST_For aFor:
                    instructions.AddRange(EmitForLoop(aFor));
                    break;
                case AST_Null:
                    break;
                default:
                    throw new ArgumentException(string.Format("TAC: Unexpected AST Expression type: {0}", statement.GetType()));
            }

            return instructions;
        }

        private static List<TAC_Instruction> EmitDoLoop(AST_DoWhile doWhile)
        {
            List<TAC_Instruction> instructions = [];

            TAC_Label startLabel = new(Utilities.GenerateUniqueLabel("do_loop_start"));
            TAC_Label breakLabel = EmitBreakLabel(doWhile.Identifier);
            TAC_Label continueLabel = EmitContinueLabel(doWhile.Identifier);
            var (condInstr, value) = EmitForExpression(doWhile.Condition);
            instructions.Add(startLabel);
            instructions.AddRange(EmitForStatement(doWhile.Body));
            instructions.Add(continueLabel);
            instructions.AddRange(condInstr);
            instructions.Add(new TAC_JumpNotZero(value, startLabel));
            instructions.Add(breakLabel);

            return instructions;
        }

        private static List<TAC_Instruction> EmitWhileLoop(AST_While aWhile)
        {
            List<TAC_Instruction> instructions = [];

            TAC_Label continueLabel = EmitContinueLabel(aWhile.Identifier);
            TAC_Label breakLabel = EmitBreakLabel(aWhile.Identifier);
            var (condInstr, value) = EmitForExpression(aWhile.Condition);
            instructions.Add(continueLabel);
            instructions.AddRange(condInstr);
            instructions.Add(new TAC_JumpIfZero(value, breakLabel));
            instructions.AddRange(EmitForStatement(aWhile.Body));
            instructions.Add(new TAC_Jump(continueLabel));
            instructions.Add(breakLabel);

            return instructions;
        }

        private static List<TAC_Instruction> EmitForLoop(AST_For aFor)
        {
            List<TAC_Instruction> instructions = [];

            TAC_Label startLabel = new(Utilities.GenerateUniqueLabel("for_loop_start"));
            TAC_Label continueLabel = EmitContinueLabel(aFor.Identifier);
            TAC_Label breakLabel = EmitBreakLabel(aFor.Identifier);
            //Loop Initialization
            switch (aFor.Initialization)
            {
                case AST_InitDecl decl:
                    instructions.AddRange(EmitForDeclaration(decl.Declaration));
                    break;
                case AST_InitExp expr:
                    instructions.AddRange(EmitForExpressionOptional(expr.Expression));
                    break;
                default:
                    break;
            }
            instructions.Add(startLabel);
            // Test Condition
            if(aFor.Condition != null) {
                var (instr, value) = EmitForExpression(aFor.Condition);
                instructions.AddRange(instr);
                instructions.Add(new TAC_JumpIfZero(value, breakLabel));
            }
            instructions.AddRange(EmitForStatement(aFor.Body));
            // Looping
            instructions.Add(continueLabel);
            instructions.AddRange(EmitForExpressionOptional(aFor.Post));
            instructions.Add(new TAC_Jump(startLabel));
            // End
            instructions.Add(breakLabel);

            return instructions;
        }

        private static List<TAC_Instruction> EmitForIfStatement(AST_If ifStatement)
        {
            List<TAC_Instruction> instr = [];

            var (condInstr, condResult) = EmitForExpression(ifStatement.Condition);
            var thenInstr = EmitForStatement(ifStatement.ThenStatement);
            TAC_Label endLabel = new(Utilities.GenerateUniqueLabel("if_end"));

            if (ifStatement.ElseStatement == null)
            {
                // null else statement
                instr.AddRange(condInstr);
                instr.Add(new TAC_JumpIfZero(condResult, endLabel));
                instr.AddRange(thenInstr);
                instr.Add(endLabel);
            }
            else
            {
                //full if/else statement
                var elseInstr = EmitForStatement(ifStatement.ElseStatement);
                TAC_Label elseLabel = new(Utilities.GenerateUniqueLabel("else"));

                instr.AddRange(condInstr);
                instr.Add(new TAC_JumpIfZero(condResult, elseLabel));
                instr.AddRange(thenInstr);
                instr.Add(new TAC_Jump(endLabel));
                instr.Add(elseLabel);
                instr.AddRange(elseInstr);
                instr.Add(endLabel);
            }

            return instr;
        }

        private static List<TAC_Instruction> EmitForExpressionOptional(AST_Factor? expression){
            if (expression != null){
                var(instr, _) = EmitForExpression(expression);
                return instr;
            } else {
                return [];
            }

        }
        private static (List<TAC_Instruction> instructions, TAC_Value value) EmitForExpression(AST_Factor expression)
        {
            switch (expression)
            {
                case AST_Int constant:
                    return ([], new TAC_Constant(constant.Value));
                case AST_Unary unary:
                    return EmitUnary(unary);
                case AST_Binary binary:
                    return EmitBinary(binary);
                case AST_Var var:
                    return ([], new TAC_Variable(var.Identifier.Name));
                case AST_Assignment asmt:
                    return EmitAssignment(asmt);
                case AST_Conditional cond:
                    return EmitForCondAssignment(cond);
                default:
                    throw new ArgumentException(string.Format("TAC: Unexpected AST Expression type: {0}", expression.GetType()));
            }

        }

        private static (List<TAC_Instruction> instructions, TAC_Value value) EmitUnary(AST_Unary unary)
        {
            var (innerUnaryInstr, unarySrc) = EmitForExpression(unary.Factor);
            TAC_Variable dst = new(Utilities.GenerateUniqueId());
            TAC_UnaryOp unaryOp = ConvertUnaryOp(unary.UnaryOp);
            innerUnaryInstr.Add(new TAC_Unary(unaryOp, unarySrc, dst));
            return (innerUnaryInstr, dst);
        }

        private static TAC_UnaryOp ConvertUnaryOp(AST_UnaryOp unaryOp)
        {
            return unaryOp switch
            {
                AST_Complement => new TAC_Complement(),
                AST_Negate => new TAC_Negate(),
                AST_Not => new TAC_Not(),
                _ => throw new ArgumentException("Unexpected AST Unary Operation"),
            };
        }

        private static (List<TAC_Instruction>, TAC_Value) EmitBinary(AST_Binary binary)
        {
            return binary.BinaryOp switch
            {
                AST_LogicalAnd => EmitLogicalAnd(binary),
                AST_LogicalOr => EmitLogicalOr(binary),
                _ => EmitBinaryOp(binary),
            };
        }

        private static (List<TAC_Instruction>, TAC_Value) EmitBinaryOp(AST_Binary binary)
        {
            List<TAC_Instruction> instr = [];

            var (leftInstr, leftSrc) = EmitForExpression(binary.LeftFactor);
            var (rightInstr, rightSrc) = EmitForExpression(binary.RightFactor);
            TAC_Variable dstName = new(Utilities.GenerateUniqueId());
            TAC_BinaryOp binaryOp = ConvertBinaryOp(binary.BinaryOp);

            instr.AddRange(leftInstr);
            instr.AddRange(rightInstr);
            instr.Add(new TAC_Binary(binaryOp, leftSrc, rightSrc, dstName));

            return (instr, dstName);
        }

        private static (List<TAC_Instruction>, TAC_Variable) EmitLogicalAnd(AST_Binary binaryAnd)
        {
            List<TAC_Instruction> instr = [];

            var (leftCond, valLeft) = EmitForExpression(binaryAnd.LeftFactor);
            var (rightCond, valRight) = EmitForExpression(binaryAnd.RightFactor);
            TAC_Label falseLabel = new(Utilities.GenerateUniqueLabel("and_false"));
            TAC_Label endLabel = new(Utilities.GenerateUniqueLabel("and_end"));
            TAC_Variable dst = new(Utilities.GenerateUniqueId());

            instr.AddRange(leftCond);
            instr.Add(new TAC_JumpIfZero(valLeft, falseLabel));
            instr.AddRange(rightCond);
            instr.Add(new TAC_JumpIfZero(valRight, falseLabel));
            instr.Add(new TAC_Copy(new TAC_Constant(1), dst));
            instr.Add(new TAC_Jump(endLabel));
            instr.Add(falseLabel);
            instr.Add(new TAC_Copy(new TAC_Constant(0), dst));
            instr.Add(endLabel);


            return (instr, dst);
        }

        private static (List<TAC_Instruction>, TAC_Variable) EmitLogicalOr(AST_Binary binaryOr)
        {
            List<TAC_Instruction> instr = [];

            var (leftCond, valLeft) = EmitForExpression(binaryOr.LeftFactor);
            var (rightCond, valRight) = EmitForExpression(binaryOr.RightFactor);
            TAC_Label trueLabel = new(Utilities.GenerateUniqueLabel("or_true"));
            TAC_Label endLabel = new(Utilities.GenerateUniqueLabel("or_end"));
            TAC_Variable dst = new(Utilities.GenerateUniqueId());

            instr.AddRange(leftCond);
            instr.Add(new TAC_JumpNotZero(valLeft, trueLabel));
            instr.AddRange(rightCond);
            instr.Add(new TAC_JumpNotZero(valRight, trueLabel));
            instr.Add(new TAC_Copy(new TAC_Constant(0), dst));
            instr.Add(new TAC_Jump(endLabel));
            instr.Add(trueLabel);
            instr.Add(new TAC_Copy(new TAC_Constant(1), dst));
            instr.Add(endLabel);

            return (instr, dst);
        }
        private static TAC_BinaryOp ConvertBinaryOp(AST_BinaryOp binaryOp)
        {
            return binaryOp switch
            {
                AST_Add => new TAC_Add(),
                AST_Subtract => new TAC_Subtract(),
                AST_Multiply => new TAC_Multiply(),
                AST_Divide => new TAC_Divide(),
                AST_Mod => new TAC_Remainder(),
                AST_AND => new TAC_AND(),
                AST_OR => new TAC_OR(),
                AST_XOR => new TAC_XOR(),
                AST_LeftShift => new TAC_LeftShift(),
                AST_RightShift => new TAC_RightShift(),
                AST_Equal => new TAC_Equal(),
                AST_NotEqual => new TAC_NotEqual(),
                AST_LessThan => new TAC_LessThan(),
                AST_LessOrEqual => new TAC_LessOrEqual(),
                AST_GreaterThan => new TAC_GreaterThan(),
                AST_GreaterOrEqual => new TAC_GreaterOrEqual(),
                _ => throw new ArgumentException("Unexpected AST Binary Operation"),
            };
        }

        private static (List<TAC_Instruction> instructions, TAC_Value value) EmitAssignment(AST_Assignment asmt)
        {
            switch (asmt.LeftFactor)
            {
                case AST_Var var:
                    var (instr, rightResult) = EmitForExpression(asmt.RightFactor);
                    TAC_Variable dst = new(var.Identifier.Name);
                    instr.Add(new TAC_Copy(rightResult, dst));
                    return (instr, dst);
                default:
                    throw new Exception(string.Format("TAC: Invalid Assignment Left Factor: {0}", asmt.LeftFactor.GetType()));
            }
        }

        private static (List<TAC_Instruction> instructions, TAC_Value value) EmitForCondAssignment(AST_Conditional cond)
        {
            List<TAC_Instruction> instr = [];

            var (condInstr, condResult) = EmitForExpression(cond.Condition);
            var (thenInstr, thenResult) = EmitForExpression(cond.ThenClause);
            var (elseInstr, elseResult) = EmitForExpression(cond.ElseClause);
            TAC_Label elseLabel = new(Utilities.GenerateUniqueLabel("cond_else"));
            TAC_Label endLabel = new(Utilities.GenerateUniqueLabel("cond_end"));
            TAC_Variable dst = new(Utilities.GenerateUniqueId());

            instr.AddRange(condInstr);
            instr.Add(new TAC_JumpIfZero(condResult, elseLabel));
            instr.AddRange(thenInstr);
            instr.Add(new TAC_Copy(thenResult, dst));
            instr.Add(new TAC_Jump(endLabel));
            instr.Add(elseLabel);
            instr.AddRange(elseInstr);
            instr.Add(new TAC_Copy(elseResult, dst));
            instr.Add(endLabel);

            return (instr, dst);
        }

        private static TAC_Label EmitContinueLabel(string id)
        {
            return new("continue." + id);
        }
        private static TAC_Label EmitBreakLabel(string id)
        {
            return new("break." + id);
        }
    }
}
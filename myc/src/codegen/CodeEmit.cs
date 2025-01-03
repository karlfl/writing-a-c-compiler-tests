namespace myc
{
    public static class CodeEmit
    {
        public static void Emit(ASM_Program program, string fileName)
        {
            // Create a StreamWriter to write to the ASM file
            string asm_filename = fileName.Split(".")[0] + ".s";
            using StreamWriter writer = new StreamWriter(asm_filename);
            EmitFunction(writer, program.Function);
            EmitStackNote(writer);
        }

        public static void EmitFunction(StreamWriter writer, ASM_Function function)
        {
            string label = "" + function.Name; //Linux
            writer.WriteLine("\t.globl {0}", label);
            writer.WriteLine("{0}:", label);
            writer.WriteLine("\tpushq \t%rbp");
            writer.WriteLine("\tmovq  \t%rsp, %rbp");

            foreach (ASM_Instruction instr in function.Instructions)
            {
                EmitInstruction(writer, instr);
            }
        }

        private static void EmitInstruction(StreamWriter writer, ASM_Instruction instr)
        {
            switch (instr)
            {
                case ASM_Mov inst:
                    string src = ConvertOperand(inst.Source);
                    string dst = ConvertOperand(inst.Destination);
                    writer.WriteLine("\tmovl \t{0}, {1}", src, dst);
                    break;
                case ASM_Ret inst:
                    writer.WriteLine("\tmovq \t%rbp, %rsp");
                    writer.WriteLine("\tpopq \t%rbp");
                    writer.WriteLine("\tret");
                    break;
                case ASM_Unary unary:
                    string unaryOp = ConvertUnaryOp(unary.UnaryOp);
                    string operand = ConvertOperand(unary.Operand);
                    writer.WriteLine("\t{0} \t{1}", unaryOp, operand);
                    break;
                case ASM_Binary binary:
                    string binaryOp = ConvertBinaryOp(binary.BinaryOp);
                    string binOper1 = ConvertOperand(binary.Operand1);
                    string binOper2 = ConvertOperand(binary.Operand2);
                    writer.WriteLine("\t{0} \t{1}, {2}", binaryOp, binOper1, binOper2);
                    break;
                case ASM_Idiv iDiv:
                    string divOperand = ConvertOperand(iDiv.Operand);
                    writer.WriteLine("\tidivl \t{0}", divOperand);
                    break;
                case ASM_Cdq:
                    writer.WriteLine("\tcdq");
                    break;
                case ASM_AllocateStack stack:
                    writer.WriteLine("\tsubq \t${0}, %rsp", stack.Size);
                    break;
                default:
                    return;
            }
            return;
        }

        private static string ConvertOperand(ASM_Operand operand)
        {
            return operand switch
            {
                ASM_Register register => register.Register switch
                {
                    ASM_AX => "%eax",
                    ASM_DX => "%edx",
                    ASM_R10 => "%r10d",
                    ASM_R11 => "%r11d",
                    _ => throw new InvalidOperationException("Register Not Defined for Conversion: {0}"),
                },
                ASM_Stack stack => string.Format("{0}(%rbp)", stack.Stack),
                ASM_Imm imm => string.Format("${0}", imm.Value),
                _ => throw new InvalidOperationException("Operand Not Defined for Conversion: {0}"),
            };
        }

        private static string ConvertUnaryOp(ASM_UnaryOp operand)
        {
            return operand switch
            {
                ASM_UnaryNeg => "negl",
                ASM_UnaryNot => "notl",
                _ => throw new InvalidOperationException("Unary Operand Not Defined for Conversion: {0}"),
            };
        }

        private static string ConvertBinaryOp(ASM_BinaryOp operand)
        {
            return operand switch
            {
                ASM_BinaryAdd => "addl",
                ASM_BinarySub => "subl",
                ASM_BinaryMult => "imull",
                _ => throw new InvalidOperationException("Unary Operand Not Defined for Conversion: {0}"),
            };
        }

        private static void EmitStackNote(StreamWriter writer)
        {
            writer.WriteLine("\t.section .note.GNU-stack,\"\",@progbits");
        }
    }
}
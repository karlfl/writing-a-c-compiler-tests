using System.Collections;
using System.Reflection.Metadata.Ecma335;

namespace myc
{
    public static class CodeEmit
    {
        public static void Emit(ASM_Program program, string fileName)
        {
            // Create a StreamWriter to write to the ASM file
            string asm_filename = fileName.Split(".")[0] + ".s";
            using StreamWriter writer = new(asm_filename);
            EmitFunction(writer, program.Function);
            EmitStackNote(writer);

            Console.WriteLine("Emit Complete {0}",asm_filename);
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
                    switch (binary.BinaryOp) {
                        case ASM_BitwiseLShift:
                        case ASM_BitwiseRShift:
                            string shiftOp = ConvertBinaryOp(binary.BinaryOp);
                            string shiftOper1 = ConvertOperand(binary.Operand2);
                            // the Shift instructions work on byte operand (CL)
                            string shiftOper2 = ConvertByteOperand(binary.Operand1);
                            // Operands are in a different order for the Shift instructions
                            writer.WriteLine("\t{0} \t{1}, {2}", shiftOp, shiftOper2, shiftOper1);
                            break;
                        default:
                            string binaryOp = ConvertBinaryOp(binary.BinaryOp);
                            string binOper1 = ConvertOperand(binary.Operand1);
                            string binOper2 = ConvertOperand(binary.Operand2);
                            writer.WriteLine("\t{0} \t{1}, {2}", binaryOp, binOper1, binOper2);
                            break;
                    }
                    break;
                case ASM_Idiv iDiv:
                    string divOperand = ConvertOperand(iDiv.Operand);
                    writer.WriteLine("\tidivl \t{0}", divOperand);
                    break;
                case ASM_Cmp cmp:
                    string cmpOp1 = ConvertOperand(cmp.Operand1);
                    string cmpOp2 = ConvertOperand(cmp.Operand2);
                    writer.WriteLine("\tcmpl \t{0}, {1}", cmpOp1, cmpOp2);
                    break;
                case ASM_Jmp jmp:
                    writer.WriteLine("\tjmp \t.L{0}", jmp.Identifier);
                    break;
                case ASM_JmpCC jmpCC:
                    string condCode = CovertCondCode(jmpCC.ConditionCode);
                    writer.WriteLine("\tj{0} \t.L{1}", condCode, jmpCC.Identifier);
                    break;
                case ASM_SetCC setCC:
                    string setCondCode = CovertCondCode(setCC.ConditionCode);
                    string setOpByte = ConvertByteOperand(setCC.Operand);
                    writer.WriteLine("\tset{0} \t{1}", setCondCode, setOpByte);
                    break;
                case ASM_Label label:
                    writer.WriteLine("\t.L{0}:", label.Identifier);
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

        private static string CovertCondCode(ASM_CondCode conditionCode)
        {
            return conditionCode switch
            {
                ASM_EQ => "e",
                ASM_NE => "ne",
                ASM_GT => "g",
                ASM_GE => "ge",
                ASM_LT => "l",
                ASM_LE => "le",
                _ => throw new InvalidOperationException("Code Emit: Condition Code Not Defined for Conversion: {0}"),
            };
        }

        private static string ConvertByteOperand(ASM_Operand operand)
        {
            return operand switch
            {
                ASM_Register register => register.Register switch
                {
                    // convert these to the one byte register
                    ASM_AX => "%al",
                    ASM_CL => "%cl",
                    ASM_DX => "%dl",
                    ASM_R10 => "%r10b",
                    ASM_R11 => "%r11b",
                    // All others just convert normal
                    _ => ConvertOperand(operand),
                },
                // All others just convert normal
                _ => ConvertOperand(operand),
            };
        }

        private static string ConvertOperand(ASM_Operand operand)
        {
            return operand switch
            {
                ASM_Register register => register.Register switch
                {
                    ASM_AX => "%eax",
                    ASM_CL => "%ecx",
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
                ASM_BitwiseAND => "andl",
                ASM_BitwiseOR => "orl",
                ASM_BitwiseXOR => "xorl",
                ASM_BitwiseLShift => "sall",
                ASM_BitwiseRShift => "sarl",
                _ => throw new InvalidOperationException("Unary Operand Not Defined for Conversion: {0}"),
            };
        }

        private static void EmitStackNote(StreamWriter writer)
        {
            writer.WriteLine("\t.section .note.GNU-stack,\"\",@progbits");
        }
    }
}
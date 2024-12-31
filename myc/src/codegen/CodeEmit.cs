
using System.Runtime.Serialization;

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
            foreach (ASM_Instruction instr in function.Body)
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
                    writer.WriteLine("\tret");
                    break;

                default:
                    return;
            }
            return;
        }

        private static string ConvertOperand(ASM_Operand operand)
        {
            switch (operand)
            {
                case ASM_Register:
                    return "%eax";
                case ASM_Imm imm:
                    return string.Format("${0}", imm.Value);
                default:
                    throw new InvalidOperationException("Operand Not Defined for Conversion: {0}");
            }
        }

        private static void EmitStackNote(StreamWriter writer)
        {
            writer.WriteLine("\t.section .note.GNU-stack,\"\",@progbits");
        }
    }
}
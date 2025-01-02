
using System.Diagnostics;

namespace myc
{
    public class PseudoState(int Offset, Dictionary<string, int> Map)
    {
        public int CurrentOffset = Offset;
        public Dictionary<string, int> OffsetMap = Map;
    }

    public static class ReplacePseudos
    {
        static readonly PseudoState State = new(0, []);

        public static int Replace(ASM_Program program)
        {
            List<ASM_Instruction> newInstructions = [];
            foreach (ASM_Instruction instr in program.Function.Instructions)
            {
                newInstructions.Add(ReplaceInInstruction(instr));
            }
            program.Function.Instructions = newInstructions;

            // Return the last stack slot.
            return State.CurrentOffset;
        }

        private static ASM_Instruction ReplaceInInstruction(ASM_Instruction instruction)
        {
            switch (instruction)
            {
                case ASM_Mov mov:
                    ASM_Operand newMovSrc = ReplaceOperand(mov.Source);
                    ASM_Operand newMovDst = ReplaceOperand(mov.Destination);
                    return new ASM_Mov(newMovSrc, newMovDst);
                case ASM_Unary unary:
                    ASM_Operand newDst = ReplaceOperand(unary.Operand);
                    return new ASM_Unary(unary.UnaryOp, newDst);
                case ASM_Ret rtrn:
                    return rtrn;
                case ASM_AllocateStack:
                default:
                    throw new ArgumentException(string.Format("Unexpected ASM Instruction Type: {0}", instruction.GetType().Name));
            }
        }

        private static ASM_Operand ReplaceOperand(ASM_Operand operand)
        {
            switch (operand)
            {
                case ASM_Pseudo pseudo:
                    if (!State.OffsetMap.TryGetValue(pseudo.Identifier, out _))
                    {
                        //identifier not found create new offset
                        State.CurrentOffset -= 4;
                        State.OffsetMap.Add(pseudo.Identifier, State.CurrentOffset);
                    }
                    // use the offset to create a new Stack operand
                    return new ASM_Stack(State.OffsetMap[pseudo.Identifier]);
                default:
                    // not a pseudo do nothing.
                    return operand;
            }
        }
    }
}
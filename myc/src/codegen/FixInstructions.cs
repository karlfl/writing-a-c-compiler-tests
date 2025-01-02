namespace myc
{
    public static class FixInstructions
    {
        public static void Fix(ASM_Program program, int lastStackSlot)
        {
            List<ASM_Instruction> fixedInstructions = [];

            //Add instruction to allocate the stack with the appropriate bytes
            fixedInstructions.Add(new ASM_AllocateStack(-lastStackSlot));

            // Fixup the remainder of the instructions
            foreach (ASM_Instruction instruction in program.Function.Instructions)
            {
                switch (instruction)
                {
                    case ASM_Mov mov:
                        //only fix up the mov statements that use stack for both source and dest
                        if (
                            mov.Source.GetType() == typeof(ASM_Stack) &&
                            mov.Destination.GetType() == typeof(ASM_Stack))
                        {
                            // Change the Mov instruction to use the R10 register
                            fixedInstructions.Add(new ASM_Mov(mov.Source, new ASM_Register(new ASM_R10())));
                            fixedInstructions.Add(new ASM_Mov(new ASM_Register(new ASM_R10()), mov.Destination));
                        } else {
                            fixedInstructions.Add(mov);
                        }
                        break;
                    default:
                        fixedInstructions.Add(instruction);
                        break;
                }
            }

            program.Function.Instructions = fixedInstructions;
        }
    }
}
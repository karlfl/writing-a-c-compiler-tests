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
                        // fix up the mov statements that use stack for both source and dest
                        if (
                            mov.Source.GetType() == typeof(ASM_Stack) &&
                            mov.Destination.GetType() == typeof(ASM_Stack))
                        {
                            // Change the Mov instruction to use the R10 register
                            fixedInstructions.Add(new ASM_Mov(mov.Source, new ASM_Register(new ASM_R10())));
                            fixedInstructions.Add(new ASM_Mov(new ASM_Register(new ASM_R10()), mov.Destination));
                        }
                        else
                        {
                            //no changes needed just add it back
                            fixedInstructions.Add(mov);
                        }
                        break;
                    case ASM_Idiv idiv:
                        // fix up the idiv statements that use a constant
                        if (idiv.Operand.GetType() == typeof(ASM_Imm))
                        {
                            // Add a Mov instruction to use the R10 register for idiv
                            fixedInstructions.Add(new ASM_Mov(idiv.Operand, new ASM_Register(new ASM_R10())));
                            fixedInstructions.Add(new ASM_Idiv(new ASM_Register(new ASM_R10())));
                        }
                        else
                        {
                            //no changes needed just add it back
                            fixedInstructions.Add(idiv);
                        }
                        break;
                    case ASM_Cmp cmp:
                        // Fix up the cmp that has memory address for both operands
                        if (cmp.Operand1.GetType() == typeof(ASM_Stack) &&
                           cmp.Operand2.GetType() == typeof(ASM_Stack))
                        {
                            fixedInstructions.Add(new ASM_Mov(cmp.Operand1, new ASM_Register(new ASM_R10())));
                            fixedInstructions.Add(new ASM_Cmp(new ASM_Register(new ASM_R10()), cmp.Operand2));
                        }
                        // Fix up the cmp that has constant address second operand
                        else if (cmp.Operand2.GetType() == typeof(ASM_Imm))
                        {
                            fixedInstructions.Add(new ASM_Mov(cmp.Operand2, new ASM_Register(new ASM_R11())));
                            fixedInstructions.Add(new ASM_Cmp(cmp.Operand1, new ASM_Register(new ASM_R11())));
                        }
                        else
                        {
                            //no changes needed just add it back
                            fixedInstructions.Add(cmp);
                        }
                        break;
                    case ASM_Binary binary:
                        FixupBinary(fixedInstructions, binary);
                        break;
                    default:
                        //no changes needed just add it back
                        fixedInstructions.Add(instruction);
                        break;
                }
            }

            program.Function.Instructions = fixedInstructions;
        }

        private static void FixupBinary(List<ASM_Instruction> fixedInstructions, ASM_Binary binary)
        {

            switch (binary.BinaryOp)
            {
                case ASM_BinaryAdd:
                case ASM_BinarySub:
                case ASM_BitwiseAND:
                case ASM_BitwiseOR:
                case ASM_BitwiseXOR:
                    // fix up the Add/Sub statements that use stack for both operands
                    if (binary.Operand1.GetType() == typeof(ASM_Stack) &&
                        binary.Operand2.GetType() == typeof(ASM_Stack))
                    {
                        // Add a Mov instruction to use the R10 register for first operand
                        fixedInstructions.Add(new ASM_Mov(binary.Operand1, new ASM_Register(new ASM_R10())));
                        fixedInstructions.Add(new ASM_Binary(binary.BinaryOp, new ASM_Register(new ASM_R10()), binary.Operand2));
                    }
                    else
                    {
                        //no changes needed just add it back
                        fixedInstructions.Add(binary);
                    }
                    break;
                case ASM_BinaryMult:
                    // fix up the Mult statements that use stack for second operand
                    if (binary.Operand2.GetType() == typeof(ASM_Stack))
                    {
                        // Add a Mov instruction to use the R11 register for first operand
                        fixedInstructions.Add(new ASM_Mov(binary.Operand2, new ASM_Register(new ASM_R11())));
                        fixedInstructions.Add(new ASM_Binary(binary.BinaryOp, binary.Operand1, new ASM_Register(new ASM_R11())));
                        fixedInstructions.Add(new ASM_Mov(new ASM_Register(new ASM_R11()), binary.Operand2));
                    }
                    else
                    {
                        //no changes needed just add it back
                        fixedInstructions.Add(binary);
                    }
                    break;
                case ASM_BitwiseLShift:
                case ASM_BitwiseRShift:
                    // fix up the Shift statements that use stack or constants for operands
                    ASM_Operand newOp1 = binary.Operand1;
                    ASM_Operand newOp2 = binary.Operand2;
                    //Op1 can't use stack
                    if (newOp1.GetType() == typeof(ASM_Stack))
                    {
                        // Add a Mov instruction to use the CL register for first operand
                        newOp1 = new ASM_Register(new ASM_CL());
                        fixedInstructions.Add(new ASM_Mov(binary.Operand1, newOp1));
                    }
                    // Op2 can't be a constant
                    if (newOp2.GetType() == typeof(ASM_Imm))
                    {
                        // Add a Mov instruction to use the R10 register for second operand
                        newOp2 = new ASM_Register(new ASM_R10());
                        fixedInstructions.Add(new ASM_Mov(binary.Operand2, newOp2 ));
                    }
                    fixedInstructions.Add(new ASM_Binary(binary.BinaryOp, newOp1, newOp2));
                    break;
                default:
                    //no changes needed just add it back
                    fixedInstructions.Add(binary);
                    break;

            }
        }
    }
}
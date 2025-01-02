using System.Text;

namespace myc
{
    public class TAC_Function
    {
        public readonly string Name;
        public readonly List<TAC_Instruction> Instructions;

        public TAC_Function(string name, List<TAC_Instruction> instructions)
        {
            this.Name = name;
            this.Instructions= instructions;
        }

        public string Print()
        {
            StringBuilder output = new();
            output.AppendLine(string.Format("{0}:", this.Name));
            foreach (TAC_Instruction instr in this.Instructions)
            {
                output.AppendLine(instr.Print());
            }
            return output.ToString();
        }
    }
}
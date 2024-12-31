
namespace myc {
    public class AST_Identifier : AST_Base
    {
        public readonly string Name;
        public AST_Identifier(string name)
        {
            this.Name = name;
        }

        public string Print()
        {
            return this.Name;
        }
    }
}
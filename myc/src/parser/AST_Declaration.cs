namespace myc
{
    public abstract class AST_Decl(string name) : AST_Base
    {
        public readonly string Name = name;
        public abstract string Print();
    }

    public class AST_Declaration(string name, AST_Factor? init) : AST_Decl(name)
    {
        public readonly AST_Factor? Init = init;
        public override string Print()
        {
            if (this.Init == null)
                return string.Format("{0}", Name);
            else
                return string.Format("{0} = {1}", Name, Init.Print());
        }
    }

    public class AST_Label(string name) : AST_Decl(name){

        public override string Print()
        {
            return string.Format("{0}:", Name);
        }
    }
}
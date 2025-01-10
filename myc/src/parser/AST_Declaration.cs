namespace myc
{
    public class AST_Declaration(string name, AST_Factor? init) : AST_Base
    {
        public readonly string Name = name;
        public readonly AST_Factor? Init = init;
        public string Print()
        {
            if (this.Init == null)
                return string.Format("{0}", Name);
            else
                return string.Format("{0} = {1}", Name, Init.Print());
        }
    }
}
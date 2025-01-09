namespace myc
{
    public class AST_Declaration(string name, AST_Factor? init) : AST_Base
    {
        public readonly string Name = name;
        public readonly AST_Factor? Init = init;
        public string Print(){
            return string.Format("{0} = {1}", Name, Init);
        }
    }
}
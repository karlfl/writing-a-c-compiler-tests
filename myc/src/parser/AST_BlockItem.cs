namespace myc {

    public abstract class AST_BlockItem {
        public abstract string Print();
    }

    public class AST_BlockStatement(AST_Statement statement):AST_BlockItem{
        public readonly AST_Statement Statement = statement;

        public override string Print(){
            return this.Statement.Print();
        }
    }
    
    public class AST_BlockDeclaration(AST_Declaration declaration):AST_BlockItem{
        public readonly AST_Declaration Declaration = declaration;

        public override string Print(){
            return this.Declaration.Print();
        }
    }}
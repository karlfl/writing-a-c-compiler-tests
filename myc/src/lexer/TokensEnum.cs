namespace myc
{
    public enum TokensEnum
    {
        //(* tokens with contents *)
        Identifier,
        Constant,
        //(* Keywords *)
        KWInt,
        KWReturn,
        KWVoid,
        KWIf,
        KWElse,
        //(* Operators *)
        Tilde,
        Hyphen,
        DoubleHyphen,
        Plus,
        Asterisk,
        ForwardSlash,
        Percent,
        // AND,
        // OR,
        // XOR,
        // LeftShift,
        // RightShift,
        LogicalNOT,
        LogicalAND,
        LogicalOR,
        EqualEqual,
        NotEqual,
        LessThan,
        GreaterThan,
        LessOrEqual,
        GreaterOrEqual,
        Assignment,
        // (* Loops *)
        Do,
        While,
        For,
        Break,
        Continue,
        //(* punctuation *)
        OpenParen,
        CloseParen,
        OpenBrace,
        CloseBrace,
        Semicolon,
        QuestionMark,
        Colon
    }
}
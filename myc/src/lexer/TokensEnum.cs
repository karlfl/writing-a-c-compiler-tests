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
        EqualTo,
        NotEqualTo,
        LessThan,
        GreaterThan,
        LessOrEqual,
        GreaterOrEqual,
        //(* punctuation *)
        OpenParen,
        CloseParen,
        OpenBrace,
        CloseBrace,
        Semicolon
    }
}
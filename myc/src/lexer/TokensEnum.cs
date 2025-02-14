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
        KWDo,
        KWWhile,
        KWFor,
        KWBreak,
        KWContinue,
        KWGoto,
        //(* Operators *)
        Tilde,
        Hyphen,
        HyphenEqual,
        DoubleHyphen,
        Plus,
        PlusEqual,
        DoublePlus,
        Asterisk,
        AsteriskEqual,
        ForwardSlash,
        ForwardSlashEqual,
        Percent,
        PercentEqual,
        AND,
        ANDEqual,
        OR,
        OREqual, 
        XOR,
        XOREqual,
        LeftShift,
        LeftShiftEqual,
        RightShift,
        RightShiftEqual,
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
        //(* punctuation *)
        OpenParen,
        CloseParen,
        OpenBrace,
        CloseBrace,
        Semicolon,
        Colon,
        QuestionMark
    }
}
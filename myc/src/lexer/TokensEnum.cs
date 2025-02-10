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
        OR,
        XOR,
        LeftShift,
        RightShift,
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
        QuestionMark,
        Colon
    }
}
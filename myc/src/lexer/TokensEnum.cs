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
        Decrement,
        //(* punctuation *)
        OpenParen,
        CloseParen,
        OpenBrace,
        CloseBrace,
        Semicolon
    }
}
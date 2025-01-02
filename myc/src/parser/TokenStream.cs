namespace myc
{
    public class TokenStream(StringReader tokenStream)
    {
        public readonly StringReader Stream = tokenStream;
        public Stack<string> PeekBuffer = [];

        public string Get_Token()
        {
            //return the last peeked item if available.
            if (PeekBuffer.Count > 0)
            {
                return PeekBuffer.Pop();
            }
            //no peeked values read from stream
            return ReadToken();
        }

        public TokensEnum Peek_TokenEnum()
        {
            // if there is already a token in the peek buffer use that
            // otherwise read the token from the stream.
            string token = (PeekBuffer.Count > 0) ? PeekBuffer.Pop() : ReadToken();

            // Since we're just peeking, place the token in a buffer to get later.
            PeekBuffer.Push(token);

            string[] tokenParts = token.Split(' ');
            _ = Enum.TryParse(tokenParts[0], out TokensEnum keyToken);

            return keyToken;
        }

        protected string ReadToken()
        {
            string token = this.Stream.ReadLine() ??
                throw new EndOfStreamException("Unexpected End of File");
            // Remove the end token ';' if there is one
            return token.Replace(";", "");
        }

    }

}
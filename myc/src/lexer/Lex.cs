using System.Text;
using System.Text.RegularExpressions;

namespace myc
{
    public static class Lex
    {

        private static readonly string[] Keywords = ["int", "return", "void"];

        private static readonly TokenDef[] TokenDefs =
        [
            new("Identifier",      @"[a-zA-Z_]\w*\b",  ConvertKeyWord),
            new("Constant",        @"[0-9]+\b",        ConvertInt),
            new("OpenParen",       @"\(",              (name)=>TokensEnum.OpenParen.ToString()),
            new("CloseParen",      @"\)",              (name)=>TokensEnum.CloseParen.ToString()),
            new("OpenBrace",       @"\{",              (name)=>TokensEnum.OpenBrace.ToString()),
            new("CloseBrace",      @"\}",              (name)=>TokensEnum.CloseBrace.ToString()),
            new("Semicolon",       @";",               (name)=>TokensEnum.Semicolon.ToString()),
            new("Complement",      @"~",               (name)=>TokensEnum.Tilde.ToString()),
            new("Negation",        @"-",               (name)=>TokensEnum.Hyphen.ToString()),
            new("Decrement",       @"--",              (name)=>TokensEnum.DoubleHyphen.ToString()),
            new("Add",             @"\+",              (name)=>TokensEnum.Plus.ToString()),
            new("Multiply",        @"\*",              (name)=>TokensEnum.Asterisk.ToString()),
            new("Divide",          @"/",               (name)=>TokensEnum.ForwardSlash.ToString()),
            new("Remainder",       @"%",               (name)=>TokensEnum.Percent.ToString()),
            // new("AND",             @"\&",              (name)=>TokensEnum.AND.ToString()),
            // new("OR",              @"\|",              (name)=>TokensEnum.OR.ToString()),
            // new("XOR",             @"\^",              (name)=>TokensEnum.XOR.ToString()),
            // new("LeftShift",       @"\<\<",            (name)=>TokensEnum.LeftShift.ToString()),
            // new("RightShift",      @"\>\>",            (name)=>TokensEnum.RightShift.ToString()),
            new("LogicalNOT",      @"!",               (name)=>TokensEnum.LogicalNOT.ToString()),
            new("LogicalAND",      @"\&\&",            (name)=>TokensEnum.LogicalAND.ToString()),
            new("LogicalOR",       @"\|\|",            (name)=>TokensEnum.LogicalOR.ToString()),
            new("EqualEqual",      @"==",              (name)=>TokensEnum.EqualEqual.ToString()),
            new("NotEqual",        @"!=",              (name)=>TokensEnum.NotEqual.ToString()),
            new("LessThan",        @"<",               (name)=>TokensEnum.LessThan.ToString()),
            new("GreaterThan",     @">",               (name)=>TokensEnum.GreaterThan.ToString()),
            new("LessOrEqual",     @"<=",              (name)=>TokensEnum.LessOrEqual.ToString()),
            new("GreaterOrEqual",  @">=",              (name)=>TokensEnum.GreaterOrEqual.ToString()),
            new("Assignment",      @"=",               (name)=>TokensEnum.Assignment.ToString()),
            new("?Cond",           @"\?",              (name)=>TokensEnum.QuestionMark.ToString()),
            new(":Cond",           @":",               (name)=>TokensEnum.Colon.ToString()),
       ];


        public static string Process(string source)
        {
            StringBuilder sourceTokens = new();
            string sourceString = source;
            if (sourceString.Trim() == "")
            {
                throw new Exception("Empty file, nothing to process");
            }

            while (sourceString != "")
            {
                //Find all token matches for the current string
                List<MatchDef> matches = FindAllMatches(sourceString);

                //Find the longest match, if none found throw exception
                MatchDef? longMatch = matches.MaxBy(m => m.match.Length) ?? throw new Exception("No token matches found");

                // Console.WriteLine("longMatch.match.Value: {0}", longMatch.match.Value);
                // Console.WriteLine("LongMatch.matchedToken {0}", longMatch.matchedToken.Name);

                if (string.IsNullOrEmpty(longMatch.match.Value)) { throw new Exception(String.Format("Invalid Match Found '{0}'", longMatch.match.Value)); }

                sourceTokens.AppendLine(longMatch.matchedToken.Converter(longMatch.match.Value) + ";");

                // Console.WriteLine("Before: {0}", sourceString);
                sourceString = sourceString[longMatch.match.Length..];
                // Console.WriteLine("After : {0}", sourceString);

                sourceString = sourceString.TrimStart();
            }
            return sourceTokens.ToString();
        }

        static string ConvertKeyWord(string name)
        {
            return name switch
            {
                "int" => TokensEnum.KWInt.ToString(),
                "return" => TokensEnum.KWReturn.ToString(),
                "void" => TokensEnum.KWVoid.ToString(),
                "if" => TokensEnum.KWIf.ToString(),
                "else" => TokensEnum.KWElse.ToString(),
                "do" => TokensEnum.KWDo.ToString(),
                "while" => TokensEnum.KWWhile.ToString(),
                "for" => TokensEnum.KWFor.ToString(),
                "break" => TokensEnum.KWBreak.ToString(),
                "continue" => TokensEnum.KWContinue.ToString(),
                _ => string.Format("{0} \"{1}\"", TokensEnum.Identifier.ToString(), name),
            };
        }


        internal static string ConvertInt(string value)
        {
            return string.Format("{0} {1}", TokensEnum.Constant.ToString(), value);
        }


        internal static List<MatchDef> FindAllMatches(string source)
        {
            List<MatchDef> matches = [];
            foreach (TokenDef def in TokenDefs)
            {
                Match result = def.Regex.Match(source);
                matches.Add(
                    new MatchDef(result.Groups[0], def)
                );
            }
            return matches;
        }

    }
}
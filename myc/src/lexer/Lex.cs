
using System.Collections.Immutable;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.XPath;

namespace myc
{
    public static class Lex
    {

        private static readonly string[] Keywords = ["int", "return", "void"];

        private static readonly TokenDef[] TokenDefs =
        [
            new("Identifier",  @"^[a-zA-Z_]\w*\b",  ConvertIdentifier),
            new("Constant",    @"^[0-9]+\b",        ConvertInt),
            new("OpenParen",   @"^\(",              (name)=>TokensEnum.OpenParen.ToString()),
            new("CloseParen",  @"^\)",              (name)=>TokensEnum.CloseParen.ToString()),
            new("OpenBrace",   @"^{",               (name)=>TokensEnum.OpenBrace.ToString()),
            new("CloseBrace",  @"^}",               (name)=>TokensEnum.CloseBrace.ToString()),
            new("Semicolon",   @"^;",               (name)=>TokensEnum.Semicolon.ToString()),
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

                if(string.IsNullOrEmpty(longMatch.match.Value)){throw new Exception("Invalid Match Found");}

                sourceTokens.AppendLine(longMatch.matchedToken.Converter(longMatch.match.Value) + ";");

                // Console.WriteLine("Before: {0}", sourceString);
                sourceString = sourceString[longMatch.match.Length..];
                // Console.WriteLine("After : {0}", sourceString);

                sourceString = sourceString.TrimStart();
            }
            return sourceTokens.ToString();
        }

        static string ConvertIdentifier(string name)
        {
            return name switch
            {
                "int" => TokensEnum.KWInt.ToString(),
                "return" => TokensEnum.KWReturn.ToString(),
                "void" => TokensEnum.KWVoid.ToString(),
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
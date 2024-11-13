
using System.Collections.Immutable;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.XPath;

namespace myc
{

    public class TokenDef(string name, string matchString, Func<string, string> converter)
    {
        public readonly string Name = name;
        public readonly Regex Regex = new(matchString);
        public readonly Func<string, string> Converter = converter;
    }

    public class MatchDef(Group match, TokenDef matchingToken)
    {
        public readonly Group match = match;
        public readonly TokenDef matchedToken = matchingToken;
    }

    public enum Tokens
    {
        //(* tokens with contents *)
        Identifier,
        Constant,
        //(* Keywords *)
        KWInt,
        KWReturn,
        KWVoid,
        //(* punctuation *)
        OpenParen,
        CloseParen,
        OpenBrace,
        CloseBrace,
        Semicolon
    }

    public static class Lex
    {

        private static readonly string[] Keywords = ["int", "return", "void"];

        private static readonly TokenDef[] TokenDefs =
        [
            new("Identifier",  @"^[a-zA-Z_]\w*\b",  ConvertIdentifier),
            new("Constant",    @"^[0-9]+\b",        ConvertInt),
            new("OpenParen",   @"^\(",              (name)=>Tokens.OpenParen.ToString()),
            new("CloseParen",  @"^\)",              (name)=>Tokens.CloseParen.ToString()),
            new("OpenBrace",   @"^{",               (name)=>Tokens.OpenBrace.ToString()),
            new("CloseBrace",  @"^}",               (name)=>Tokens.CloseBrace.ToString()),
            new("Semicolon",   @"^;",               (name)=>Tokens.Semicolon.ToString()),
        ];


        public static string Process(string source)
        {
            StringBuilder sourceTokens = new();
            string sourceString = source;
            if (sourceString.Trim() == "")
            {
                return "";
            }

            while (sourceString != "")
            {
                //Find all token matches
                List<MatchDef> matches = FindAllMatches(sourceString);

                //Find the longest match
                MatchDef? longMatch = matches.MaxBy(m => m.match.Length);
                if (longMatch is null)
                {
                    // Throw error
                    return "";
                }
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
                "int" => Tokens.KWInt.ToString(),
                "return" => Tokens.KWReturn.ToString(),
                "void" => Tokens.KWVoid.ToString(),
                _ => string.Format("{0} \"{1}\"", Tokens.Identifier.ToString(), name),
            };
        }


        internal static string ConvertInt(string value)
        {
            return string.Format("{0} {1}", Tokens.Constant.ToString(), value);
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
using System.Text.RegularExpressions;

namespace myc
{
    public class TokenDef(string name, string matchString, Func<string, string> converter)
    {
        public readonly string Name = name;
        public readonly Regex Regex = new("^" + matchString);
        public readonly Func<string, string> Converter = converter;
    }
}
using System.Text.RegularExpressions;

namespace myc
{
    public class MatchDef(Group match, TokenDef matchingToken)
    {
        public readonly Group match = match;
        public readonly TokenDef matchedToken = matchingToken;
    }
}
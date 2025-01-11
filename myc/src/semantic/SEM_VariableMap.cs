using System.Security.Cryptography.X509Certificates;

namespace myc {
    public class VariableMap : List<(string Name, string UniqueName, bool InCurrentScope)> {

        public VariableMap MakeCopy (){
            VariableMap newMap = [];
            foreach (var (Name, UniqueName, _) in this)
            {
                // when copying the old variables are not 'in-scope'
                newMap.Add((Name, UniqueName, false));
            }
            return newMap;
        }
    }
}
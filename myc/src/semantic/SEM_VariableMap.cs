using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace myc
{
    public class VariableMap
    {
        
        private class MapValue(string uName, bool inScope)
        {
            public string UniqueName = uName;
            public bool InCurrentScope = inScope;
        }
        readonly Dictionary<string, MapValue> map = [];
        public VariableMap() { }

        public void Add(string Name, string UniqueName, bool InCurrentScope)
        {
            this.map[Name] =  new(UniqueName, InCurrentScope);
        }
        private void Add(string Name, MapValue value)
        {
            this.map[Name] =  value;
        }

        public VariableMap MakeCopy()
        {
            VariableMap newMap = new();

            foreach (var (Name, Value) in this.map)
            {
                // when copying the old variables are not 'in-scope'
                newMap.Add(Name, new(Value.UniqueName, false));
            }
            return newMap;
        }

        public bool InCurrentScope(string Name){
            if(this.map.TryGetValue(Name, out MapValue? value)){
                return value.InCurrentScope;
            }
            return false;
        }

        public bool Exists(string Name) {
            return this.map.TryGetValue(Name, out MapValue? value);
        }

        public string GetUniqueName(string Name) {
            return this.map[Name].UniqueName;
        }
    }
}
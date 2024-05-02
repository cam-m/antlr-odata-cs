using ODType = AntlrODataCSharp.Lang.Edm.Type;

namespace AntlrODataCSharp.Lang.Edm
{
    public class Property {
        public string Name;
        public ODType Type;
        public bool Nullable;
    }
}
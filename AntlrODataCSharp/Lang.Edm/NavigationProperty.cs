using ODType = AntlrODataCSharp.Lang.Edm.Type;

namespace AntlrODataCSharp.Lang.Edm
{
    public class NavigationProperty {
        public string Name;
        public ODType Type;
        public bool Nullable;
        public bool ContainsTarget;
        public bool Partner;
        public ReferentialConstraint[] ReferentialConstraints;
        public OnDelete OnDelete;
    }
}
namespace AntlrODataCSharp.Lang.Edm
{
    public class EntityType {
        public Schema Schema;
        public string Name;
        public Key Key;
        public Property[] Properties;
        public NavigationProperty[] NavigationProperties;
    }
}
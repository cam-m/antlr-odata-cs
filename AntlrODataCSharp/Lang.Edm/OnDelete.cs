namespace AntlrODataCSharp.Lang.Edm
{
    public enum Action
    {
        Cascade, 
        None, 
        SetNull, 
        SetDefault
    }
    public class OnDelete {
        public Action Action;
    }
}
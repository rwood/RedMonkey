namespace RedMonkey.SchemaAnalysis
{
    public interface IBaseObject
    {
        string Name { get; set; }
        string Notes { get; set; }
        void AddNote(string _Note);
    }
}
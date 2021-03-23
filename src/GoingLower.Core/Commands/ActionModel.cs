namespace GoingLower.Core.Actions
{
    public class ActionModel
    {
        public ActionModel()
        {
        }

        public ActionModel(string name, object? arg = null)
        {
            Name = name;
            Arg  = arg;
        }

        public string  Name { get; set; }
        public object? Arg  { get; set; }
    }

}
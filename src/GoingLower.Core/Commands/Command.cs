namespace GoingLower.Core.Actions
{
    public class Command
    {
        public Command()
        {
        }

        public Command(string name, object? arg = null)
        {
            Name = name;
            Arg  = arg;
        }

        public string  Name { get; set; }
        public object? Arg  { get; set; }
    }

}
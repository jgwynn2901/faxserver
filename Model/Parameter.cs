namespace FaxServer.Model
{
    public class Parameter
    {
        public string LastError { get; protected set; }
        public bool IsValid { get; protected set; }
        public virtual Parameter Validate()
        {
            IsValid = false;
            LastError = string.Empty;
            return this;
        }
    }
}

namespace FaxServer.Model
{
    public class FaxQueue : User
    {
        public string Queue { get; set; }
        public override Parameter Validate()
        {
            if (!string.IsNullOrEmpty(Queue)) return base.Validate();
            LastError = "Queue is missing.";
            return this;
        }
    }
}

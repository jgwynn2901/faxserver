namespace FaxServer.Model
{
    public class User : Parameter
    {
        public string Username { get; set; }
        public string Password { get; set; }

        public override Parameter Validate()
        {
            base.Validate();
            IsValid = !string.IsNullOrEmpty(Username);
            if (!IsValid) LastError = "Username is missing.";
            return this;
        }
    }
}

namespace FaxServer.Model
{
    public class Entity : Parameter
    {
        public string  Name { get; set; }
        public string  Company { get; set; }
        public string FaxNumber { get; set; }

        public override Parameter Validate()
        {
            base.Validate();
            if (string.IsNullOrEmpty(Name))
            {
                LastError = "Name is missing.";
                return this;
            }
            
            if (string.IsNullOrEmpty(FaxNumber))
            {
                LastError = "FaxNumber is missing.";
                return this;
            }

            IsValid = true;
            return this;
        }
    }
}

namespace EmailRouter.Service.Validation
{
    public class ValidationResult
    {
        public bool Success { get; set; }
        public string Error { get; set; }

        public static ValidationResult Fail(string error)
        {
            return new ValidationResult
            {
                Error = error
            };
        }

        public static ValidationResult Pass()
        {
            return new ValidationResult
            {
                Success = true
            };
        }
    }
}
namespace ChatCase.Domain.Validation
{
    public class ValidationMessage
    {
        public const string Required = "This field is required !";
        public const string RequiredField = "{0} field is required !";
        public const string RequiredEmail = "Email address is wrong !";
        public const string RequiredDate = "Date Format is wrong !ş";
        public const string RequiredPhone = "Phone Number is wrong !";
        public const string InvalidValue = "{0} field is invalid!";

        public static string MaxLength(int length)
        {
            return $"You can enter at most {length} character !";
        }

        public static string MinLength(int length)
        {
            return $"You can enter at least {length} character !";
        }

        public static string GreaterThan(string field, int value)
        {
            return $"{field} has to be greater than {value}";
        }

        public static string GreaterThan(string value)
        {
            return $" The word has to be greater than {value}";
        }
    }
}

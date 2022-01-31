namespace Api.Dto.Authentication
{
    public class PasswordDto
    {
        public string Hash { get; set; }
        public string Salt { get; set; }
    }
}
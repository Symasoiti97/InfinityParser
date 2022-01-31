using Api.Dto.Authentication;

namespace Api.Core.Services
{
    public interface IPasswordService
    {
        PasswordDto GenerateSaltAndPassword(string password);
        bool CheckAccessPassword(PasswordDto hash, string password);
    }
}
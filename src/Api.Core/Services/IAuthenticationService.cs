using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Api.Dto.Authentication;

namespace Api.Core.Services
{
    public interface IAuthenticationService
    {
        Task Register([NotNull] RegisterRequest request);
        Task Login([NotNull] LoginRequest request);
    }
}
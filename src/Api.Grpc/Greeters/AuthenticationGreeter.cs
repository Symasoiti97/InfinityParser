using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Api.Core.Services;
using Api.Dto.Authentication;
using GrpcService;

namespace Api.Grpc.Greeters
{
    public class AuthenticationGreeter : Greeter.GreeterBase
    {
        private readonly IAuthenticationService _authenticationService;

        public AuthenticationGreeter([NotNull] IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService ?? throw new ArgumentNullException(nameof(authenticationService));
        }

        public async Task Register(RegisterRequest request)
        {
            await _authenticationService.Register(request);
        }

        public async Task Login(LoginRequest request)
        {
            await _authenticationService.Login(request);            
        }

        public Task Logout()
        {
            throw new NotImplementedException();
        }
    }
}
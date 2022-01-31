using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using System.Transactions;
using Api.Dto.Authentication;
using Db.Models;
using Db.Provider;
using Microsoft.EntityFrameworkCore;

namespace Api.Core.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IDataProvider _dataProvider;
        private readonly IPasswordService _passwordService;

        public AuthenticationService([NotNull] IDataProvider dataProvider, [NotNull] IPasswordService passwordService)
        {
            _dataProvider = dataProvider ?? throw new ArgumentNullException(nameof(dataProvider));
            _passwordService = passwordService ?? throw new ArgumentNullException(nameof(passwordService));
        }

        public async Task Register([NotNull] RegisterRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            using var tr = _dataProvider.Transaction(IsolationLevel.RepeatableRead);

            var user = await _dataProvider.Get<UserDb>(i => i.Email == request.Email).SingleOrDefaultAsync();
            if (user != null)
                throw new Exception();

            var password = _passwordService.GenerateSaltAndPassword(request.Password);

            var newUser = new UserDb
            {
                Email = request.Email,
                Name = request.Name,
                Password = password.Hash,
                Salt = password.Salt
            };

            await _dataProvider.Insert(newUser);

            tr.Complete();
        }

        public async Task Login([NotNull] LoginRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            using var tr = _dataProvider.Transaction();

            var user = await _dataProvider.Get<UserDb>(i => i.Email == request.Email).SingleOrDefaultAsync();
            if (user == null)
                throw new Exception();

            if (!_passwordService.CheckAccessPassword(new PasswordDto(), request.Password))
                throw new Exception();

            tr.Complete();
        }
    }
}
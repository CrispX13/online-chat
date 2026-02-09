using Microsoft.AspNetCore.Identity;
using OnlineChatBackend.DTOs;
using OnlineChatBackend.Models;
using OnlineChatBackend.Repositories;

namespace OnlineChatBackend.Services
{
    public class AccountService(AccountRepository accountRepository, JwtService jwtService)
    {

        public void Register(RegisterUserRequest request)
        {
            var account = new Contact
            {
                Name = request.UserName,
            };
            var passHash = new PasswordHasher<Contact>().HashPassword(account, request.Password);
            account.PasswordHash = passHash;
            accountRepository.Add(account);
        }

        public LoginResult Login(string UserName, string Password)
        {
            var account = accountRepository.GetByUserName(UserName);
            var result = new PasswordHasher<Contact>().VerifyHashedPassword(account, account.PasswordHash, Password);
            if (result == PasswordVerificationResult.Success)
            {
                //generate token
                return new LoginResult { Id = account.Id.ToString(), token = jwtService.GenerateToken(account) };
            }
            else
            {
                throw new Exception("Unauthorized");
            }
        }

        public bool ChangePassword(int Id, string LastPassword, string NewPassword)
        {
            var account = accountRepository.GetById(Id);
            if (account == null)
            {
                return false;
            }

            var result = new PasswordHasher<Contact>().VerifyHashedPassword(account, account.PasswordHash, LastPassword);

            if (result == PasswordVerificationResult.Success)
            {
                var passHash = new PasswordHasher<Contact>().HashPassword(account, NewPassword);
                return accountRepository.ChangePassword(Id, passHash);
            }
            else
            {
                return false;
            }
        }

        public bool ChangeName(int Id, string Name)
        {
            var account = accountRepository.GetById(Id);
            if (account == null)
            {
                return false;
            }

            return accountRepository.ChangeName(Id, Name);
        }
    }
}

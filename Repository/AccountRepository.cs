using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NewsStacksAPI.Data;
using NewsStacksAPI.Models;
using NewsStacksAPI.Models.Dto;
using NewsStacksAPI.Repository.IRepository;
using NewsStacksAPI.Utils;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace NewsStacksAPI.Repository
{
    public class AccountRepository : IAccountRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly AppSettings _appSettings;

        public AccountRepository(ApplicationDbContext db, IOptions<AppSettings> appSettings)
        {
            _db = db;
            _appSettings = appSettings.Value;
        }

        public string GenerateToken(string username, string role)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, username),
                    new Claim(ClaimTypes.Role,role),
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials
                                (new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)

            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public string Login(LoginDto model)
        {
            var account = _db.Accounts.SingleOrDefault(x => x.Username == model.Username && x.Password == model.Password);
            if (account == null)
            {
                return null;
            }
            var token = GenerateToken(account.Username, account.Role);
            return token;
        }

        public bool isUniqueUser(string username)
        {
            var account = GetAccount(username);
            if (account == null)
            {
                return true;
            }
            return false;
        }
        public Account GetAccount(string username)
        {
            return _db.Accounts.SingleOrDefault(x => x.Username == username);
        }

        public Account Register(RegisterDto model)
        {
            Account account = new Account
            {
                Username = model.Username,
                Password = model.Password,
                Role = model.Role
            };
            _db.Accounts.Add(account);
            _db.SaveChanges();

            switch (account.Role)
            {
                case "Writer":
                    Writer writer = new Writer
                    {
                        Name = model.Name,
                        Email = model.Email,
                        Account = account
                    };
                    _db.Writers.Add(writer);
                    break;
                case "Publisher":
                    Publisher publisher = new Publisher
                    {
                        Name = model.Name,
                        Email = model.Email,
                        Account = account
                    };
                    _db.Publishers.Add(publisher);
                    break;

                default:
                    break;
            }
            _db.SaveChanges();
            return account;
        }

    }
}

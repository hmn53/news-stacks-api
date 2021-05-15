using NewsStacksAPI.Models;
using NewsStacksAPI.Models.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewsStacksAPI.Repository.IRepository
{
    public interface IAccountRepository
    {
        string GenerateToken(string username, string role);
        bool isUniqueUser(string username);
        string Login(LoginDto model);
        Account Register(RegisterDto model);
        Account GetAccount(string username);
    }
}

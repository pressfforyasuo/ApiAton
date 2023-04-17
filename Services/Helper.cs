using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using ApiAton.Controllers;

namespace ApiAton.Services
{
    public sealed class Helper
    {
        private static Helper instance = null;
        private static readonly object padlock = new object();

        private Helper()
        {
        }

        public static Helper Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new Helper();
                    }
                    return instance;
                }
            }
        }

        public bool IsLettersAndNumbers(string? value)
        {
            if (value == null)
                return true;
            return Regex.IsMatch(value, @"^[0-9a-zA-Z]+$");
        }

        public bool IsLetters(string? value)
        {
            if (value == null)
                return true;
            return Regex.IsMatch(value, @"^[a-zA-Zа-яА-Я]+$");
        }

        public bool isAdmin(string login, string password)
        {
            if (UsersController._context.Users.Any(u => u.Login == login && u.Password == password && u.Admin && u.RevokedOn == null))
            {
                return true;
            }

            return false;
        }
    }
}

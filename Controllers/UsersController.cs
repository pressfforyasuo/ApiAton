using ApiAton.Model;
using Microsoft.AspNetCore.Mvc;

namespace ApiAton.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserContext _context = new();

        [HttpGet("GetAllActiveUser")]
        public async Task<ActionResult<IOrderedQueryable<User>>> GetAllActiveUser(string login, string password)
        {
            if (_context.Users.Any(u => u.Login == login && u.Password == password && u.Admin))
            {
                return Ok(_context.Users.Where(u => u.RevokedOn == null).OrderBy(u => u.CreatedOn));
            }

            return StatusCode(403);
        }

        [HttpGet("GetUser")]
        public async Task<ActionResult<User>> GetUser(string login, string password, string loginForSearch)
        {
            if (_context.Users.Any(u => u.Login == login && u.Password == password && u.Admin))
            {
                if (!_context.Users.Any(u => u.Login == loginForSearch))
                {
                    return StatusCode(404);
                }

                return Ok(_context.Users.Where(u => u.Login == loginForSearch)
                    .Select(u => new {
                        u.Name,
                        u.Gender,
                        u.Birthday,
                        IsActive = u.RevokedOn == null
                    })
                    .FirstOrDefault());
            } 

            return StatusCode(403);
        }

        [HttpGet("GetMe")]
        public async Task<ActionResult<User>> GetMe(string login, string password)
        {
            if (_context.Users.Any(u => u.Login == login && u.Password == password && u.RevokedOn == null))
            {
                return Ok(_context.Users.SingleOrDefault(u => u.Login == login));
            }

            return StatusCode(404);
        }

        [HttpGet("GetUsersByAge")]
        public async Task<ActionResult<List<User>>> GetUsersByAge(string login, string password, int age)
        {
            if (_context.Users.Any(u => u.Login == login && u.Password == password && u.Admin))
            {
                var dataList = _context.Users.Select(u => new
                {
                    u.Guid,
                    u.Name,
                    u.Gender,
                    u.Admin,
                    u.CreatedBy,
                    u.CreatedOn,
                    u.Login,
                    u.Password,
                    u.ModifiedBy,
                    u.ModifiedOn,
                    u.RevokedBy,
                    u.RevokedOn,
                    u.Birthday,
                    age = DateTime.Now.Year - u.Birthday.GetValueOrDefault().Year
                }).ToList().Where(u => u.age > age).ToList();

                if (dataList.Count == 0)
                {
                    return StatusCode(404);
                }

                return Ok(dataList);
            }

            return StatusCode(403);
        }

        [HttpPost("CreateUser")]
        public async Task<ActionResult> CreteUser(string login, string password, string newLogin, string newPassword, string newName, int newGender, DateTime? newBirthday, bool isAdmin)
        {
            if (_context.Users.Any(u => u.Login == login && u.Password == password && u.Admin))
            {
                if (_context.Users.Any(u => u.Login == newLogin))
                {
                    return BadRequest("Данный логин уже используется");
                }

                var newUser = new User
                {
                    Guid = Guid.NewGuid().ToString(),
                    Login = newLogin,
                    Password = newPassword,
                    Name = newName,
                    Gender = newGender,
                    Birthday = newBirthday ?? null,
                    Admin = isAdmin,
                    CreatedOn = DateTime.UtcNow,
                    CreatedBy = login
                };
                _context.Users.Add(newUser);
                await _context.SaveChangesAsync();
                return Ok("Пользователь добавлен");

            }
            return StatusCode(403);
        }


    }
}

using ApiAton.Model;
using ApiAton.Services;
using Microsoft.AspNetCore.Mvc;

namespace ApiAton.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        public static readonly UserContext _context = new();

        [HttpGet("GetAllActiveUser")]
        public async Task<ActionResult> GetAllActiveUser(string login, string password)
        {
            if (Helper.Instance.isAdmin(login, password))
                return Ok(_context.Users.Where(u => u.RevokedOn == null).OrderBy(u => u.CreatedOn));

            return StatusCode(403);
        }

        [HttpGet("GetUser")]
        public async Task<ActionResult> GetUser(string login, string password, string loginForSearch)
        {
            if (Helper.Instance.isAdmin(login, password))
            {
                if (!_context.Users.Any(u => u.Login == loginForSearch))
                    return StatusCode(404);

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
        public async Task<ActionResult> GetMe(string login, string password)
        {
            if (_context.Users.Any(u => u.Login == login && u.Password == password && u.RevokedOn == null))
            {
                return Ok(_context.Users.SingleOrDefault(u => u.Login == login));
            }

            return StatusCode(404);
        }

        [HttpGet("GetUsersByAge")]
        public async Task<ActionResult> GetUsersByAge(string login, string password, int age)
        {
            if (Helper.Instance.isAdmin(login, password))
            {
                var dataList = _context.Users.Where(u => u.Birthday.HasValue).ToList().Where(u => DateTime.Now.Year - u.Birthday.GetValueOrDefault().Year! > age).ToList();

                if (dataList.Count == 0)
                    return StatusCode(404);

                return Ok(dataList);
            }

            return StatusCode(403);
        }

        [HttpPost("CreateUser")]
        public async Task<ActionResult> CreteUser(string login, string password, string newLogin, string newPassword, string newName, int? newGender, DateTime? newBirthday, bool isAdmin)
        {
            if (Helper.Instance.isAdmin(login, password))
            {
                if (_context.Users.Any(u => u.Login == newLogin))
                    return BadRequest("Данный логин уже используется");

                if (!Helper.Instance.IsLettersAndNumbers(newLogin))
                    return BadRequest("Данный логин невалиден");

                if (!Helper.Instance.IsLettersAndNumbers(newPassword))
                    return BadRequest("Данный пароль невалиден");

                if (!Helper.Instance.IsLetters(newName))
                    return BadRequest("Данное имя невалидно");

                switch (newGender)
                {
                    case 0:
                        break;
                    case 1:
                        break;
                    case 2:
                        break;
                    case null:
                        break;
                    default:
                        return BadRequest("Пол не валиден");
                }


                var newUser = new User
                {
                    Guid = Guid.NewGuid(),
                    Login = newLogin,
                    Password = newPassword,
                    Name = newName,
                    Gender = newGender ?? 2,
                    Birthday = newBirthday ?? null,
                    Admin = isAdmin,
                    CreatedOn = DateTime.Now,
                    CreatedBy = login,
                    ModifiedBy = login,
                    ModifiedOn = DateTime.Now
                };

                _context.Users.Add(newUser);
                await _context.SaveChangesAsync();

                return Ok("Пользователь добавлен");

            }
            return StatusCode(403);
        }

        [HttpPut("UpdatePersonalData")]
        public async Task<ActionResult> UpdatePersonalData(string login, string password, string? loginForUpdate, string? newName, int? newGender, DateTime? newBirthday)
        {
            var log = loginForUpdate ?? login;

            var currentUser = _context.Users.SingleOrDefault(u => u.Login == log);

            if (currentUser != null )
            {
                if (_context.Users.Any(u => u.Login == login && u.Password == password && (u.Admin || currentUser.RevokedOn == null)))
                {
                    if (!Helper.Instance.IsLetters(newName))
                        return BadRequest("Данное имя невалидно");

                    switch (newGender)
                    {
                        case 0:
                            break;
                        case 1:
                            break;
                        case 2:
                            break;
                        case null:
                            break;
                        default:
                            return BadRequest("Пол не валиден");
                    }

                    currentUser.Name = newName ?? currentUser.Name;
                    currentUser.Gender = newGender ?? currentUser.Gender;
                    currentUser.Birthday = newBirthday ?? currentUser.Birthday;
                    currentUser.ModifiedBy = log;
                    currentUser.ModifiedOn = DateTime.Now;

                    await _context.SaveChangesAsync();

                    return Ok("Данные изменены");
                }

                return StatusCode(403);
            }

            return StatusCode(404);
        }

        [HttpPut("UpdatePassword")]
        public async Task<ActionResult> UpdatePassword(string login, string password, string newPassword, string? loginForUpdate)
        {
            var log = loginForUpdate ?? login;

            var currentUser = _context.Users.SingleOrDefault(u => u.Login == log);

            if (currentUser != null)
            {
                if (Helper.Instance.isAdmin(login, password) && loginForUpdate != null)
                {
                    if (!Helper.Instance.IsLettersAndNumbers(newPassword))
                        return BadRequest("Данный пароль невалиден");

                    currentUser.Password = newPassword;
                    currentUser.ModifiedBy = log;
                    currentUser.ModifiedOn = DateTime.Now;

                    await _context.SaveChangesAsync();

                    return Ok("Данные изменены");
                }
                else if (currentUser.RevokedOn == null && loginForUpdate == null)
                {
                    currentUser.Password = newPassword;
                    currentUser.ModifiedBy = log;
                    currentUser.ModifiedOn = DateTime.Now;

                    await _context.SaveChangesAsync();

                    return Ok("Данные изменены");
                }

                return StatusCode(403);
            }

            return StatusCode(404);
        }

        [HttpPut("UpdateLogin")]
        public async Task<ActionResult> UpdateLogin(string login, string password, string newLogin, string? loginForUpdate)
        {
            if (_context.Users.Any(u => u.Login == newLogin))
                return BadRequest("Данный логин уже существует");

            var log = loginForUpdate ?? login;

            var currentUser = _context.Users.SingleOrDefault(u => u.Login == log);

            if (currentUser != null)
            {
                if (!Helper.Instance.IsLettersAndNumbers(newLogin))
                    return BadRequest("Данный логин невалиден");

                if (Helper.Instance.isAdmin(log, password) && loginForUpdate != null)
                {
                    currentUser.Login = newLogin;
                    currentUser.ModifiedBy = log;
                    currentUser.ModifiedOn = DateTime.Now;

                    await _context.SaveChangesAsync();

                    return Ok("Данные изменены");
                }
                else if (loginForUpdate == null && currentUser.RevokedOn == null)
                {
                    currentUser.Login = newLogin;
                    currentUser.ModifiedBy = log;
                    currentUser.ModifiedOn = DateTime.Now;

                    await _context.SaveChangesAsync();

                    return Ok("Данные изменены");
                }
                
                return StatusCode(403);
            }

            return StatusCode(404);
        }

        [HttpPut("RestoreUser")]
        public async Task<ActionResult> RestoreUser(string login, string password, string loginForRestore)
        {
            var currentUser = _context.Users.SingleOrDefault(u => u.Login == loginForRestore);

            if (currentUser == null)
                return BadRequest("Данного пользователя не существует");

            if (Helper.Instance.isAdmin(login, password))
            {
                currentUser.RevokedBy = null;
                currentUser.RevokedOn = null;
                currentUser.ModifiedBy = login;
                currentUser.ModifiedOn = DateTime.Now;

                await _context.SaveChangesAsync();

                return Ok("Пользователь востановлен");
            }

            return StatusCode(403);
        }

        [HttpDelete("DeleteUser")]
        public async Task<ActionResult> DeleteUser(string login, string password, string loginForDelete, bool isSoft)
        {
            var currentUser = _context.Users.SingleOrDefault(u => u.Login == loginForDelete);

            if (currentUser == null)
                return BadRequest("Данного пользователя не существует");

            if (Helper.Instance.isAdmin(login, password))
            {
                if (isSoft)
                {
                    currentUser.RevokedBy = login;
                    currentUser.RevokedOn = DateTime.Now;
                    currentUser.ModifiedBy = login;
                    currentUser.ModifiedOn = DateTime.Now;

                    await _context.SaveChangesAsync();

                    return Ok("Пользователь удален(Мягкое удаление)");
                }

                _context.Users.Remove(currentUser);
                await _context.SaveChangesAsync();

                return Ok("Пользователь удален(Жесткое удаление)");
            }

            return StatusCode(403);
        }
    }
}

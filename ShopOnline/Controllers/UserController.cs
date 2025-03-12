using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopOnline.DataBaseContext;
using ShopOnline.Models;
using System.Globalization;
using System.Xml.Serialization;

namespace ShopOnline.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowOrigin")]
    public class UserController : ControllerBase
    {
        public readonly DBaseContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public UserController(DBaseContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            var principalStruct = await _context.PrincipalStructs.FindAsync(1);

            var users = await _context.Users
                              .Where(u => u.Email != "xxxxxx")
                              .ToListAsync();

            foreach (var user in users)
            {
                int userIndex = Array.FindIndex(principalStruct.Data9, x => x == user.Id.ToString());

                if (userIndex != -1 && userIndex < principalStruct.Data4.Length)
                {
                    user.Rol = principalStruct.Data4[userIndex];
                }
            }

            return users;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        [HttpGet("getDiscount/{userId}")]
        public async Task<ActionResult<string>> GetUserDate(int userId)
        {
            var principalStruct = await _context.PrincipalStructs.FindAsync(1);
            if (principalStruct == null)
            {
                return NotFound("No se encontró la estructura principal.");
            }

            int userIndex = Array.FindIndex(principalStruct.Data9, x => x == userId.ToString());

            if (userIndex == -1 || userIndex >= principalStruct.Substructs.Length)
            {
                return NotFound("Usuario no encontrado en la estructura.");
            }

            string dateString = principalStruct.Substructs[userIndex].Date;

            if (string.IsNullOrWhiteSpace(dateString))
            {
                return BadRequest("La fecha del usuario está vacía o es inválida.");
            }

            string dateFormat = "M/d/yyyy h:mm:ss tt";

            DateTime userDate;
            bool isDateValid = DateTime.TryParseExact(dateString, dateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out userDate);

            if (!isDateValid)
            {
                return BadRequest("El formato de la fecha es incorrecto.");
            }

            DateTime threeMonthsAgo = DateTime.UtcNow.AddMonths(-3);

            string discount = userDate <= threeMonthsAgo
                ? principalStruct.Data6[1] // Cliente antiguo
                : principalStruct.Data6[0]; // Cliente nuevo

            return Ok(discount);
        }


        [HttpGet("{name}/{password}")]
        public async Task<IActionResult> Login(string name, string password)
        {
            var principalStruct = await _context.PrincipalStructs.FindAsync(1);
            User user = _context.Users.First(u => u.CI == name && u.Password == password || u.Email == name && u.Password == password);
            if (user != null)
            {
                int userIndex = Array.FindIndex(principalStruct.Data9, x => x == user.Id.ToString());

                if (userIndex != -1 && userIndex < principalStruct.Data4.Length)
                {
                    user.Rol = principalStruct.Data4[userIndex];
                }
                return Ok(new { Message = "Inicio de sesión exitoso", User = user });
            }
            else
            {
                return Unauthorized(new { Message = "Credenciales inválidas" });
            }
        }

        [HttpPost("GenerateUsername")]
        public async Task<ActionResult<string>> GenerateUsername()
        {
            string baseUsername = "Temp";

            string generatedUsername;
            do
            {
                generatedUsername = $"{baseUsername}{(new Random().Next(0, 1000))}";
            } while (await _context.Users.AnyAsync(u => u.Name == generatedUsername));

            return Ok(generatedUsername);
        }

        [HttpPost("signup/{userName}/{email}")]
        public async Task<ActionResult<User>> PostUserGoogle(string userName, string email)
        {
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (existingUser != null)
            {
                return Ok(existingUser);
            }

            string code;
            do
            {
                code = (new Random().Next(10000, 100000)).ToString();
            } while (_context.Users.Any(u => u.AccountNumber == code));

            var user = new User { Name = userName, Email = email };
            user.AccountNumber = code;
            user.Password = "";
            user.CI = "";
            user.City = "";
            _context.Users.Add(user);
            await SaveChangesAndSerializeAsync();

            if (user.CI != "000000")
            {
                await UpdatePrincipalStruct(user);
            }

            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
        }

        [HttpPost]
        public async Task<ActionResult<User>> PostUser(User user)
        {
            if (_context.Users.Any(u => u.Email == user.Email && !string.IsNullOrEmpty(u.Email)))
            {
                return BadRequest("El correo electrónico ya están en uso.");
            }

            string code;
            do
            {
                code = (new Random().Next(10000, 100000)).ToString();
            } while (_context.Users.Any(u => u.AccountNumber == code));

            user.AccountNumber = code;
            _context.Users.Add(user);
            await SaveChangesAndSerializeAsync();

            if (user.CI != "000000")
            {
                await UpdatePrincipalStruct(user);
            }

            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
        }

        [HttpGet("UpdatePrincipalStruct")]
        public async Task UpdatePrincipalStruct(User user)
        {
            PrincipalStruct principalStruct = _context.PrincipalStructs.FirstOrDefault();
            if (principalStruct != null)
            {
                await IncrementArrayFields(principalStruct);

                principalStruct.Opiniones[principalStruct.Opiniones.Length - 1].UserId = user.Id.ToString();

                string[] newData9 = new string[principalStruct.Data9.Length + 1];
                Array.Copy(principalStruct.Data9, newData9, principalStruct.Data9.Length);
                newData9[newData9.Length - 1] = user.Id.ToString();
                principalStruct.Data9 = newData9;

                await _context.SaveChangesAsync();
                await PStructSerializeToXmlAsync();
            }
        }
        private async Task IncrementArrayFields(PrincipalStruct principalStruct)
        {
            var opiniones = principalStruct.Opiniones;
            Array.Resize(ref opiniones, principalStruct.Opiniones.Length + 1);
            principalStruct.Opiniones = opiniones;
            principalStruct.Opiniones[principalStruct.Opiniones.Length - 1] = new Opiniones();

            var substructs = principalStruct.Substructs;
            Array.Resize(ref substructs, principalStruct.Substructs.Length + 1);
            principalStruct.Substructs = substructs;
            principalStruct.Substructs[principalStruct.Substructs.Length - 1] = new Substruct();

            for (int i = 0; i < 18; i++)
            {
                if (i == 5 || i == 6 || i == 7 || i == 8 || i == 0 || i == 1 || i == 2) continue;
                var propertyName = $"Data{i + 1}";
                var property = principalStruct.GetType().GetProperty(propertyName);
                if (property != null)
                {
                    if (i > 8 && i < 12)
                    {
                        var array = (double[])property.GetValue(principalStruct);
                        Array.Resize(ref array, array.Length + 1);
                        property.SetValue(principalStruct, array);
                    }
                    else if (i > 11)
                    {
                        var array = (int[])property.GetValue(principalStruct);
                        Array.Resize(ref array, array.Length + 1);
                        property.SetValue(principalStruct, array);
                    }
                    else if (i < 8)
                    {
                        var array = (string[])property.GetValue(principalStruct);
                        Array.Resize(ref array, array.Length + 1);
                        property.SetValue(principalStruct, array);
                    }
                }
            }
        }

        [HttpGet("updateRol/{rol}/{id}")]
        public async Task<IActionResult> PutUser(string rol, int id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            var principalStruct = await _context.PrincipalStructs.FindAsync(1);
            int userIndex = Array.FindIndex(principalStruct.Data9, x => x == id.ToString());
            principalStruct.Data4[userIndex] = rol;

            _context.Entry(principalStruct).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, User user)
        {
            if (id != user.Id)
            {
                return BadRequest();
            }

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await SaveChangesAndSerializeAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await SaveChangesAndSerializeAsync();

            return NoContent();
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }

        private async Task SaveChangesAndSerializeAsync()
        {
            await _context.SaveChangesAsync();
            await SerializeToXmlAsync();
        }

        private async Task SerializeToXmlAsync()
        {
            var users = await _context.Users.ToListAsync();

            XmlSerializer serializer = new XmlSerializer(typeof(List<User>));
            var uploadsFolder = Path.Combine(_webHostEnvironment.ContentRootPath, "xmluploads");
            Directory.CreateDirectory(uploadsFolder);
            var filePath = Path.Combine(uploadsFolder, "users.xml");

            using (var writer = new StreamWriter(filePath))
            {
                serializer.Serialize(writer, users);
            }
        }

        private async Task PStructSerializeToXmlAsync()
        {
            var principalStructs = await _context.PrincipalStructs.ToListAsync();

            XmlSerializer serializer = new XmlSerializer(typeof(List<PrincipalStruct>));
            var uploadsFolder = Path.Combine(_webHostEnvironment.ContentRootPath, "xmluploads");
            Directory.CreateDirectory(uploadsFolder);
            var filePath = Path.Combine(uploadsFolder, "PrincipalStructs.xml");

            using (var writer = new StreamWriter(filePath))
            {
                serializer.Serialize(writer, principalStructs);
            }
        }
    }
}
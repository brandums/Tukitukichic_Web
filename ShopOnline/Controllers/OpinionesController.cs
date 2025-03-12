using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ShopOnline.DataBaseContext;
using ShopOnline.Models;

namespace ShopOnline.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowOrigin")]
    public class OpinionesController : ControllerBase
    {
        public readonly DBaseContext _context;
        private UserController _userController;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public OpinionesController(DBaseContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _userController = new UserController(_context, webHostEnvironment);
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet("{codigo}")]
        public async Task<ActionResult<List<Opiniones>>> GetOpinions(int codigo)
        {
            var principalStruct = await _context.PrincipalStructs.FindAsync(1);
            if (principalStruct == null)
            {
                return NotFound("PrincipalStruct not found.");
            }

            var opinionesEncontradas = new List<Opiniones>();

            foreach (var userOpinion in principalStruct.Opiniones)
            {
                var productIndices = userOpinion.Codigo
                    .Select((code, index) => new { code, index })
                    .Where(pair => pair.code == codigo.ToString())
                    .Select(pair => pair.index)
                    .ToList();

                if (userOpinion.UserId != null)
                {
                    var user = await _context.Users.FindAsync(int.Parse(userOpinion.UserId));
                    if (productIndices.Any())
                    {
                        foreach (var index in productIndices)
                        {
                            var opinion = new Opiniones
                            {
                                UserId = userOpinion.UserId,
                                Codigo = new string[] { userOpinion.Codigo[index] },
                                Calificacion = new string[] { userOpinion.Calificacion[index] },
                                Comentario = new string[] { userOpinion.Comentario[index] },
                                Data1 = new string[] { user.Name },
                                Data4 = new string[] { userOpinion.Data4[index] }
                            };

                            opinionesEncontradas.Add(opinion);
                        }
                    }
                }
            }

            if (!opinionesEncontradas.Any())
            {
                return NotFound("No opinions found for the specified code.");
            }

            return Ok(opinionesEncontradas);
        }


        [HttpGet("{codigo}/{userId}")]
        public async Task<ActionResult<Opiniones>> GetOpinion(int codigo, int userId)
        {
            var principalStruct = await _context.PrincipalStructs.FindAsync(1);
            if (principalStruct == null)
            {
                return NotFound();
            }

            var userIndex = Array.FindIndex(principalStruct.Opiniones, x => x.UserId == userId.ToString());
            if (userIndex == -1)
            {
                return NotFound("User not found");
            }
            var productIndex = Array.FindIndex(principalStruct.Opiniones[userIndex].Codigo, x => x == codigo.ToString());
            if (productIndex == -1)
            {
                return NotFound("Product not found for this user");
            }

            var calificacion = principalStruct.Opiniones[userIndex].Calificacion[productIndex];
            var comentario = principalStruct.Opiniones[userIndex].Comentario[productIndex];

            var opinion = new Opiniones
            {
                UserId = userId.ToString(),
                Codigo = new string[] { codigo.ToString() },
                Calificacion = new string[] { calificacion },
                Comentario = new string[] { comentario }
            };

            return opinion;
        }

        [HttpGet("enableComments/{codigo}/{userId}")]
        public async Task<ActionResult<bool>> enableComments(int codigo, int userId)
        {
            var isEnable = false;
            var principalStruct = await _context.PrincipalStructs.FindAsync(1);
            List<Substruct> purchasedProducts = new List<Substruct>();
            var userIndex = Array.FindIndex(principalStruct.Data9, x => x == userId.ToString());
            if (userIndex == -1)
            {
                return NotFound("User not found");
            }

            if (principalStruct.Data5[userIndex] != null || principalStruct.Data5[userIndex] != "")
            {
                var substructs = JsonConvert.DeserializeObject<List<Substruct>>(principalStruct.Data5[userIndex]);
                foreach (var substruct in substructs)
                {
                    foreach (var codigoX in substruct.Codigo)
                    {
                        if (codigoX == codigo.ToString())
                        {
                            isEnable = true;
                        }
                    }
                }
            }
            return isEnable;
        }

        [HttpGet("getBots")]
        public async Task<ActionResult<IEnumerable<User>>> GetBots()
        {
            var users = await _context.Users
                                      .Where(u => u.Email == "xxxxxx")
                                      .ToListAsync();

            if (users == null || users.Count == 0)
            {
                return NotFound("No users found with the specified email.");
            }

            return Ok(users);
        }

        [HttpPost("createUser/{name}")]
        public async Task<ActionResult<User>> PostUser(string name)
        {
            var user = new User();
            string code;
            do
            {
                code = (new Random().Next(10000, 100000)).ToString();
            } while (_context.Users.Any(u => u.AccountNumber == code));

            user.AccountNumber = code;
            user.Name = name;
            user.Email = "xxxxxx";
            user.Password = "xxxxxx";
            user.CI = "xxxxxx";
            user.City = "xxxxxx";
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            await _userController.UpdatePrincipalStruct(user);

            return Ok();
        }

        [HttpPost("AddOrUpdateOpinion")]
        public async Task<ActionResult> AddOrUpdateOpinion(int codigo, int userId, int calificacion, string comentario)
        {
            var principalStruct = await _context.PrincipalStructs.FindAsync(1);
            if (principalStruct == null)
            {
                return NotFound();
            }

            var userIndex = Array.FindIndex(principalStruct.Opiniones, x => x.UserId == userId.ToString());
            if (userIndex == -1)
            {
                return NotFound("User not found");
            }

            var productIndex = Array.FindIndex(principalStruct.Opiniones[userIndex].Codigo, x => x == codigo.ToString());
            if (productIndex == -1)
            {
                principalStruct.Opiniones[userIndex].Codigo = principalStruct.Opiniones[userIndex].Codigo.Append(codigo.ToString()).ToArray();
                principalStruct.Opiniones[userIndex].Calificacion = principalStruct.Opiniones[userIndex].Calificacion.Append(calificacion.ToString()).ToArray();
                principalStruct.Opiniones[userIndex].Comentario = principalStruct.Opiniones[userIndex].Comentario.Append(comentario).ToArray();
                principalStruct.Opiniones[userIndex].Data1 = principalStruct.Opiniones[userIndex].Data1.Append(string.Empty).ToArray();
                principalStruct.Opiniones[userIndex].Data2 = principalStruct.Opiniones[userIndex].Data2.Append(string.Empty).ToArray();
                principalStruct.Opiniones[userIndex].Data3 = principalStruct.Opiniones[userIndex].Data3.Append(string.Empty).ToArray();
                principalStruct.Opiniones[userIndex].Data4 = principalStruct.Opiniones[userIndex].Data4.Append(string.Empty).ToArray();
            }
            else
            {
                principalStruct.Opiniones[userIndex].Calificacion[productIndex] = calificacion.ToString();
                principalStruct.Opiniones[userIndex].Comentario[productIndex] = comentario;
                principalStruct.Opiniones[userIndex].Data4[productIndex] = DateTime.Now.ToString();
            }

            await _context.SaveChangesAsync();
            return Ok("Opinion added/updated successfully.");
        }

        [HttpPost("BotOpinion/{userId}/{codigo}/{calificacion}/{comentario}/{fecha}")]
        public async Task<ActionResult> UpdateRating(int userId, int codigo, int calificacion, string comentario, string fecha)
        {
            var principalStruct = await _context.PrincipalStructs.FindAsync(1);
            if (principalStruct == null)
            {
                return NotFound();
            }

            var userIndex = Array.FindIndex(principalStruct.Opiniones, x => x.UserId == userId.ToString());
            if (userIndex == -1)
            {
                return NotFound("User not found");
            }

            var productIndex = Array.FindIndex(principalStruct.Opiniones[userIndex].Codigo, x => x == codigo.ToString());
            if (productIndex == -1)
            {
                principalStruct.Opiniones[userIndex].Codigo = principalStruct.Opiniones[userIndex].Codigo.Append(codigo.ToString()).ToArray();
                principalStruct.Opiniones[userIndex].Calificacion = principalStruct.Opiniones[userIndex].Calificacion.Append(calificacion.ToString()).ToArray();
                principalStruct.Opiniones[userIndex].Comentario = principalStruct.Opiniones[userIndex].Comentario.Append(comentario).ToArray();
                principalStruct.Opiniones[userIndex].Data1 = principalStruct.Opiniones[userIndex].Data1.Append(string.Empty).ToArray();
                principalStruct.Opiniones[userIndex].Data2 = principalStruct.Opiniones[userIndex].Data2.Append(string.Empty).ToArray();
                principalStruct.Opiniones[userIndex].Data3 = principalStruct.Opiniones[userIndex].Data3.Append(string.Empty).ToArray();
                principalStruct.Opiniones[userIndex].Data4 = principalStruct.Opiniones[userIndex].Data4.Append(fecha).ToArray();
            }
            else
            {
                principalStruct.Opiniones[userIndex].Calificacion[productIndex] = calificacion.ToString();
                principalStruct.Opiniones[userIndex].Comentario[productIndex] = comentario;
                principalStruct.Opiniones[userIndex].Data4[productIndex] = fecha;
            }

            _context.Entry(principalStruct).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return Ok("Rating updated successfully.");
        }

        [HttpPost("UpdateRating/{codigo}/{userId}/{calificacion}")]
        public async Task<ActionResult> UpdateRating(int codigo, int userId, int calificacion)
        {
            var principalStruct = await _context.PrincipalStructs.FindAsync(1);
            if (principalStruct == null)
            {
                return NotFound();
            }

            var userIndex = Array.FindIndex(principalStruct.Opiniones, x => x.UserId == userId.ToString());
            if (userIndex == -1)
            {
                return NotFound("User not found");
            }

            var productIndex = Array.FindIndex(principalStruct.Opiniones[userIndex].Codigo, x => x == codigo.ToString());
            if (productIndex == -1)
            {
                principalStruct.Opiniones[userIndex].Codigo = principalStruct.Opiniones[userIndex].Codigo.Append(codigo.ToString()).ToArray();
                principalStruct.Opiniones[userIndex].Calificacion = principalStruct.Opiniones[userIndex].Calificacion.Append(calificacion.ToString()).ToArray();
                principalStruct.Opiniones[userIndex].Comentario = principalStruct.Opiniones[userIndex].Comentario.Append(string.Empty).ToArray();
                principalStruct.Opiniones[userIndex].Data1 = principalStruct.Opiniones[userIndex].Data1.Append(string.Empty).ToArray();
                principalStruct.Opiniones[userIndex].Data2 = principalStruct.Opiniones[userIndex].Data2.Append(string.Empty).ToArray();
                principalStruct.Opiniones[userIndex].Data3 = principalStruct.Opiniones[userIndex].Data3.Append(string.Empty).ToArray();
                principalStruct.Opiniones[userIndex].Data4 = principalStruct.Opiniones[userIndex].Data4.Append(string.Empty).ToArray();
            }
            else
            {
                principalStruct.Opiniones[userIndex].Calificacion[productIndex] = calificacion.ToString();
            }

            _context.Entry(principalStruct).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return Ok("Rating updated successfully.");
        }

        [HttpPost("UpdateComment/{codigo}/{userId}/{comentario}")]
        public async Task<ActionResult> UpdateComment(int codigo, int userId, string comentario)
        {
            var principalStruct = await _context.PrincipalStructs.FindAsync(1);
            if (principalStruct == null)
            {
                return NotFound();
            }

            var userIndex = Array.FindIndex(principalStruct.Opiniones, x => x.UserId == userId.ToString());
            if (userIndex == -1)
            {
                return NotFound("User not found");
            }

            var productIndex = Array.FindIndex(principalStruct.Opiniones[userIndex].Codigo, x => x == codigo.ToString());
            if (productIndex == -1)
            {
                principalStruct.Opiniones[userIndex].Codigo = principalStruct.Opiniones[userIndex].Codigo.Append(codigo.ToString()).ToArray();
                principalStruct.Opiniones[userIndex].Comentario = principalStruct.Opiniones[userIndex].Comentario.Append(comentario).ToArray();
                principalStruct.Opiniones[userIndex].Calificacion = principalStruct.Opiniones[userIndex].Calificacion.Append(string.Empty).ToArray();
                principalStruct.Opiniones[userIndex].Data1 = principalStruct.Opiniones[userIndex].Data1.Append(string.Empty).ToArray();
                principalStruct.Opiniones[userIndex].Data2 = principalStruct.Opiniones[userIndex].Data2.Append(string.Empty).ToArray();
                principalStruct.Opiniones[userIndex].Data3 = principalStruct.Opiniones[userIndex].Data3.Append(string.Empty).ToArray();
                principalStruct.Opiniones[userIndex].Data4 = principalStruct.Opiniones[userIndex].Data4.Append(DateTime.Now.ToString("yyyy-MM-dd")).ToArray();
            }
            else
            {
                principalStruct.Opiniones[userIndex].Comentario[productIndex] = comentario;
                principalStruct.Opiniones[userIndex].Data4[productIndex] = DateTime.Now.ToString("yyyy-MM-dd");
            }

            _context.Entry(principalStruct).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return Ok("Comment updated successfully.");
        }

    }
}

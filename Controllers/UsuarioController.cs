using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SistemaInventario.DTO;
using SistemaInventario.Models;
using SistemaInventario.Repositories;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SistemaInventario.Controllers
{
    [Route("api/[controller]")]
    [Authorize(Roles = "Administrador")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IConfiguration _config;

        public UsuarioController(IUsuarioRepository usuarioRepository, IConfiguration config)
        {
            _usuarioRepository = usuarioRepository;
            _config = config;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UsuarioDto>>> GetAll()
        {
            var usuarios = await _usuarioRepository.GetAll();

            var usuariosList = usuarios.Select(u => new UsuarioDto
            {
                Id = u.IdUsuario,
                Nombre = u.Nombre,
                Apellido = u.Apellido,
                Correo = u.Correo,
                Estado = u.Estado,
                FechaRegistro = u.FechaRegistro.ToString("yyyy-MM-dd HH:mm:ss"),
                RolNombre = u.IdRolNavigation != null ? u.IdRolNavigation.Nombre : "Sin rol",
            }).ToList();

            return Ok(usuariosList);
        }

        [HttpGet("id")]
        public async Task<ActionResult<UsuarioDto>> GetId(int id)
        {
            var usuarios = await _usuarioRepository.GetUsuarioID(id);
            if (usuarios == null)
            {
                BadRequest("El id ingresado no existe o no contiene datos");
            }
            var usuariosList =  new UsuarioDto
            {
                Id = usuarios.IdUsuario,
                Nombre = usuarios.Nombre,
                Apellido = usuarios.Apellido,
                Correo = usuarios.Correo,
                Estado = usuarios.Estado,
                FechaRegistro = usuarios.FechaRegistro.ToString("yyyy-MM-dd HH:mm:ss"),
                RolNombre = usuarios.IdRolNavigation != null ? usuarios.IdRolNavigation.Nombre : "Sin rol",
            };

            return Ok(usuariosList);
        }

        [HttpPost("registro")]
        public async Task<ActionResult> Add([FromBody] UsuarioInsertDto usuario)
        {
            if (usuario == null)
            {
                return BadRequest();
            }
            var nameExists = await _usuarioRepository.NameExistAsync(usuario.Correo);

            if (nameExists == true)
            {
                return NotFound("El correo del usuario ya esat registrado.");
            }

            // encriptacion
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(usuario.Contraseña);

            var usuarioEntity = new Usuario
            {
                Nombre = usuario.Nombre,
                Apellido = usuario.Apellido,
                Correo = usuario.Correo,
                PasswordHash = passwordHash,
                FechaRegistro = DateTime.Now,
                IdRol = usuario.IdRol ?? 1, // Asignar un valor predeterminado si IdRol es nulo
            };
            await _usuarioRepository.AddAsync(usuarioEntity);
            return CreatedAtAction(nameof(GetAll), new { id = usuarioEntity.IdUsuario }, usuario);
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult> Login([FromBody] UsuarioLoginDto usuario)
        {
            var user = await _usuarioRepository.ExistUsauarioAsync(usuario.Correo);
            if (user == null || !BCrypt.Net.BCrypt.Verify(usuario.Contraseña, user.PasswordHash))
            {
                return Unauthorized("El correo electrónico o la contraseña son incorrectos.");
            }

            // Generar token JWT
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.IdUsuario.ToString()),
                new Claim(ClaimTypes.Email, user.Correo),
                new Claim(ClaimTypes.Role, user.IdRolNavigation?.Nombre ?? "Sin rol")
            };

            // leemos la clave secreta que gaurdamos en el json
            var keyString = _config.GetSection("Jwt:Key").Value;
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString!));
            // Configuramos los algoritmos de firmado para que nadie pueda falsificarlo
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            // Armamos el token final con una duración (por ejemplo, expira en 1 día)
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = creds
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            // 4. Le devolvemos la llave digital (Token) al Frontend en un JSON
            return Ok(new
            {
                mensaje = "Login exitoso",
                token = tokenHandler.WriteToken(token)
            });
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, [FromBody] UsuarioUpdateDto usuario)
        {
            if(id == usuario.Id)
            {
                BadRequest("El id de la cabecera no coincide con el id del cuerpo");
            }
            
            var existingUsuario = await _usuarioRepository.UsuarioExistAsync(id);
            if (existingUsuario == null)
            {
                return NotFound("El usuario no existe.");
            }
            existingUsuario.Nombre = usuario.Nombre;
            existingUsuario.Apellido = usuario.Apellido;
            existingUsuario.Correo = usuario.Correo;
            existingUsuario.PasswordHash = usuario.Contraseña ?? existingUsuario.PasswordHash;
            existingUsuario.IdRol = usuario.IdRol ?? existingUsuario.IdRol;
            if (!string.IsNullOrEmpty(usuario.Contraseña))
            {
                // encriptacion
                string passwordHash = BCrypt.Net.BCrypt.HashPassword(usuario.Contraseña);
                existingUsuario.PasswordHash = passwordHash;
            }
            await _usuarioRepository.UpdateAsync(existingUsuario);
            return NoContent();
        }

        [HttpPut("Estado")]
        public async Task<ActionResult> Update(int id, [FromBody] bool estado)
        {
            if (id <= 0)
            {
                BadRequest("El id es no valido");
            }

            var existingUsuario = await _usuarioRepository.UsuarioExistAsync(id);
            if (existingUsuario == null)
            {
                return NotFound("El usuario no existe.");
            }
            existingUsuario.Estado = estado;

            await _usuarioRepository.UpdateAsync(existingUsuario);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var existingUsuario = await _usuarioRepository.UsuarioExistAsync(id);
            if (existingUsuario == null)
            {
                return NotFound("El usuario no existe.");
            }
            await _usuarioRepository.DeleteAsync(existingUsuario);
            return NoContent();
        }
    }
}

using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using DatingApp.API.Data;
using DatingApp.API.Dtos;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DatingApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _repo;
        private readonly IConfiguration _config;

        public AuthController(IAuthRepository repo, IConfiguration config)
        {
            _repo = repo;
            _config = config;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserForRegisterDto userForRegisterDto ){

            // validazione parametri user
            userForRegisterDto.Username = userForRegisterDto.Username.ToLower();

            // se già esiste si restituisce una bad request
            if(await _repo.UserExits(userForRegisterDto.Username))
                return BadRequest("User already exists!");

            // si crea un oggetto utente con solo la username 
            var userToCreate = new User(){
                Username = userForRegisterDto.Username
            };

            // si richiama la procedura di registrazione dove vengono generate 
            // la coppia di hash della password e della salt
            var createdUser = await _repo.Register(userToCreate,  userForRegisterDto.Password);

            return StatusCode(201);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserForLoginDto userForLoginDto){

            //throw new Exception("Computer says no");

            // recupero l'utente dal db
            var userFromRepo = await _repo.Login(userForLoginDto.Username.ToLower(), userForLoginDto.Password);

            // se non presente vuole dire che non è autorizzato
            if(userFromRepo == null)
                return Unauthorized();

            // aggiungo i claims ossia le proprietà che voglio portarmi dietro nel token jwt
            // in questo caso l'id e lo username
            var claims = new[] {
                new Claim(ClaimTypes.NameIdentifier, userFromRepo.Id.ToString()),
                new Claim(ClaimTypes.Name, userFromRepo.Username)
            };

            // creo la chiave simmetrica dalla chiave segreta  memorizzata nella sezione Appsettings del file appsettings.json
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value));

            // la cripto con l'algoritmo HmacSha512
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            // Creo il token JWT conenente i claims, la data di scaadenza e la chiave simmetrica criptata
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };

            // creo l'handler per la generazione del token
            var tokenHandler = new JwtSecurityTokenHandler();
            // genero il token
            var token = tokenHandler.CreateToken(tokenDescriptor);

            // restituisco il token generato al client
            return Ok(new {
                token = tokenHandler.WriteToken(token)
            });
        }
    }
}
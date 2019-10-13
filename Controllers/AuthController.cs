using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.Api.Dtos;
using DatingApp.API.Data;
using DatingApp.API.Dtos;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace DatingApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [EnableCors("AllowSpecificOrigin")]
    public class AuthController : Controller
    {
        private readonly IAuthRepository _repo;
        private readonly IMapper _mapper;

        public AuthController(IAuthRepository repo, IMapper mapper)
        {
            this._mapper = mapper;
            this._repo = repo;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody]UserForRegisterDto userForRegisterDto)
        {
            if (!string.IsNullOrEmpty(userForRegisterDto.Username))
                userForRegisterDto.Username = userForRegisterDto.Username.ToLower();
            if (await _repo.UserExist(userForRegisterDto.Username))
                ModelState.AddModelError("Username", "Username already exist");

            //validate request
            if (!ModelState.IsValid)
                return BadRequest(ModelState);


            var userToCreate = new Users
            {
                Username = userForRegisterDto.Username
            };

            await _repo.Register(userToCreate, userForRegisterDto.Password);

            return StatusCode(201);

        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody]UserForLoginDto userForLoginDto)
        {
            try
            {
                Users userFromRepo = await _repo.Login(userForLoginDto.Username.ToLower(), userForLoginDto.Password);

                if (userFromRepo == null)
                    return Unauthorized();

                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes("super secret key");

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.NameIdentifier,userFromRepo.Id.ToString()),
                        new Claim(ClaimTypes.Name,userFromRepo.Username)
                    }
                    ),
                    Expires = DateTime.Now.AddDays(1),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512Signature)
                };
                var token = tokenHandler.CreateToken(tokenDescriptor);
                var tokenString = tokenHandler.WriteToken(token);

                var user = _mapper.Map<UserForListDto>(userFromRepo);
                return Ok(new { tokenString, user });
            }
            catch (Exception)
            {
                return StatusCode(500, "It is wrong login apply.");
            }
        }

    }
}
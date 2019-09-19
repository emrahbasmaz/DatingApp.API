using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.Api.Data;
using DatingApp.Api.Dtos;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [EnableCors("AllowSpecificOrigin")]
    public class UsersController : ControllerBase
    {
        private readonly IDatingRepository _datingRepository;
        private readonly IMapper _mapper;

        public UsersController(IDatingRepository datingRepository,
                                IMapper mapper)
        {
            this._datingRepository = datingRepository;
            this._mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult> GetUsers()
        {
            try
            {
                IEnumerable<Users> users = await _datingRepository.GetUsers();

                var usersToReturn = _mapper.Map<IEnumerable<Users>, List<UserForListDto>>(users);
                return Ok(usersToReturn);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetUser(int id)
        {
            try
            {
                var user = await _datingRepository.GetUser(id);
                var userToReturn = _mapper.Map<UserForDetailedDto>(user);
                return Ok(userToReturn);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UserForUpdateDto userForUpdateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var currentUser = await _datingRepository.GetUser(userForUpdateDto.Id);
                var userFromRepo = await _datingRepository.GetUser(id);

                if (userFromRepo == null)
                {
                    return NotFound($"Could not found user with an ID if {id}");
                }
                if (currentUser != userFromRepo)
                {
                    return Unauthorized();
                }

                _mapper.Map(userForUpdateDto, userFromRepo);

                if (await _datingRepository.SaveAll())
                {
                    return NoContent();
                }
                throw new Exception($"Updating user {id} failed during save operation");
            }
            catch (Exception ex)
            {
                throw new Exception($"Updating user {id} failed during save operation...Message:" + ex.Message);
            }
        }
    }
}
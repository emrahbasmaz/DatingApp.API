using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.Api.Data;
using DatingApp.Api.Dtos;
using DatingApp.Api.Extension;
using DatingApp.Api.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [EnableCors("AllowSpecificOrigin")]
    public class MessagesController : ControllerBase
    {
        private readonly IDatingRepository datingRepository;
        private readonly IMapper mapper;

        public MessagesController(IDatingRepository datingRepository, IMapper mapper)
        {
            this.datingRepository = datingRepository;
            this.mapper = mapper;
        }

        [HttpGet("{userId}/{id}", Name = "GetMessage")]
        public async Task<IActionResult> GetMessage(int userId, int id)
        {
            var messageFromRepo = await datingRepository.GetMessage(id);

            if (messageFromRepo == null)
                return NotFound();

            return Ok(messageFromRepo);
        }

        [HttpGet(Name = "GetMessageForUser")]
        public async Task<IActionResult> GetMessageForUser([FromQuery]MessageParams messageParams)
        {
            var messageFromRepo = await datingRepository.GetMessagesForUser(messageParams);

            var messages = mapper.Map<IEnumerable<MessageToReturnDto>>(messageFromRepo);

            Response.AddPagination(messageFromRepo.CurrentPage, messageFromRepo.PageSize,
                                    messageFromRepo.TotalCount, messageFromRepo.TotalPages);

            return Ok(messages);
        }

        [HttpPost("{userId}", Name = "CreateMessage")]
        public async Task<IActionResult> CreateMessage(int userId, [FromBody] MessagesForCreationDto messagesForCreationDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            messagesForCreationDto.SenderId = userId;
            var recipient = await datingRepository.GetUser(messagesForCreationDto.RecipientId);

            if (recipient == null)
            {
                return BadRequest("Recipient Could not Found...");
            }

            var message = mapper.Map<Message>(messagesForCreationDto);
            datingRepository.Add(message);

            var messageToReturn = mapper.Map<MessagesForCreationDto>(message);

            if (await datingRepository.SaveAll())
            {
                return CreatedAtRoute("GetMessage", new { id = message.Id }, message);
            }

            throw new Exception("Creating a message operation failed ...");
        }


        [HttpGet("thread/{userId}/{id}")]
        public async Task<IActionResult> GetMessageThread(int userId, int id)
        {
            var userFromRepo = await datingRepository.GetUser(userId);
            if (userFromRepo == null)
            {
                return NotFound($"Could not found user with an ID if {id}");
            }

            var messagedFromRepo = await datingRepository.GetMessageThread(userId, id);

            var messageThread = mapper.Map<IEnumerable<MessageToReturnDto>>(messagedFromRepo);

            return Ok(messageThread);

        }
    }
}
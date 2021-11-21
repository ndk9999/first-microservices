using System.Collections.Generic;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlatformService.AsyncDataServices;
using PlatformService.Data;
using PlatformService.Dtos;
using PlatformService.Models;
using PlatformService.SyncDataServices.Http;

namespace PlatformService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlatformsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IPlatformRepository _platformRepo;
        private readonly ICommandDataClient _cmdClient;
        private readonly IMessageBusClient _msgBusClient;

        public PlatformsController(IMapper mapper, IPlatformRepository platformRepo, ICommandDataClient cmdClient, IMessageBusClient msgBusClient)
        {
            _mapper = mapper;
            _platformRepo = platformRepo;
            _cmdClient = cmdClient;
            _msgBusClient = msgBusClient;
        }

        [HttpGet]
        public ActionResult<IEnumerable<PlatformRecord>> GetAll()
        {
            var platforms = _platformRepo.GetAll();

            return Ok(_mapper.Map<IEnumerable<PlatformRecord>>(platforms));
        }

        [HttpGet("{id}", Name = "GetPlatformById")]
        public ActionResult<PlatformRecord> GetPlatform(int id)
        {
            var platform = _platformRepo.GetById(id);

            if (platform == null) return NotFound();

            return Ok(_mapper.Map<PlatformRecord>(platform));
        }

        [HttpPost]
        public async Task<ActionResult<PlatformRecord>> CreatePlatform(PlatformCreateModel model)
        {
            if (!ModelState.IsValid) return BadRequest();

            var platform = _mapper.Map<Platform>(model);
            _platformRepo.Add(platform);
            _platformRepo.SaveChanges();

            var record = _mapper.Map<PlatformRecord>(platform);

            // Send message synchronously
            try
            {
                await _cmdClient.SendPlatformToCommand(record);
            }
            catch (System.Exception ex)
            {
                Console.WriteLine($"--> Could not send synchronously: {ex.Message}");
            }

            // Send message asynchronously
            try
            {
                var message = _mapper.Map<PlatformPublishedMessage>(record);
                message.Event = "Platform_Published";

                _msgBusClient.PublishNewPlatform(message);
            }
            catch (System.Exception ex)
            {
                Console.WriteLine($"--> Could not send asynchronously: {ex.Message}");
            }

            return CreatedAtRoute("GetPlatformById", new {record.Id}, record);
        }
    }
}
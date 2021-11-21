using AutoMapper;
using CommandService.Data;
using CommandService.Dtos;
using CommandService.Models;
using Microsoft.AspNetCore.Mvc;

namespace CommandService.Controllers
{
    [ApiController]
    [Route("api/c/platforms/{platformId}/[controller]")]
    public class CommandsController : ControllerBase
    {
        private readonly IMapper _mapper;

        private readonly ICommandRepository _commandRepo;

        public CommandsController(IMapper mapper, ICommandRepository commandRepo)
        {
            _mapper = mapper;
            _commandRepo = commandRepo;
        }

        [HttpGet]
        public ActionResult<IEnumerable<CommandRecord>> GetCommandsForPlatForm(int platformId)
        {
            Console.WriteLine($"--> Hit GetCommandsForPlatForm: {platformId}");

            if (!_commandRepo.PlatformExists(platformId))
            {
                return NotFound();
            }

            var commands = _commandRepo.GetCommands(platformId);
            return Ok(_mapper.Map<IEnumerable<CommandRecord>>(commands));
        }

        [HttpGet("{commandId}", Name = "GetOneCommand")]
        public ActionResult<CommandRecord> GetOneCommand(int platformId, int commandId)
        {
            Console.WriteLine($"--> Hit GetOneCommand: {platformId} - {commandId}");

            if (!_commandRepo.PlatformExists(platformId))
            {
                return NotFound();
            }

            var command = _commandRepo.GetCommand(platformId, commandId);
            if (command == null) return NotFound();

            return Ok(_mapper.Map<CommandRecord>(command));
        }

        [HttpPost]
        public ActionResult<CommandRecord> CreateCommand(int platformId, CommandCreateModel model)
        {
            Console.WriteLine($"--> Hit CreateCommand: {platformId}");

            if (!_commandRepo.PlatformExists(platformId)) return BadRequest();

            var command = _mapper.Map<Command>(model);
            _commandRepo.CreateCommand(platformId, command);
            _commandRepo.SaveChanges();

            var record = _mapper.Map<CommandRecord>(command);
            return CreatedAtRoute(nameof(GetOneCommand), new{platformId, commandId = record.Id}, record);
        }
    }
}
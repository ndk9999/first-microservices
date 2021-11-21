using AutoMapper;
using CommandService.Data;
using CommandService.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace CommandService.Controllers
{
    [ApiController]
    [Route("api/c/[controller]")]
    public class PlatformsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ICommandRepository _commandRepo;

        public PlatformsController(IMapper mapper, ICommandRepository commandRepo)
        {
            _mapper = mapper;
            _commandRepo = commandRepo;
        }

        [HttpGet]
        public ActionResult<IEnumerable<PlatformRecord>> GetAll()
        {
            Console.WriteLine("--> Get Platforms from CommandService");

            var platforms = _commandRepo.GetAllPlatforms();
            
            return Ok(_mapper.Map<IEnumerable<PlatformRecord>>(platforms));
        }

        [HttpPost]
        public ActionResult TestInboundConnection()
        {
            Console.WriteLine("--> Inbound POST # Command Service");
            return Ok("Inbound test from Platforms Controller");
        }
    }
}
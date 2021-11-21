using AutoMapper;
using CommandService.Models;
using Grpc.Net.Client;
using PlatformService;

namespace CommandService.SyncDataServices.Grpc
{
    public class PlatformDataClient : IPlatformDataClient
    {
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        public PlatformDataClient(IConfiguration configuration, IMapper mapper)
        {
            _configuration = configuration;
            _mapper = mapper;
        }

        public IEnumerable<Platform> GetAllPlatforms()
        {
            var grpcUrl = _configuration["GrpcPlatform"];
            Console.WriteLine($"--> Calling gRPC service: {grpcUrl}");

            var channel = GrpcChannel.ForAddress(grpcUrl);
            var client = new GrpcPlatform.GrpcPlatformClient(channel);
            var request = new GetAllRequest();

            try
            {
                var reply = client.GetAllPlatforms(request);
                return _mapper.Map<IEnumerable<Platform>>(reply.Platform);
            }
            catch (System.Exception ex)
            {
                Console.WriteLine($"--> Cound not call gRPC server: {ex.Message}");
                return null;
            }
        }
    }
}
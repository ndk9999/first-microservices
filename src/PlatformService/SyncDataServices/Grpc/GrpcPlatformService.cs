using AutoMapper;
using Grpc.Core;
using PlatformService.Data;

namespace PlatformService.SyncDataServices.Grpc
{
    public class GrpcPlatformService : GrpcPlatform.GrpcPlatformBase
    {
        private readonly IMapper _mapper;
        private readonly IPlatformRepository _platformRepo;

        public GrpcPlatformService(IMapper mapper, IPlatformRepository platformRepo)
        {
            _mapper = mapper;
            _platformRepo = platformRepo;
        }

        public override Task<PlatformResponse> GetAllPlatforms(GetAllRequest request, ServerCallContext context)
        {
            var response = new PlatformResponse();
            var platforms = _platformRepo.GetAll();

            foreach (var item in platforms)
            {
                response.Platform.Add(_mapper.Map<GrpcPlatformModel>(item));
            }

            return Task.FromResult(response);
        }
    }
}
using System.Text.Json;
using AutoMapper;
using CommandService.Data;
using CommandService.Dtos;
using CommandService.Models;

namespace CommandService.EventProcessing
{
    public class EventProcessor : IEventProcessor
    {
        private readonly IServiceScopeFactory _serviceFactory;
        private readonly IMapper _mapper;

        public EventProcessor(IServiceScopeFactory serviceFactory, IMapper mapper)
        {
            _serviceFactory = serviceFactory;
            _mapper = mapper;
        }

        public void ProcessEvent(string message)
        {
            var eventType = DetermineEvent(message);

            switch (eventType)
            {
                case EventType.PlatformPublished:
                    AddPlatform(message);
                    break;
                default:
                    break;
            }
        }

        private EventType DetermineEvent(string notificationMessage)
        {
            Console.WriteLine("--> Determining event type ...");
            
            var eventMessage = JsonSerializer.Deserialize<GenericEventMessage>(notificationMessage);
            switch (eventMessage.Event)
            {
                case "Platform_Published":
                    Console.WriteLine("--> Platform Published event detected");
                    return EventType.PlatformPublished;
                default:
                    Console.WriteLine("--> Could not determine the event type");
                    return EventType.Undetermined;
            }
        }

        private void AddPlatform(string eventMessage)
        {
            using (var scope = _serviceFactory.CreateScope())
            {
                var repo = scope.ServiceProvider.GetRequiredService<ICommandRepository>();
                var publishedMessage = JsonSerializer.Deserialize<PlatformPublishedMessage>(eventMessage);
                var platform = _mapper.Map<Platform>(publishedMessage);

                try
                {
                    if (repo.ExternalPlatformExists(platform.ExternalId))
                    {
                        Console.WriteLine("--> Platform already exists");
                    }
                    else
                    {
                        repo.CreatePlatform(platform);
                        repo.SaveChanges();

                        Console.WriteLine("--> Platform is added");
                    }
                }
                catch (System.Exception ex)
                {
                    Console.WriteLine($"--> Could not add platform to database: {ex.Message}");
                }
            }
        }
    }

    public enum EventType
    {
        PlatformPublished,
        Undetermined
    }
}
using CommandService.Models;
using CommandService.SyncDataServices.Grpc;

namespace CommandService.Data
{
    public static class DataSeeder
    {
        public static void PrepareData(this IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                var commandRepo =serviceScope.ServiceProvider.GetService<ICommandRepository>(); 
                var grpcClient = serviceScope.ServiceProvider.GetService<IPlatformDataClient>();
                var platforms = grpcClient.GetAllPlatforms();

                SeedData(commandRepo, platforms);
            }
        }

        private static void SeedData(ICommandRepository repository, IEnumerable<Platform> platforms)
        {
            Console.WriteLine("--> Seeding new platforms ...");

            foreach (var item in platforms)
            {
                if (repository.ExternalPlatformExists(item.ExternalId)) continue;

                repository.CreatePlatform(item);
                repository.SaveChanges();
            }
        }
    }
}
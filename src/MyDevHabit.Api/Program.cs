using MyDevHabit.Api;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

await builder.AddControllers()
             .AddErrorHandling()
             .AddDatabase()
             .AddObservability()
             .AddApplicationServices()
             .AddAuthenticationServices()
             .Build()
             .ConfigureApplication()
             .RunAsync();

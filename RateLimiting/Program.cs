
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;

namespace RateLimiting
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

			// Configure Rate Limiter
			builder.Services.AddRateLimiter(options =>
			{
				options.AddFixedWindowLimiter("fixed", limiterOptions =>
				{
					limiterOptions.PermitLimit = 2;               // Allow only 2 requests
					limiterOptions.Window = TimeSpan.FromSeconds(10); // Every 10 seconds
					limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
					limiterOptions.QueueLimit = 0;               // No queued requests
				});

				options.OnRejected = async (context, cancellationToken) =>
				{
					context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
					await context.HttpContext.Response.WriteAsync("Too many requests. Please try again later.", cancellationToken);
				};
			});

			var app = builder.Build();


			// Enable Middleware
			app.UseRateLimiter();

			// Configure the HTTP request pipeline.
			if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}

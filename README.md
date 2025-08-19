🚦 ASP.NET Core Web API Rate Limiting Example
Welcome to the ASP.NET Core Web API Rate Limiting Example! This project showcases how to implement rate limiting in an ASP.NET Core Web API using the direct builder pattern introduced in .NET. The rate limiting is applied to specific endpoints (actions) rather than the entire controller, providing fine-grained control over request handling.

📌 What is Rate Limiting?
Rate limiting is a powerful technique to control the number of requests a client can make to an API within a specified time window. It helps to:

Prevent abuse (e.g., spamming endpoints).
Protect server resources from overuse.
Enhance application security and performance.

This project demonstrates a fixed window rate limiter, allowing a set number of requests within a defined time period.

🛠️ Project Setup
Follow these steps to set up and run the project locally.
Prerequisites

.NET SDK (version 6.0 or later)
A code editor like Visual Studio Code or Visual Studio
Basic knowledge of ASP.NET Core and C#

1. Add Rate Limiting Services
In Program.cs, the rate limiter is configured as follows:
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();

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

app.MapControllers();

app.Run();

2. Apply Rate Limiting to an Endpoint
In the WeatherForecastController.cs, the rate limiter is applied to a specific action using the [EnableRateLimiting] attribute:
[HttpGet(Name = "GetWeatherForecast")]
[EnableRateLimiting("fixed")]
public IEnumerable<WeatherForecast> Get()
{
    return Enumerable.Range(1, 5).Select(index => new WeatherForecast
    {
        Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
        TemperatureC = Random.Shared.Next(-20, 55),
        Summary = Summaries[Random.Shared.Next(Summaries.Length)]
    })
    .ToArray();
}

3. Run the Project
To run the project, execute the following command in the terminal:
dotnet run

4. Test the API
Send HTTP GET requests to the following endpoint:
https://localhost:5001/WeatherForecast

Expected Behavior:

✅ First 2 requests within 10 seconds → 200 OK
❌ Additional requests within the same 10-second window → 429 Too Many Requests


📋 Summary

Rate Limiting Configuration: Configured in Program.cs using AddRateLimiter.
Selective Application: Applied to the Get action in WeatherForecastController using [EnableRateLimiting("fixed")].
Behavior: Requests exceeding the limit (2 requests per 10 seconds) return a 429 Too Many Requests response.


🔑 Key Points

Use [EnableRateLimiting("policyName")] to apply rate limiting to specific actions.
For global rate limiting, apply the middleware without attributes.
Supported rate limiting policies include:
FixedWindow: Limits requests in a fixed time window.
SlidingWindow: Limits requests in a sliding time window.
TokenBucket: Limits based on token availability.
Concurrency: Limits concurrent requests.




🚀 Try It Out!

Clone the repository:git clone <repository-url>


Navigate to the project directory:cd <project-directory>


Run the application:dotnet run


Use a tool like Postman or curl to test the /WeatherForecast endpoint.


📚 Resources

ASP.NET Core Rate Limiting Documentation
Microsoft Learn: Rate Limiting in .NET
GitHub: ASP.NET Core


🌟 Contributing
Feel free to fork this repository, submit issues, or create pull requests to improve this example! Contributions are always welcome.

📄 License
This project is licensed under the MIT License.

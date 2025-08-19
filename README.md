# 🚦 ASP.NET Core Web API Rate Limiting Example

Welcome to the **ASP.NET Core Web API Rate Limiting Example**!  
This project showcases how to implement **rate limiting** in an ASP.NET Core Web API using the direct builder pattern introduced in .NET.  
The rate limiting is applied to **specific endpoints (actions)** rather than the entire controller, providing fine-grained control over request handling.

---

## 📌 What is Rate Limiting?

Rate limiting is a powerful technique to control the number of requests a client can make to an API within a specified time window.  
It helps to:

- ✅ Prevent abuse (e.g., spamming endpoints)  
- ✅ Protect server resources from overuse  
- ✅ Enhance application security and performance  

This project demonstrates a **fixed window rate limiter**, allowing a set number of requests within a defined time period.

---

## 🛠️ Project Setup

Follow these steps to set up and run the project locally.

### 🔧 Prerequisites
- .NET SDK (version **6.0 or later**)  
- A code editor like **Visual Studio Code** or **Visual Studio**  
- Basic knowledge of **ASP.NET Core** and **C#**

---

### 1️⃣ Add Rate Limiting Services

In **Program.cs**, the rate limiter is configured as follows:

```csharp
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
        limiterOptions.PermitLimit = 2; // Allow only 2 requests
        limiterOptions.Window = TimeSpan.FromSeconds(10); // Every 10 seconds
        limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        limiterOptions.QueueLimit = 0; // No queued requests
    });

    options.OnRejected = async (context, cancellationToken) =>
    {
        context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        await context.HttpContext.Response.WriteAsync(
            "Too many requests. Please try again later.", cancellationToken);
    };
});

var app = builder.Build();

// Enable Middleware
app.UseRateLimiter();

app.MapControllers();

app.Run();

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

## ✅ Expected Behavior

- First **2 requests within 10 seconds** → `200 OK`  
- Additional requests in the same window → `429 Too Many Requests`  

---

## 📋 Summary

- **Rate Limiting Configuration** → Configured in `Program.cs` using `AddRateLimiter`.  
- **Selective Application** → Applied to the `Get` action in `WeatherForecastController` using `[EnableRateLimiting("fixed")]`.  
- **Behavior** → More than **2 requests in 10 seconds** → returns `429 Too Many Requests`.  

---

## 🔑 Key Points

- Use `[EnableRateLimiting("policyName")]` to apply rate limiting to **specific actions**.  
- For **global rate limiting**, apply the middleware without attributes.  
- Supported rate limiting policies include:  
  - **FixedWindow** → Limits requests in a fixed time window.  
  - **SlidingWindow** → Limits requests in a sliding time window.  
  - **TokenBucket** → Limits based on token availability.  
  - **Concurrency** → Limits concurrent requests.  

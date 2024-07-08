using CORNCafePOSAPI.Authorization;
using CORNCafePOSAPICommon;

var builder = WebApplication.CreateBuilder(args);

Cache.DBConnectionString = builder.Configuration["DBConnectionString"];
Cache.Jwt_Key = builder.Configuration["Jwt:Key"];
Cache.Jwt_Issuer = builder.Configuration["Jwt:Issuer"];
Cache.Jwt_ExpiryDurationInMin = Convert.ToInt32(builder.Configuration["Jwt:ExpiryDurationInMin"]);

builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = null;
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors();
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
    });
builder.Services.AddScoped<IJwtUtils, JwtUtils>();
builder.Services.AddAWSLambdaHosting(LambdaEventSource.RestApi);

var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.MapSwagger();
app.UseSwaggerUI();
app.UseCors(x => x
    .AllowAnyMethod()
    .AllowAnyHeader()
    .SetIsOriginAllowed(origin => true)
    .AllowCredentials());
app.UseMiddleware<JwtMiddleware>();
app.MapControllers();

app.Run();

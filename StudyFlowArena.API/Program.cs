using StudyFlowArena.API.Data;
using StudyFlowArena.API.Interfaces;
using StudyFlowArena.API.Repositories;
using StudyFlowArena.API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<TokenBlackListService>();


builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
);


builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<TokenService>();


var jwtSettings = builder.Configuration.GetSection("Jwt");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true, 
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]))
        };
        
        options.Events = new JwtBearerEvents
        {   
            OnTokenValidated = context =>
            {
                var tokenService = context.HttpContext.RequestServices.GetRequiredService<TokenBlackListService>();
                var token = context.SecurityToken as JwtSecurityToken;

                if (token != null && tokenService.IsTokenBlacklisted(token.RawData))
                {
                    context.Fail("This token has been blacklisted.");
                }

                return Task.CompletedTask;
            }
        };    
    });

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// IMPORTANT: Add Authentication BEFORE Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

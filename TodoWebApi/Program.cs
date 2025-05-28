using TodoWebApi.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using TodoWebApi.Services;
using TodoWebApi.Services.Interfaces;
using System.Text;
using Microsoft.AspNetCore.Identity;
using TodoWebApi.Models;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TodoWebApi.Filters;
using Microsoft.OpenApi.Models;


namespace TodoWebApi
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // JWT config
            var jwtSettings = builder.Configuration.GetSection("Jwt");
            var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]);

            builder.Services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme; // Sets the default way of authentication, in this case JWT
                    // This is what happens when user tries to reach an protected resource without authentication
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme; 
                    // In simple terms - Each time user tries to access a controller/method with [Authorize], we will 

                })
            .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false; // Should be true in production to ensure tokens are only accepted via HTTPS
                    options.SaveToken = true; // Saves token in HttpContext after user has been successfully authenticated
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true, // Validate the signature - so attackers can't create their own token
                        IssuerSigningKey = new SymmetricSecurityKey(key), // The key that was used to sign token
                        ValidateIssuer = true, // Validates that the issuer('iss') is what we expect it to be
                        ValidateAudience = true, // Validates that the audience('aud') is what we expecte it to be
                        ValidIssuer = jwtSettings["Issuer"], // The expected issues value in token
                        ValidAudience = jwtSettings["Audience"], // Expected audience value in token
                        ClockSkew = TimeSpan.Zero
                    };
                });

            builder.Services.AddAuthorization(); // For when we want to use [Authorize] requirement on Controller methods

            // Add swagger services
            builder.Services.AddEndpointsApiExplorer(); // Minimal APIs and better endpoint discovery
            builder.Services.AddSwaggerGen(); // Swagger generator service

            // Swagger - JWT bearer token authorization
            builder.Services.AddSwaggerGen(c =>
            {
                c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Description = @"JWT Authorization header using the Bearer scheme.  
                      Enter 'Bearer' [space] and then your token in the text input below.
                      Example: 'Bearer eyJhbGciOiJIUzI1...'",
                    Name = "Authorization",
                    In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                    Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement()
                {
                    {
                        new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                        {
                            Reference = new Microsoft.OpenApi.Models.OpenApiReference
                            {
                                Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = Microsoft.OpenApi.Models.ParameterLocation.Header,

                        },
                        new List<string>()
                    }
                });
            });

            // Swagger - add title/version info
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Todo Web API",
                    Version = "v1",
                    Description = "Learning project with JWT, EF Core, Swagger, etc."
                });
            });


            // Adds Identity to the container. 
            // Because it injects cookie-based authentication as its default auth scheme and we don't want to use neither that or it's JWT functionality right now, we comment it away and can easily transition to it later on.
            //builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
            //    .AddEntityFrameworkStores<AppDbContext>();



            // Add services to the container.


            // FluentValidation
            builder.Services.AddValidatorsFromAssemblyContaining<Program>();
            // FluentValidation Filter
            builder.Services.AddControllers(options =>
            {
                options.Filters.Add<ValidationFilter>();
            });

            builder.Services.AddScoped<AuthService>();
            builder.Services.AddScoped<ITodoItemService, TodoItemService>();
            builder.Services.AddScoped<ICategoryService, CategoryService>();

            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            var app = builder.Build();

            // Seed the database, if empty.
            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                await SeedService.SeedAsync(dbContext);
            }

            // Only show swagger if in dev
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger(); // Middleware to generate swagger JSON endpoint
                app.UseSwaggerUI(); // Middleware to serve swagger UI - web page
            }


            // Configure the HTTP request pipeline.
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}

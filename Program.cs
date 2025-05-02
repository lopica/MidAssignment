
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MidAssignment.Domain;
using MidAssignment.Infrastructure;
using MidAssignment.Services;
using System.Text;
using MidAssignment.Middlewares;
using MidAssignment.Services.Interfaces;
using MidAssignment.Repositories;
using Microsoft.AspNetCore.Mvc;
using MidAssignment.DTOs;

namespace MidAssignment
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            string jwtKey = builder.Configuration["JwtSettings:PrivateKey"]!;
            int accessTokenExpiry = int.Parse(builder.Configuration["JwtSettings:AccessTokenExpirationMinutes"]!);
            int refreshTokenExpiry = int.Parse(builder.Configuration["JwtSettings:RefreshTokenExpirationDays"]!);
            string domainFrontend = builder.Configuration["DomainFrontend"]!;
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.SupportNonNullableReferenceTypes();
                options.SchemaFilter<DefaultValueSchemaFilter>();
                var xmlFilename = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
                options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));

            });
            builder.Services.AddRouting(options => options.LowercaseUrls = true);
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend", policy =>
                {
                    policy.WithOrigins(domainFrontend)
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials();
                });
            });
            builder.Services.AddScoped<IAuthServices, AuthServices>();
            builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            builder.Services.AddScoped<ICategoryServices, CategoryServices>();
            builder.Services.AddScoped<IBookServices, BookServices>();
            builder.Services.AddScoped<IBorrowingRequestServices, BorrowingRequestServices>();
            builder.Services.AddScoped<IApplicationUserServices, ApplicationUserServices>();
            builder.Services.AddSingleton<IJWTServices, JWTServices>();
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey!))
                    };

                    options.Events = new JwtBearerEvents
                    {
                        OnAuthenticationFailed = context =>
                        {
                            return Task.CompletedTask;
                        },
                        OnMessageReceived = context =>
                        {
                            var token = context.Request.Cookies["access_token"];
                            if (!string.IsNullOrEmpty(token))
                            {
                                context.Token = token;
                            }
                            return Task.CompletedTask;
                        }
                    };
                });

            builder.Services.AddAuthorization();
            builder.Services.AddControllers()
                .ConfigureApiBehaviorOptions(options =>
                {
                    options.InvalidModelStateResponseFactory = context =>
                    {
                        var errors = context.ModelState
                            .Where(x => x.Value?.Errors.Count > 0)
                            .SelectMany(x => x.Value!.Errors)
                            .Select(x => x.ErrorMessage)
                            .ToList();

                        var customResponse = new ErrorApplicationResponse(
                            StatusCodes.Status400BadRequest,
                            errors
                        );

                        return new BadRequestObjectResult(customResponse);
                    };
                });



            var app = builder.Build();
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                RoleInitializer.CreateRoles(services).GetAwaiter().GetResult();
            }
            app.UseExceptionHandler(errorApp =>
            {
                errorApp.Run(async context =>
                {
                    context.Response.StatusCode = 500;
                    context.Response.ContentType = "application/json";

                    var exceptionHandlerPathFeature = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerPathFeature>();

                    var errorMessage = exceptionHandlerPathFeature?.Error.Message ?? "An unexpected error occurred.";

                    var error = new ErrorApplicationResponse(
                        StatusCodes.Status500InternalServerError,
                        [errorMessage]
                    );

                    var errorJson = System.Text.Json.JsonSerializer.Serialize(error);

                    await context.Response.WriteAsync(errorJson);
                });
            });

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseHttpsRedirection();

            app.UseCors("AllowFrontend");
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }

    }
}

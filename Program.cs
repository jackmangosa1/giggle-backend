
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ServiceManagementAPI.Data;
using ServiceManagementAPI.Hubs;
using ServiceManagementAPI.Repositories.AuthRepository;
using ServiceManagementAPI.Repositories.ChatRepository;
using ServiceManagementAPI.Repositories.CustomerRepository;
using ServiceManagementAPI.Repositories.ProviderRepository;
using ServiceManagementAPI.Services.AuthService;
using ServiceManagementAPI.Services.ChatService;
using ServiceManagementAPI.Services.CustomerService;
using ServiceManagementAPI.Services.EmailService;
using ServiceManagementAPI.Services.ProviderService;
using ServiceManagementAPI.Utils;
using System.Text;

namespace ServiceManagementAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddDbContext<ServiceManagementDbContext>(options =>
               options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnectionString")));

            builder.Services.AddDbContext<ServiceManagementIdentityDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnectionString")));

            builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
                 {
                     options.Password.RequireDigit = false;
                     options.Password.RequiredLength = 6;
                     options.Password.RequireLowercase = false;
                     options.Password.RequireUppercase = false;
                     options.Password.RequireNonAlphanumeric = false;
                     options.SignIn.RequireConfirmedEmail = true;
                     options.Tokens.EmailConfirmationTokenProvider = TokenOptions.DefaultEmailProvider;
                 })
                .AddEntityFrameworkStores<ServiceManagementIdentityDbContext>()
                .AddDefaultTokenProviders();

            builder.Services.AddSingleton(x =>
               new BlobServiceClient(builder.Configuration.GetValue<string>("AzureBlobStorage:ConnectionString")));

            builder.Services.AddSingleton(x =>
            {
                var blobServiceClient = x.GetRequiredService<BlobServiceClient>();
                return blobServiceClient.GetBlobContainerClient("profile-pictures");
            });



            builder.Services.AddScoped<IAuthRepository, AuthRepository>();
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IEmailService, EmailService>();
            builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
            builder.Services.AddScoped<ICustomerService, CustomerService>();
            builder.Services.AddScoped<IProviderRepository, ProviderRepository>();
            builder.Services.AddScoped<IProviderService, ProviderService>();
            builder.Services.AddScoped<IChatRepository, ChatRepository>();
            builder.Services.AddScoped<IChatService, ChatService>();
            builder.Services.AddScoped<ChatService>();

            builder.Services.AddSingleton<BlobStorageUtil>();


            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
           .AddJwtBearer(options =>
           {
               options.TokenValidationParameters = new TokenValidationParameters
               {
                   ValidateIssuer = true,
                   ValidateAudience = true,
                   ValidateLifetime = true,
                   ValidateIssuerSigningKey = true,
                   ValidIssuer = builder.Configuration["Jwt:Issuer"],
                   ValidAudience = builder.Configuration["Jwt:Audience"],
                   IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
               };
           });

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOrigins", policy =>
                {
                    policy.WithOrigins("http://localhost:3000")
                          .AllowAnyMethod()
                          .AllowAnyHeader()
                          .AllowCredentials();
                });
            });


            builder.Services.AddSignalR();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseCors("AllowAllOrigins");

            app.UseAuthorization();


            app.MapControllers();

            app.MapHub<NotificationHub>("/notificationHub").RequireCors("AllowAllOrigins");
            app.MapHub<ChatHub>("/chatHub").RequireCors("AllowAllOrigins");
            app.MapHub<ProviderStatusHub>("/ProviderStatusHub").RequireCors("AllowAllOrigins");

            app.Run();
        }
    }
}

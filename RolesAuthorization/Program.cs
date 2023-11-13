
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RolesAuthorization.AuthorizeMiddleware;
using RolesAuthorization.Data;
using RolesAuthorization.Helper;
using RolesAuthorization.Permission;
using RolesAuthorization.Repository;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using RolesAuthorization.Constant;


namespace RolesAuthorization
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddDbContext<ApplicationDbContext>
            (options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
            builder.Services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();
            // configure the Identity 
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(2);
                options.Lockout.MaxFailedAccessAttempts = 5;

            }).AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders()
                .AddRoles<IdentityRole>();

            builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
            builder.Services.AddScoped<ISubCategoryRepository, SubCategoryRepository>();
            builder.Services.AddScoped<IProductRepository, ProductRepository>();


            builder.Services.AddAutoMapper(typeof(Program));

            builder.Services.AddScoped<JWTService>();


            // Adding Authentication  
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })

                        // Adding Jwt Bearer  
                        .AddJwtBearer(options =>
                        {
                            options.SaveToken = true;
                            options.RequireHttpsMetadata = false;
                            options.TokenValidationParameters = new TokenValidationParameters()
                            {
                                ValidateIssuer = true,
                                ValidateAudience = true,
                                ValidAudience = builder.Configuration["JWTKey:ValidAudience"],
                                ValidIssuer = builder.Configuration["JWTKey:ValidIssuer"],
                                ClockSkew = TimeSpan.Zero,
                                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWTKey:Secret"]))
                            };
                        });

            // policy for authorize

            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("ViewProductPolicy", policy =>
                    policy.RequireClaim("Permission", Permissions.Products.View));

                options.AddPolicy("CreateProductPolicy", policy =>
                    policy.RequireClaim("Permission", Permissions.Products.Create));

                options.AddPolicy("EditProductPolicy", policy =>
                    policy.RequireClaim("Permission", Permissions.Products.Edit));

                options.AddPolicy("DeleteProductPolicy", policy =>
                    policy.RequireClaim("Permission", Permissions.Products.Delete));
            });



            //builder.Services.AddAuthorization(options =>
            //{
            //    options.AddPolicy("AdminOnly", policy => policy.Requirements.Add(new AdminRequirement()));
            //});

            //   builder.Services.AddSingleton<IAuthorizationHandler, AdminAuthorizationHandler>();


            builder.Services.AddControllers().AddNewtonsoftJson(options =>
            options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
            );
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            //Add this before app.UseMvc
            app.Use(async (context, next) =>
            {
                if (!context.User.Identities.Any(i => i.IsAuthenticated))
                {
                    //Assign all anonymous users the same generic identity, which is authenticated
                    context.User = new ClaimsPrincipal(new GenericIdentity("anonymous"));
                }
                await next.Invoke();

            });
            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();
            

            app.Run();
        }
    }
}
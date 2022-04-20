using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CMSClientApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpClient("MyClient", options =>
            {
                options.BaseAddress = new Uri("http://localhost:44385/api/");
            });

            //services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("CMSConnectionStrings")));
            //services.AddIdentity<IdentityUser, IdentityRole>()
            //        .AddEntityFrameworkStores<ApplicationDbContext>();

            //services.AddAuthentication(o =>
            //{
            //    o.DefaultScheme = "Application";
            //    o.DefaultSignInScheme = "External";
            //})
            //.AddCookie("Application")
            //.AddCookie("External")
            //.AddGoogle(o =>
            //{
            //    o.ClientId = "833315438537-8a499aojqs53aaum03dnltbt398nftuo.apps.googleusercontent.com";
            //    o.ClientSecret = "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";
            //});


            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;

            })
            .AddCookie(options =>
            {
                options.LoginPath = "/home/GoogleLogin";
            })
            .AddGoogle(options =>
             {
                 options.ClientId = "833315438537-8a499aojqs53aaum03dnltbt398nftuo.apps.googleusercontent.com";
                 options.ClientSecret = "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";
                 options.ClaimActions.MapJsonKey("urn:google:picture", "picture", "url");
                 options.SaveTokens = true;
               


                 //options.Events.OnCreatingTicket = ctx =>
                 //{
                 //    List<AuthenticationToken> tokens = ctx.Properties.GetTokens().ToList();

                 //    tokens.Add(new AuthenticationToken()
                 //    {
                 //        Name = "TicketCreated",
                 //        Value = DateTime.UtcNow.ToString()
                 //    });

                 //    ctx.Properties.StoreTokens(tokens);

                 //    return Task.CompletedTask;
                 //};


             });

            // });
            //.AddFacebook(options =>
            //{
            //    options.AppId = "1357419171336994";
            //    options.AppSecret = "6ee23c7d057edc77528b059d6054c977";
            //});
            services.AddControllersWithViews();
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(20);
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseSession();
            app.UseAuthentication();


            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}

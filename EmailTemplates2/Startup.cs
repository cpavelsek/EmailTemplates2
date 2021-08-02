using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmailTemplates2 {
	public class Startup {
		public Startup( IConfiguration configuration ) {
			Configuration = configuration;
		}

	public class NoDirectAccessAttribute : ActionFilterAttribute {
		public override void OnActionExecuting( ActionExecutingContext filterContext ) {
			var canAcess = false;

			// check the refer
			var referer = filterContext.HttpContext.Request.Headers[ "Referer" ].ToString();
			if ( !string.IsNullOrEmpty( referer ) ) {
				var rUri = new System.UriBuilder( referer ).Uri;
				var req = filterContext.HttpContext.Request;
				if ( req.Host.Host == rUri.Host && req.Host.Port == rUri.Port && req.Scheme == rUri.Scheme ) {
					canAcess = true;
				}
			}

			// ... check other requirements

			if ( !canAcess ) {
				filterContext.Result = new RedirectToRouteResult( new RouteValueDictionary( new { controller = "Home", action = "Error", area = "" } ) );
			}
		}
	}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices( IServiceCollection services ) {
			services.AddControllersWithViews();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure( IApplicationBuilder app, IWebHostEnvironment env ) {
			if ( env.IsDevelopment() ) {
				app.UseDeveloperExceptionPage();
			} else {
				app.UseExceptionHandler( "/Home/Error" );
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}
			app.UseHttpsRedirection();
			app.UseStaticFiles();

			app.UseRouting();

			app.UseAuthorization();

			app.UseEndpoints( endpoints => {

				endpoints.MapGet( "/welcome", async ( context ) => {
					context.Response.StatusCode = 404;
				} );

				endpoints.MapControllerRoute(
					name: "default",
					pattern: "{controller=Home}/{action=Index}/{id?}" );
			} );
		}
	}
}

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Azure.Identity;
using System;
using Azure.Security.KeyVault.Secrets;
using Azure;

namespace GameServer
{
	public class Startup
	{
		public static string ConnectionString;
		public static string VaultUri { get { return Environment.GetEnvironmentVariable("VaultUri"); } }

		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			var cred = new ChainedTokenCredential(new ManagedIdentityCredential(), new AzureCliCredential());
			var client = new SecretClient(new Uri(VaultUri), cred);

			Response<KeyVaultSecret> secret = client.GetSecret("ConnectionStrings--tcp--marketgame--database--windows--net--1433");
			ConnectionString = secret.Value.Value.ToString();

			services.AddControllers();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseHttpsRedirection();

			app.UseRouting();

			app.UseAuthorization();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
			});
		}
	}
}

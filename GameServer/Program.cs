using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using Azure.Identity;

namespace GameServer
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var host = new HostBuilder()
			 .ConfigureAppConfiguration((hostContext, builder) =>
			 {

			 })
			 .Build();

			CreateHostBuilder(args).Build().Run();
		}

		public static IHostBuilder CreateHostBuilder(string[] args) => Host.CreateDefaultBuilder(args).ConfigureAppConfiguration((context, config) =>
		{
			string uri = Startup.VaultUri;
			var cred = new ChainedTokenCredential(new ManagedIdentityCredential(), new AzureCliCredential());
			var keyVaultEndpoint = new Uri(uri);
			config.AddAzureKeyVault(keyVaultEndpoint, cred);
		})
		.ConfigureWebHostDefaults(webBuilder =>
		{
			webBuilder.UseStartup<Startup>();
		});
	}
}

using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using RawRabbit.Configuration;
using RawRabbit.Enrichers.GlobalExecutionId;
using RawRabbit.Enrichers.MessageContext;
using RawRabbit.Enrichers.MessageContext.Context;
using RawRabbit.Instantiation;
using MPS.PaternalMessenger.Messages;
using Serilog;
using RawRabbit;

namespace MPS.PaternalMessenger.Subscriber
{
	public class Program
	{
		private static IBusClient _client;

		public static void Main(string[] args)
		{
			System.Console.WriteLine("Waiting for your son. Please be patient.");

			RunAsync()
				.GetAwaiter()
				.GetResult();
		} 

		public static async Task RunAsync()
		{
			Log.Logger = new LoggerConfiguration()
				.WriteTo.LiterateConsole()
				.CreateLogger();

			_client = RawRabbitFactory.CreateSingleton(new RawRabbitOptions
			{
				ClientConfiguration = new ConfigurationBuilder()
					.SetBasePath(Directory.GetCurrentDirectory())
					.AddJsonFile("rawrabbit.json")
					.Build()
					.Get<RawRabbitConfiguration>(),
				Plugins = p => p
					.UseGlobalExecutionId()
					.UseMessageContext<MessageContext>()
			});




			await _client.SubscribeAsync<NameRequest, MessageContext>((receivedName, ctx) => 
				SendGreetingAsync(receivedName, ctx)
				);

			
		}

		private static Task<NameResponse> SendGreetingAsync(NameRequest request, MessageContext ctx)
		{
			System.Console.WriteLine(request.MessageTemplate.Replace("{Name}", request.Name));
			string greeting = $"Hello {request.Name}, I am your father!";
			System.Console.WriteLine(greeting);
			return Task.FromResult(new NameResponse
			{
				Greeting = greeting
			});
		}
	}
}


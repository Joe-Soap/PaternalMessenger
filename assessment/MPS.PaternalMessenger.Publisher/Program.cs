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

namespace MPS.PaternalMessenger.Publisher
{
	public class Program
	{
		private static IBusClient _client;

		public static void Main(string[] args)
		{
			// Get the user's Name
			System.Console.Write("Please enter your name> ");
			string currentName = System.Console.ReadLine();

			RunAsync(currentName)
				.GetAwaiter()
				.GetResult();
				
		}

		public static async Task RunAsync(string currentName)
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
			await _client.SubscribeAsync<NameResponse, MessageContext>((response, ctx) => GetNameResponse(response, ctx) );
			await _client.PublishAsync(new NameRequest { MessageTemplate = "Hello, my name is {Name}", Name = currentName });
			
			}

		public static Task<NameResponse> GetNameResponse(NameResponse nameResponse, MessageContext ctx)
		{
			System.Console.WriteLine(nameResponse.Greeting);
			return Task.FromResult(new NameResponse
			{ Greeting = nameResponse.Greeting });
		}
		
	}
}


using System.Text;
using Microsoft.Extensions.Logging;
using RepetierSharp;
using RepetierSharp.Util;
using RestSharp;
using Websocket.Client;

namespace RepetierSharpTester
{
    internal class Program
    {
        private static async Task Main()
        {
            try
            {
      
                using var factory = LoggerFactory.Create(builder =>
                {
                    builder.AddConsole();
                });
                var logger = factory.CreateLogger<RepetierConnection>();
                var builder = new RepetierConnection.RepetierConnectionBuilder()
                    .WithLogger(logger )
                //.WithCommandFilter("ping")
                //.WithEventFilter("temp")
                    .WithCommandFilter("ping")
                    .WithEventFilter("temp")
                    // .WithEventFilter("ping")
                    .WithSession("e630a6e6-b745-4a2f-b002-4b04034840bc")
                    .WithApiKey("lFajzG5d7wHlgP2wh4UI6IUKAbbdd49g")
                .WithWebsocketHost("ws://10.197.45.104/socket/?sess=lFajzG5d7wHlgP2wh4UI6IUKAbbdd49g&lang=de&apiKey=e630a6e6-b745-4a2f-b002-4b04034840bc")
                .WithWebsocketAuth("Repetier-Admin", "sfm_2020", true)
                    //.UseWebSocketClient(new WebsocketClient(new Uri("ws://10.197.45.104/socket/?sess=1234542135462354&lang=de&apiKey=e630a6e6-b745-4a2f-b002-4b04034840bc")))
                .UseRestClient(new RestClient("http://10.197.45.104"));

            var repetierConn = builder.Build();
            repetierConn.ConnectedAsync += eventArgs =>
            {
                Console.WriteLine("Connected");
                return Task.CompletedTask;
            };
            
            repetierConn.PermissionDeniedAsync += eventArgs =>
            {
                Console.WriteLine("PermDenied");
                return Task.CompletedTask;
            };

            repetierConn.LoginRequiredAsync += eventArgs =>
            {
                return Task.CompletedTask;
            };

            repetierConn.DisconnectedAsync += eventArgs =>
            {
                return Task.CompletedTask;
            };

            repetierConn.RepetierRequestSendAsync += eventArgs =>
            {
                Console.WriteLine(
                    $" ===> {eventArgs.Command.CallbackId} {eventArgs.Command.Action} {eventArgs.Command.Printer}");
                return Task.CompletedTask;
            };

            repetierConn.RepetierRequestFailedAsync += eventArgs =>
            {
                Console.WriteLine($" =X=> {eventArgs.Command.CallbackId} {eventArgs.Command.Action} {eventArgs.Command.Printer}");
                return Task.CompletedTask;
            };

            repetierConn.RepetierResponseReceivedAsync += eventArgs =>
            {
                Console.WriteLine($" <=== {eventArgs.CallbackId} {eventArgs.Command}");
                return Task.CompletedTask;
            };

            repetierConn.EventReceivedAsync += eventArgs =>
            {
                Console.WriteLine($" <=!= {eventArgs.EventName}");
                return Task.CompletedTask;
            };

            repetierConn.RawRepetierEventReceivedAsync += eventArgs =>
            {
                Console.WriteLine($" <=?= {eventArgs.EventName}");
                if ( eventArgs.EventName == "wifiChanged" ||eventArgs.EventName == "extruder1")
                {
                    Console.WriteLine($" ########### {Encoding.UTF8.GetString(eventArgs.EventPayload)}");
                }
                return Task.CompletedTask;
            };
            await repetierConn.Connect();

            while ( true )
            {
            }

            await repetierConn.Close();
            await Task.CompletedTask;
            }
            catch ( Exception e )
            {
                Console.Error.WriteLine(e); 
            }
         
        }
        
    }
}

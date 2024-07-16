using System.Net.Mime;
using RepetierSharp;
using RepetierSharp.Util;
using RestSharp;
using Websocket.Client;

namespace RepetierSharpTester;

class Program
{
    static async Task Main()
    {
        var builder = new RepetierConnection.RepetierConnectionBuilder()
            .WithApiKey("e630a6e6-b745-4a2f-b002-4b04034840bc") // TODO: 
            .ExcludePing()
            .SelectPrinter("EVOlizer")
            .UseWebSocketClient(new WebsocketClient(new Uri("ws://10.197.45.104/socket/?lang=de&apiKey=e630a6e6-b745-4a2f-b002-4b04034840bc")))
            .UseRestClient(new RestClient("http://10.197.45.104"))
            .UseRestOptions(new RestClientOptions() 
                {Authenticator = new RepetierApiKeyRequestHeaderAuthenticator("e630a6e6-b745-4a2f-b002-4b04034840bc")});

        var repetierConn = builder.Build();
        await repetierConn.Connect();
        
        repetierConn.ConnectedAsync += (eventArgs) =>
        {
            Console.WriteLine("Connected");
            return Task.CompletedTask;
        };
        
        repetierConn.LoginRequiredAsync += (eventArgs) =>
        {
            Console.WriteLine("Login Required");
            return Task.CompletedTask;
        };
        
        repetierConn.DisconnectedAsync += (eventArgs) =>
        {
            Console.WriteLine("Disconnected");
            Environment.Exit(0);
            return Task.CompletedTask;
        };
        
        repetierConn.RepetierRequestSendAsync += (eventArgs) =>
        {
            Console.WriteLine($" ===> {eventArgs.RepetierBaseRequest.CallbackId} {eventArgs.RepetierBaseRequest.Action} {eventArgs.RepetierBaseRequest.Printer}");
            return Task.CompletedTask;
        };
        
        repetierConn.RepetierRequestFailedAsync += (eventArgs) =>
        {
            Console.WriteLine($" =X=> {eventArgs.RepetierBaseRequest.CallbackId}");
            return Task.CompletedTask;
        };
        
        repetierConn.RepetierResponseReceivedAsync += (eventArgs) =>
        {
            Console.WriteLine($" <=== {eventArgs.CallbackId}");
            return Task.CompletedTask;
        };
        
        repetierConn.EventReceivedAsync += (eventArgs) =>
        {
            Console.WriteLine($" <=!= {eventArgs.EventName}");
            return Task.CompletedTask;
        };
        
        repetierConn.RawRepetierEventReceivedAsync += (eventArgs) =>
        {
            Console.WriteLine($" <=?= {eventArgs.EventName}");
            return Task.CompletedTask;
        };
        

        while ( true )
        {
            
        }
        
        repetierConn.Dispose();
    }
}

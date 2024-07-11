using RepetierSharp;
using RestSharp;
using Websocket.Client;

namespace RepetierSharpTester;

class Program
{
    static async Task Main(string[] args)
    {
        var builder = new RepetierConnection.RepetierConnectionBuilder()
            .WithApiKey("")
            .UseWebSocketClient(new WebsocketClient(new Uri("")))
            .UseRestClient(new RestClient(new Uri("")))
            .UseRestOptions(new RestClientOptions());

        var repetierConn = builder.Build();
        await repetierConn.Connect();
        
        repetierConn.ConnectedAsync += (eventArgs) =>
        {
            Console.WriteLine("Connected");
            return Task.CompletedTask;
        };
        
        repetierConn.DisconnectedAsync += (eventArgs) =>
        {
            Console.WriteLine("Disconnected");
            return Task.CompletedTask;
        };
        
        repetierConn.RepetierRequestSendAsync += (eventArgs) =>
        {
            Console.WriteLine($" ===> {eventArgs.RepetierBaseRequest.CallbackId}");
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
        

        while ( true )
        {
            await Task.Delay(1000);
        }
        
        repetierConn.Dispose();
    }
}

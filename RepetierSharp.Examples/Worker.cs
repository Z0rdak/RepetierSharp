using System.Text.Json;
using RepetierSharp;
using RepetierSharp.Control;
using RepetierSharp.Internal;
using RepetierSharp.Models;
using RepetierSharp.Models.Commands;
using RepetierSharp.Models.Events;
using RepetierSharp.Models.Responses;
using RepetierSharp.Util;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

/// <summary>
/// TODO: move ListModels, ListJobs, etc into remoteprinter
/// TODO: move printer events into remote printer?
/// </summary>
public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly RepetierConnection _repetierCon;
    private readonly List<IRemotePrinter> _remotePrinters = new();
    
    public Worker(ILogger<Worker> logger)
    {
        _logger = logger;
        using var factory = LoggerFactory.Create(builder =>
        {
            builder.AddConsole()
                .AddDebug()
                .SetMinimumLevel(LogLevel.Debug);
        });
        var repetierLogger = factory.CreateLogger<RepetierConnection>();
      
        // TODO: Load from secret storage
        var apiKey = "32e372fc-4107-4d52-9416-7afaec6408a4";
        // TODO: read from appsettings
        var host = "demo4010.repetier-server.com";
        
        // setup repetier connection
        _repetierCon = new RepetierConnection.RepetierConnectionBuilder()
            .WithLogger(repetierLogger)
            .WithWebsocketHost($"wss://{host}/socket/?apiKey={apiKey}")
            .WithHttpHost($"https://{host}/")
            .UseSession(new RepetierSession(new ApiKeyAuth(apiKey), 15))
            .WithEventFilter(e => e is EventConstants.TEMP) // exclude temp event
            // schedule commands based on the 30-second timer event from the server
            .ScheduleServerCommand(RepetierTimer.Timer30, new StateListCommand(false))
            .SchedulePrinterCommand(RepetierTimer.Timer30, ListJobsCommand.AllJobs, "Delta")
            .Build();
        
        _repetierCon.HttpRequestFailedAsync += (args) =>
        {
            _logger.LogInformation("=X=> HttpRequest failed: {}", args.Request.Resource);
            return Task.CompletedTask;
        };
        
        _repetierCon.ConnectedAsync += async (connectedArgs) => {
            _logger.LogInformation("<=!= {} to repetier server: {scheme}://{host}{res}",
                connectedArgs.Reconnect ? "Reconnected" : "Connected",
                connectedArgs.Url.Scheme.ToString(),
                connectedArgs.Url.Host.ToString(),
                connectedArgs.Url.AbsolutePath);
            
            var info = await _repetierCon.GetRemoteServer().GetServerInfo();
            if ( info != null )
            {
                var printerSlugs = info.Printers.Select(p => p.Slug).ToList();
                _logger.LogInformation("ServerInfo\n\t- Name: {servername} ({variant} v{version})\n\t- Printer: [{printer}]\n\t- UUID: {uuid}\n\t- API-Key: {apikey}",
                    info.ServerName, info.Name, info.Version, string.Join(", ", printerSlugs), info.ServerUUID, info.ApiKey);
                
                printerSlugs.ForEach(async printerSlug =>
                {
                    var remotePrinter = _repetierCon.GetRemotePrinter(printerSlug);
                    _remotePrinters.Add(remotePrinter);
                    _repetierCon.SchedulePrinterCommand(printerSlug, ListJobsCommand.AllJobs, RepetierTimer.Timer60);
                    await _repetierCon.SendPrinterCommand(ListModelsCommand.Instance, printerSlug);
                });
            }
        };
        
        _repetierCon.SessionEstablishedAsync += (r) => {
            _logger.LogInformation("<=!= Session established. SessionId='{SessionId}'", r.SessionId);
            return Task.CompletedTask;
        };
        
        _repetierCon.PrintStartFailedAsync += (args) => {
            _logger.LogInformation("<=!= PrintStart failed: {}", JsonSerializer.Serialize(args));
            return Task.CompletedTask;
        };

        _repetierCon.PrinterCommandSendAsync += args => Task.CompletedTask;
        _repetierCon.PrintStartedAsync += args => Task.CompletedTask;
        _repetierCon.PrintFinishedAsync += args => Task.CompletedTask;
        _repetierCon.PrintKilledAsync += args => Task.CompletedTask;
        _repetierCon.EventReceivedAsync += args => Task.CompletedTask;
        _repetierCon.PrinterStateReceivedAsync += args => Task.CompletedTask;
        _repetierCon.RawResponseReceivedAsync += args => Task.CompletedTask; 
        _repetierCon.PrinterResponseReceivedAsync += OnPrinterResponseReceived;
        _repetierCon.ServerResponseReceivedAsync += OnServerResponseReceived; 
        _repetierCon.LayerChangedAsync += args => Task.CompletedTask;
        _repetierCon.ServerCommandSendAsync += args => Task.CompletedTask;
    }

    private Task OnServerResponseReceived(ResponseEventArgs arg)
    {
        var repetierResponse = arg.Response;
        if ( repetierResponse.CommandId != CommandConstants.PING )
        {
            _logger.LogInformation("<=#=[{action}]=#= | #{callbackId}", 
                repetierResponse.CommandId, repetierResponse.CallBackId);
        }
        return Task.CompletedTask;
    }

    private Task OnPrinterResponseReceived(PrinterResponseEventArgs args)
    {
        var repetierResponse = args.Response;
        if ( repetierResponse.CommandId == CommandConstants.LIST_JOBS )
        {
            var jobList = (ModelInfoList)repetierResponse.Data;
            if ( jobList.Models.Count > 0 && jobList.Models.First().State == "running")
            {
                var modelInfo = jobList.Models.First();
                _logger.LogInformation("<=#={action}=#= | {printer} | #{calldback_id}: Amount jobs={}", repetierResponse.CommandId, args.Printer, repetierResponse.CallBackId, jobList.Models.Count);
                _logger.LogInformation(
                    "Currently printing: {Name} (JobId: {Id}), Layer={Layer}, Lines={Lines}, PrintTime={PrintTime} sec",
                    modelInfo.Name,  modelInfo.Id, modelInfo.Layer, modelInfo.Lines, modelInfo.PrintTime);
            }
        }
        return Task.CompletedTask;  
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _repetierCon.Connect();

        while (!stoppingToken.IsCancellationRequested)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                //_logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            }
            await Task.Delay(10000, stoppingToken);
        }
    }
}

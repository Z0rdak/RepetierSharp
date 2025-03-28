using System.Text.Json;
using RepetierSharp.Control;
using RepetierSharp.Internal;
using RepetierSharp.Models.Commands;
using RepetierSharp.Models.Responses;
using RepetierSharp.Util;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace RepetierSharp.Examples;

/// <summary>
/// TODO: move ListModels, ListJobs, etc into IRemotePrinter
/// TODO: Add CRUD to manage scheduled commands
/// TODO: Separate event handlers for server and printer events (like responses and commands already have)
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
                .SetMinimumLevel(LogLevel.Information);
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
            // authentication with password (password is being hashed internally according to repetier server docs)
            // .UseSession(new RepetierSession(new CredentialAuth("login", "password"), 15))
            .UseSession(new RepetierSession(new ApiKeyAuth(apiKey), 10))
            // exclude temp event from logging and being fired through event handlers
            .WithEventFilter(e => e is EventConstants.TEMP)
            // exclude ping response from logging and being fired through the event handlers
            .WithResponseFilter(e => e is CommandConstants.PING)
            // exclude ping command from logging and being fired through the event handlers
            .WithCommandFilter(e => e is CommandConstants.PING)
            // schedule command based on the 30-second timer event from the server
            .ScheduleServerCommand(RepetierTimer.Timer30, new StateListCommand(false))
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

            if ( !connectedArgs.Reconnect )
            {
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
                        _repetierCon.SchedulePrinterCommand(printerSlug, ListJobsCommand.AllJobs, RepetierTimer.Timer30);
                        await _repetierCon.SendPrinterCommand(ListModelsCommand.Instance, printerSlug);
                    });
                }  
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

        _repetierCon.PrintStartedAsync += (PrintJobStartedEventArgs args) => Task.CompletedTask;
        _repetierCon.PrintFinishedAsync += (PrintJobFinishedEventArgs args) => Task.CompletedTask;
        _repetierCon.PrintKilledAsync += (PrintJobKilledEventArgs args) => Task.CompletedTask;
        _repetierCon.EventReceivedAsync += (EventReceivedEventArgs args) => Task.CompletedTask;
        // _repetierCon.PrinterEventReceivedAsync += (PrinterEventReceivedEventArgs args) => Task.CompletedTask;
        _repetierCon.PrinterStateReceivedAsync += (StateChangedEventArgs args) => Task.CompletedTask;
        _repetierCon.RawResponseReceivedAsync += (RawResponseReceivedEventArgs args) => Task.CompletedTask; 
        _repetierCon.LayerChangedAsync += (LayerChangedEventArgs args) => Task.CompletedTask;
        _repetierCon.ServerCommandSendAsync += OnServerCommandSend;
        _repetierCon.PrinterCommandSendAsync += OnPrinterCommandSend;
        _repetierCon.PrinterResponseReceivedAsync += OnPrinterResponseReceived;
        _repetierCon.ServerResponseReceivedAsync += OnServerResponseReceived; 
    }
    private Task OnServerCommandSend(ServerCommandEventArgs args)
    {
        var command = args.Command;
        // _logger.LogInformation("=?=[{action}]=?=> | Server | #{}", command.Action, command.CallbackId);
        return Task.CompletedTask;
    }
    private Task OnServerResponseReceived(ResponseEventArgs arg)
    {
        var repetierResponse = arg.Response;
        // _logger.LogInformation("<=#=[{action}]=#= | #{callback_id}", repetierResponse.CommandId, repetierResponse.CallBackId);
        return Task.CompletedTask;
    }
    
    private Task OnPrinterCommandSend(PrinterCommandEventArgs args)
    {
        var command = args.Command;
        var printer = args.Printer;
        _logger.LogInformation("=?=[{action}]=?=> | {printer} | #{}", command.Action, printer, command.CallbackId);
        return Task.CompletedTask;
    }

    private Task OnPrinterResponseReceived(PrinterResponseEventArgs args)
    {
        var repetierResponse = args.Response;
        if ( repetierResponse.CommandId == CommandConstants.LIST_JOBS )
        {
            var jobList = (ModelInfoList)repetierResponse.Data;
            var runningJob = jobList.Models.Find(j => j.State == "running");
            if ( runningJob != null )
            {
                _logger.LogInformation("<=#={action}=#= | {printer} | #{callback_id}: Amount jobs={}\n" +
                                       "Currently printing: {Name} (JobId: {Id}, Repeat: {repeat}, Layer={Layer}, Lines={Lines}, PrintTime={PrintTime} sec",
                    repetierResponse.CommandId, args.Printer, repetierResponse.CallBackId, jobList.Models.Count,
                    runningJob.Name, runningJob.Id, runningJob.Repeat, runningJob.Layer, runningJob.Lines, (int)runningJob.PrintTime);
            }
            return Task.CompletedTask;
        }
        _logger.LogInformation("<=#={action}=#= | {printer} | #{callback_id}", repetierResponse.CommandId, args.Printer, repetierResponse.CallBackId);
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

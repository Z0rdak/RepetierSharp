using System;
using System.IO;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using RepetierSharp.Internal;
using RepetierSharp.Models;
using RepetierSharp.Util;
using RestSharp;
using static RepetierSharp.Internal.Constants;

namespace RepetierSharp.Control
{
    public class RemoteRepetierServer(RepetierSession session, RepetierPrintJobEvents printJobEvents, RepetierClientEvents clientEvents, IRestClient restClient, ILogger<RemoteRepetierServer>? logger = null) : IRemoteServer
    {
        private readonly ILogger<RemoteRepetierServer> _logger = logger ?? NullLogger<RemoteRepetierServer>.Instance;
        private RepetierSession Session { get; } = session;
        private RepetierClientEvents ClientEvents { get; } = clientEvents;
        private RepetierPrintJobEvents PrintJobEvents { get; } = printJobEvents;
        private IRestClient RestClient { get; } = restClient;

        /// <summary>
        /// Returns basic infos about the server. Can be called even without an api-key. 
        /// Beside other, it contains information about the printer slug names.
        /// </summary>
        public async Task<RepetierServerInformation?> GetServerInfo()
        {
            var restRequest = new RestRequest("/printer/info");
            try
            {
                var response = await RestClient.ExecuteAsync(restRequest);
                if ( response is { StatusCode: HttpStatusCode.OK, Content: not null } )
                {
                    return JsonSerializer.Deserialize<RepetierServerInformation>(response.Content);
                }
                await ClientEvents.HttpRequestFailedEvent.InvokeAsync(new HttpContextEventArgs(response.Request,
                    response));
            }
            catch ( Exception e )
            {
                var httpContext = new HttpContextEventArgs(restRequest, null);
                await ClientEvents.HttpRequestFailedEvent.InvokeAsync(httpContext);
            }
            return null;
        }
        
        private RestRequest StartPrintRequest(string gcodeFilePath, string printerName,
            StartBehavior autostart = StartBehavior.Autostart)
        {
            var gcodeFileName = Path.GetFileNameWithoutExtension(gcodeFilePath);
            var request = new RestRequest($"/printer/job/{printerName}", Method.Post)
                .AddFile(FilenameParam, gcodeFilePath)
                .AddHeader(KnownHeaders.ContentType, MultiPartFormData)
                .AddParameter(ActionParam, UploadAction)
                .AddParameter(AutostartParam, $"{(int)autostart}")
                .AddParameter(NameParam, gcodeFileName);

            if ( !string.IsNullOrEmpty(Session.SessionId) )
            {
                request = request.AddParameter(SessionParam, Session.SessionId);
            }

            return request;
        }

        private RestRequest StartPrintRequest(string fileName, byte[] data, string printerName,
            StartBehavior autostart = StartBehavior.Autostart)
        {
            var request = new RestRequest($"/printer/job/{printerName}", Method.Post)
                .AddFile(FilenameParam, data, fileName)
                .AddHeader(KnownHeaders.ContentType, MultiPartFormData)
                .AddParameter(ActionParam, UploadAction)
                .AddParameter(AutostartParam, $"{(int)autostart}")
                .AddParameter(NameParam, fileName);

            if ( !string.IsNullOrEmpty(Session.SessionId) )
            {
                request = request.AddParameter(SessionParam, Session.SessionId);
            }

            return request;
        }

        /// <summary>
        ///     Create a REST request for uploading a gcode file
        /// </summary>
        /// <param name="gcodeFilePath"> The path of the file to upload (/path/file.gcode) </param>
        /// <param name="printer"> Printer slug to upload to </param>
        /// <param name="group"> Group to add gcode to </param>
        /// <param name="overwrite"> Flag to overwrite existing file with the same name </param>
        /// <returns></returns>
        private RestRequest UploadModel(string gcodeFilePath, string printer, string group, bool overwrite = false)
        {
            var gcodeFileName = Path.GetFileNameWithoutExtension(gcodeFilePath);
            var request = new RestRequest($"/printer/model/{printer}", Method.Post)
                .AddFile(FilenameParam, gcodeFilePath)
                .AddHeader(KnownHeaders.ContentType, MultiPartFormData)
                .AddParameter(ActionParam, UploadAction)
                .AddParameter("group", group)
                .AddParameter("overwrite", $"{overwrite}")
                .AddParameter(NameParam, gcodeFileName);

            if ( !string.IsNullOrEmpty(Session.SessionId) )
            {
                request = request.AddParameter(SessionParam, Session.SessionId);
            }

            return request;
        }

        /// <summary>
        ///     Create a REST request for uploading a gcode file.
        /// </summary>
        /// <param name="fileName">  The name of the file to upload (file.gcode) </param>
        /// <param name="file"> The content of the actual gcode file </param>
        /// <param name="printer"> Printer slug to upload to </param>
        /// <param name="group"> Group to add gcode to </param>
        /// <param name="overwrite"> Flag to overwrite existing file with the same name </param>
        /// <returns></returns>
        private RestRequest UploadModel(string fileName, byte[] file, string printer, string group,
            bool overwrite = false)
        {
            var request = new RestRequest($"/printer/model/{printer}", Method.Post)
                .AddFile(FilenameParam, file, fileName)
                .AddHeader(KnownHeaders.ContentType, MultiPartFormData)
                .AddParameter(ActionParam, UploadAction)
                .AddParameter("group", group)
                .AddParameter("overwrite", $"{overwrite}")
                .AddParameter(NameParam, fileName);

            if ( !string.IsNullOrEmpty(Session.SessionId) )
            {
                request = request.AddParameter(SessionParam, Session.SessionId);
            }

            return request;
        }

        /// <summary>
        ///     Upload a gcode file via REST API
        /// </summary>
        /// <param name="gcodeFilePath"> The path of the file to upload (/path/file.gcode) </param>
        /// <param name="printer"> Printer slug to upload to </param>
        /// <param name="group"> Group to upload gcode to </param>
        /// <param name="overwrite"> Flag to overwrite existing file with the same name. Defaults to false </param>
        public async Task<bool> UploadGCode(string gcodeFilePath, string printer, string group, bool overwrite = false)
        {
            try
            {
                var request = UploadModel(gcodeFilePath, printer, group, overwrite);
                var response = await RestClient.ExecuteAsync(request);
                if ( response.StatusCode != HttpStatusCode.OK )
                {
                    await ClientEvents.HttpRequestFailedEvent.InvokeAsync(new HttpContextEventArgs(request, response));
                    return false;
                }

                if ( response.ErrorException != null )
                {
                    throw response.ErrorException;
                }
            }
            catch ( Exception ex )
            {
                _logger.LogError(ex, "Error while uploading gcode file: {Error}", ex.Message);
            }

            return await Task.FromResult(true);
        }

        /// <summary>
        ///     Upload a gcode file via REST API
        /// </summary>
        /// <param name="fileName"> The name of the file to upload (file.gcode) </param>
        /// <param name="file"> The content of the actual gcode file </param>
        /// <param name="printer"> Printer slug to upload to </param>
        /// <param name="group"> Group to upload gcode to </param>
        /// <param name="overwrite"> Flag to overwrite existing file with the same name. Defaults to false </param>
        public async Task<bool> UploadGCode(string fileName, byte[] file, string printer, string group,
            bool overwrite = false)
        {
            try
            {
                var request = UploadModel(fileName, file, printer, group, overwrite);
                var response = await RestClient.ExecuteAsync(request);
                if ( response.StatusCode != HttpStatusCode.OK )
                {
                    await ClientEvents.HttpRequestFailedEvent.InvokeAsync(new HttpContextEventArgs(request, response));
                    return false;
                }

                if ( response.ErrorException != null )
                {
                    throw response.ErrorException;
                }
            }
            catch ( Exception ex )
            {
                _logger.LogError(ex, "Error while uploading gcode file: {Error}", ex.Message);
            }

            return await Task.FromResult(true);
        }

        /// <summary>
        ///     Upload given gcode and start the printing process via REST-API.
        /// </summary>
        /// <param name="gcodeFilePath"> The path of the file to upload (/path/file.gcode) </param>
        /// <param name="printer"> Printer slug to upload to </param>
        /// <param name="autostart"> Flag to indicate the start behavior when uploading gcode. Defaults to autostart </param>
        public async Task<bool> UploadAndStartPrint(string gcodeFilePath, string printer,
            StartBehavior autostart = StartBehavior.Autostart)
        {
            try
            {
                var request = StartPrintRequest(gcodeFilePath, printer, autostart);
                var response = await RestClient.ExecuteAsync(request);
                if ( response.StatusCode != HttpStatusCode.OK )
                {
                    await ClientEvents.HttpRequestFailedEvent.InvokeAsync(new HttpContextEventArgs(request, response));
                    await PrintJobEvents.PrintStartFailedEvent.InvokeAsync(new PrintJobStartFailedEventArgs(printer, response));
                    return false;
                }

                if ( response.ErrorException != null )
                {
                    throw response.ErrorException;
                }
            }
            catch ( Exception ex )
            {
                _logger.LogError(ex, "Error while uploading and starting print: {Error}", ex.Message);
            }

            return await Task.FromResult(true);
        }

        /// <summary>
        ///     Upload given gcode and start the printing process via REST-API.
        /// </summary>
        /// <param name="fileName"> The name of the file to upload (file.gcode) </param>
        /// <param name="file"> The content of the actual gcode file </param>
        /// <param name="printer"> Printer slug to upload to </param>
        /// <param name="autostart"> Flag to indicate the start behavior when uploading gcode. Defaults to autostart </param>
        public async Task<bool> UploadAndStartPrint(string fileName, byte[] file, string printer,
            StartBehavior autostart = StartBehavior.Autostart)
        {
            try
            {
                var request = StartPrintRequest(fileName, file, printer, autostart);
                var response = await RestClient.ExecuteAsync(request);
                if ( response.StatusCode != HttpStatusCode.OK )
                {
                    await ClientEvents.HttpRequestFailedEvent.InvokeAsync(new HttpContextEventArgs(request, response));
                    return false;
                }

                if ( response.ErrorException != null )
                {
                    throw response.ErrorException;
                }
            }
            catch ( Exception ex )
            {
                _logger.LogError(ex, "Error while uploading and starting print: {Error}", ex.Message);
            }

            return await Task.FromResult(true);
        }
    }
}

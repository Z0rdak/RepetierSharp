using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace RepetierMqtt.Util
{
    public static class ApiConstants
    {
        public const string PRINTER_INFO_PATH = "/printer/info";
        public static readonly IRestRequest PRINTER_INFO_REQUEST = new RestRequest(PRINTER_INFO_PATH, Method.GET);

        public static IRestRequest StartPrintRequest(string gcodeFilePath, string printerName, string apiKey)
        {
            var GCODEFileName = Path.GetFileNameWithoutExtension(gcodeFilePath);
            return new RestRequest("", Method.POST)
                .AddFile("gcode", GCODEFileName)
                .AddHeader("x-api-key", apiKey)
                .AddHeader("Content-Type", "multipart/form-data")
                .AddParameter("a", "upload")
                .AddParameter("name", GCODEFileName);
        }
    }
}

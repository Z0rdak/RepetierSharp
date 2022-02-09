using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace RepetierSharp.Util
{
    public static class ApiConstants
    {
        public const string PRINTER_INFO_PATH = "/printer/info";

        public static readonly RestRequest PRINTER_INFO_REQUEST = new RestRequest(PRINTER_INFO_PATH, Method.Get);
    }
}

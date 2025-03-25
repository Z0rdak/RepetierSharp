using System.Threading.Tasks;
using RepetierSharp.Models;
using RepetierSharp.Util;
using RestSharp;

namespace RepetierSharp.Control
{
    /// <summary>
    /// Interface for server related commands 
    /// </summary>
    public interface IRemoteServer
    {
        Task<RepetierServerInformation?> GetServerInfo();

        Task<bool> UploadGCode(string gcodeFilePath, string printer, string group, bool overwrite);

        Task<bool> UploadGCode(string fileName, byte[] file, string printer, string group, bool overwrite);

        Task<bool> UploadAndStartPrint(string gcodeFilePath, string printer, StartBehavior autostart);

        Task<bool> UploadAndStartPrint(string fileName, byte[] file, string printer, StartBehavior autostart);
    }

}

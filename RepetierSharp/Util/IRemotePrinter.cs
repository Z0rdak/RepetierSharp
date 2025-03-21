using System.Threading.Tasks;
using RepetierSharp.Models.Commands;

namespace RepetierSharp.Util
{
    public interface IRemotePrinter
    {
        string Printer { get; }
        Task PauseJob(string reason);
        Task ContinueJob();
        Task StopJob();
        Task EmergencyStop();
        Task StartJob(int jobId);
        Task GetJobInfo(int jobId);
        Task EnqueueJob(int modelId, bool autostart);
        Task GetModelInfo(int modelId);
        Task RemoveJob(int jobId);
        Task PreHeat(int extruderNo, int heatedBedNo, int heatedChamberNo);
        Task PreheatAll();
        Task PreheatActive();
        Task Cooldown();
        Task SetTemperature(TemperatureTarget targetType, int temperature, int targetId = 0); 
        Task SetExtruderTemp(int temperature, int extruderNo = 0); 
        Task SetHeatedBedTemp(int temperature, int heatedBedId = 0);
        Task SetHeatedChamberTemp(int temperature, int heatedChamberId = 0);
        Task SetFanSpeed(int fanSpeed, int fanId = 0); 
        Task TurnOffFan(int fanId = 0);
        Task TurnOnFan(int fanId = 0);
        Task SetFlowMultiplier(int flowMultiplier); 
        Task SetSpeedMultiplier(int speedMultiplier);
    }
}

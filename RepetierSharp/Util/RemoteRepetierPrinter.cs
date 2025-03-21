using System;
using System.Threading.Tasks;
using RepetierSharp.Models.Commands;

namespace RepetierSharp.Util
{
    public class RemoteRepetierPrinter(RepetierConnection repetierConnection, string printer) : IRemotePrinter
    {
        private RepetierConnection rc { get; } = repetierConnection;
        public string Printer { get; } = printer;

        public async Task PauseJob(string reason = "RepetierSharp requested pause")
        {
            await rc.SendPrinterCommand(new SendCommand($"@pause {reason}"), Printer);
        }

        public async Task ContinueJob()
        {
            await rc.SendPrinterCommand(ContinueJobCommand.Instance, Printer);
        }

        public async Task StopJob()
        {
            await rc.SendPrinterCommand(StopJobCommand.Instance, Printer);
        }

        public async Task EmergencyStop()
        {
            await rc.SendPrinterCommand(EmergencyStopCommand.Instance, Printer);
        }

        public async Task StartJob(int jobId)
        {
            await rc.SendPrinterCommand(new StartJobCommand(jobId), Printer);
        }

        public async Task GetJobInfo(int jobId)
        {
            await rc.SendPrinterCommand(new JobInfoCommand(jobId), Printer);
        }

        public async Task EnqueueJob(int modelId, bool autostart = true)
        {
            await rc.SendPrinterCommand(new CopyModelCommand(modelId, autostart), Printer);
        }

        public async Task GetModelInfo(int modelId)
        {
            await rc.SendPrinterCommand(new ModelInfoCommand(modelId), Printer);
        }

        public async Task RemoveJob(int jobId)
        {
            await rc.SendPrinterCommand(new RemoveJobCommand(jobId), Printer);
        }

        public async Task PreHeat(int extruderNo, int heatedBedNo, int heatedChamberNo)
        {
            await rc.SendPrinterCommand(new PreheatCommand(extruderNo, heatedBedNo, heatedChamberNo), Printer);
        }
        
        public async Task PreheatAll()
        {
            await rc.SendPrinterCommand(new PreheatCommand((int)ExtruderConstants.All, 0, 0), Printer);
        }

        public async Task PreheatActive()
        {
            await rc.SendPrinterCommand(new PreheatCommand((int)ExtruderConstants.Active, 0, 0), Printer);
        }

        public async Task Cooldown()
        {
            await rc.SendPrinterCommand(new CooldownCommand((int)ExtruderConstants.All, 0, 0), Printer);
        }

        public async Task SetTemperature(TemperatureTarget targetType, int temperature, int targetId = 0)
        {
            switch ( targetType )
            {
                case TemperatureTarget.Extruder:
                    await SetExtruderTemp(temperature, targetId);
                    break;
                case TemperatureTarget.HeatedBed:
                    await SetHeatedBedTemp(temperature, targetId);
                    break;
                case TemperatureTarget.HeatedChamber:
                    await SetHeatedChamberTemp(temperature, targetId);
                    break;
            }
        }

        public async Task SetExtruderTemp(int temperature, int extruderNo = 0)
        {
            await rc.SendPrinterCommand(new SetExtruderTempCommand(temperature, extruderNo), Printer);
        }

        public async Task SetHeatedBedTemp(int temperature, int heatedBedId = 0)
        {
            await rc.SendPrinterCommand(new SetHeatedBedTempCommand(temperature, heatedBedId), Printer);
        }

        public async Task SetHeatedChamberTemp(int temperature, int heatedChamberId = 0)
        {
            await rc.SendPrinterCommand(new SetHeatedChamberTempCommand(temperature, heatedChamberId), Printer);
        }

        public async Task SetFanSpeed(int fanSpeed, int fanId = 0)
        {
            var speedInVoltage = Math.Clamp(fanSpeed, 0, 100) * 255 / 100;
            await rc.SendPrinterCommand(new SetFanSpeedCommand(speedInVoltage, fanId), Printer);
        }

        public async Task TurnOffFan(int fanId = 0)
        {
            await SetFanSpeed(SetFanSpeedCommand.FAN_OFF, fanId);
        }

        public async Task TurnOnFan(int fanId = 0)
        {
            await SetFanSpeed(SetFanSpeedCommand.MAX_THROTTLE, fanId);
        }

        public async Task SetFlowMultiplier(int flowMultiplier)
        {
            await rc.SendPrinterCommand(new SetFlowMultiplyCommand(flowMultiplier), Printer);
        }

        public async Task SetSpeedMultiplier(int speedMultiplier)
        {
            await rc.SendPrinterCommand(new SetSpeedMultiplyCommand(speedMultiplier), Printer);
        }
    }
}

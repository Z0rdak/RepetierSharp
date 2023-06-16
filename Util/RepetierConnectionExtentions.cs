﻿using System;
using RepetierSharp.Models.Commands;

namespace RepetierSharp.Extentions
{
    public static class RepetierConnectionExtentions
    {
        /// <summary>
        /// Send a single "listPrinters" message to the repetier rerver.
        /// The response to a "listPrinters" command contains the current print progress.
        /// </summary>
        public static async void QueryPrinterList(this RepetierConnection rc)
        {
            await rc.SendCommand(ListPrinterCommand.Instance);
        }

        /// <summary>
        /// Send a single "stateList" message to the repetier server.
        /// The response to a "stateList" command contains information regarding the printer state.
        /// </summary>
        public static async void QueryPrinterStateList(this RepetierConnection rc, bool includeHistory = false)
        {
            await rc.SendCommand(new StateListCommand(includeHistory));
        }

        public static async void PauseJob(this RepetierConnection rc)
        {
            await rc.SendCommand(new SendCommand("@pause RepetierSharp requested pause."));
        }

        /// <summary>
        /// Stop the current print and trigger a "jobKilled" event.
        /// </summary>
        /// <param name="rc"></param>
        public static async void StopJob(this RepetierConnection rc)
        {
            await rc.SendCommand(StopJobCommand.Instance);
        }

        /// <summary>
        /// Initiate an emergency stop.
        /// </summary>
        /// <param name="rc"></param>
        public static async void EmergencyStop(this RepetierConnection rc)
        {
            await rc.SendCommand(EmergencyStopCommand.Instance);
        }

        /// <summary>
        /// Logout current active session.
        /// </summary>
        /// <param name="rc"></param>
        public static async void Logout(this RepetierConnection rc)
        {
            await rc.SendCommand(LogoutCommand.Instance);
        }

        /// <summary>
        /// Enqueue gcode/model into the print queue and start print by default.
        /// </summary>
        /// <param name="rc"></param>
        /// <param name="modelId"> Gcode id to enqueue into print queue </param>
        /// <param name="autostart"> True to automatically start print job, false for queueing only</param>
        public static async void EnqueueJob(this RepetierConnection rc, int modelId, bool autostart = true)
        {
            await rc.SendCommand(new CopyModelCommand(modelId, autostart));
        }

        /// <summary>
        /// Get info about a gcode (model).
        /// </summary>
        /// <param name="rc"></param>
        /// <param name="modelId"></param>
        public static async void GetModelInfo(this RepetierConnection rc, int modelId)
        {
            await rc.SendCommand(new ModelInfoCommand(modelId));
        }

        /// <summary>
        /// Get info about a gcode in the print queue.
        /// </summary>
        /// <param name="rc"></param>
        /// <param name="jobId"></param>
        public static async void GetJobInfo(this RepetierConnection rc, int jobId)
        {
            await rc.SendCommand(new JobInfoCommand(jobId));
        }

        /// <summary>
        /// Starts job already copied to print queue.
        /// </summary>
        /// <param name="rc"></param>
        /// <param name="jobId"> Id of job in queue to start</param>
        public static async void StartJob(this RepetierConnection rc, int jobId)
        {
            await rc.SendCommand(new StartJobCommand(jobId));
        }

        /// <summary>
        /// Continue active job.
        /// </summary>
        /// <param name="rc"></param>
        public static async void ContinueJob(this RepetierConnection rc)
        {
            await rc.SendCommand(ContinueJobCommand.Instance);
        }

        /// <summary>
        /// Remove job from print queue. Only works if job with same id is not currently running.
        /// </summary>
        /// <param name="rc"></param>
        /// <param name="jobId"> Id of job to remove from print queue. </param>
        public static async void RemoveJob(this RepetierConnection rc, int jobId)
        {
            await rc.SendCommand(new RemoveJobCommand(jobId));
        }

        public static async void CreateUser(this RepetierConnection rc, string user, string password, int permission)
        {
            await rc.SendCommand(new CreateUserCommand(user, password, permission));
        }

        public static async void UpdateUser(this RepetierConnection rc, string user, int permission, string password = "")
        {
            await rc.SendCommand(new UpdateUserCommand(user, permission, password));
        }

        public static async void DeleteUser(this RepetierConnection rc, string user)
        {
            await rc.SendCommand(new DeleteUserCommand(user));
        }

        public static async void Preheat(this RepetierConnection rc, int extruderNo, int heatedBedNo, int heatedChamberNo)
        {
            await rc.SendCommand(new PreheatCommand(extruderNo, heatedBedNo, heatedChamberNo));
        }

        public static async void PreheatAll(this RepetierConnection rc)
        {
            await rc.SendCommand(new PreheatCommand((int)ExtruderConstants.All, 0, 0));
        }

        public static async void PreheatActive(this RepetierConnection rc)
        {
            await rc.SendCommand(new PreheatCommand((int)ExtruderConstants.Active, 0, 0));
        }

        public static async void Cooldown(this RepetierConnection rc)
        {
            await rc.SendCommand(new CooldownCommand((int)ExtruderConstants.All, 0, 0));
        }

        public static void SetTemperature(this RepetierConnection rc, TemperatureTarget targetType, int temperature, int targetId = 0)
        {
            switch (targetType)
            {
                case TemperatureTarget.Extruder:
                    SetExtruderTemp(rc, temperature, targetId);
                    break;
                case TemperatureTarget.HeatedBed:
                    SetHeatedBedTemp(rc, temperature, targetId);
                    break;
                case TemperatureTarget.HeatedChamber:
                    SetHeatedChamberTemp(rc, temperature, targetId);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Set extruder temperature in degree celcius.
        /// </summary>
        /// <param name="rc"></param>
        /// <param name="temperature">Extruder temperature in degree celcius</param>
        /// <param name="extruderNo">Id of the extruder (default = 0)</param>
        public static async void SetExtruderTemp(this RepetierConnection rc, int temperature, int extruderNo = 0)
        {
            await rc.SendCommand(new SetExtruderTempCommand(temperature, extruderNo));
        }

        /// <summary>
        /// Set heated bed temperature in degree celcius.
        /// </summary>
        /// <param name="rc"></param>
        /// <param name="temperature">Heated bed temperature in degree celcius</param>
        /// <param name="heatedBedId">Id of the heated bed (default = 0)</param>
        public static async void SetHeatedBedTemp(this RepetierConnection rc, int temperature, int heatedBedId = 0)
        {
            await rc.SendCommand(new SetHeatedBedTempCommand(temperature, heatedBedId));
        }

        /// <summary>
        /// Set heated chamber temperature in degree celcius.
        /// </summary>
        /// <param name="rc"></param>
        /// <param name="temperature">Heated chamber temperature in degree celcius</param>
        /// <param name="heatedChamberId">Id of the heated chamber (default = 0)</param>
        public static async void SetHeatedChamberTemp(this RepetierConnection rc, int temperature, int heatedChamberId = 0)
        {
            await rc.SendCommand(new SetHeatedChamberTempCommand(temperature, heatedChamberId));
        }

        /// <summary>
        /// Set speed of the given fan. 
        /// <br> The speed is supplied in percent. E.g. 75 will result in 75% total fan speed.</br>
        /// </summary>
        /// <param name="rc"></param>
        /// <param name="fanSpeed"> Fan speed in percent (Repetier Server usually uses values from 0-255 for voltage)</param>
        /// <param name="fanId"> Id of the fan (default = 0 for the first fan)</param>
        public static async void SetFanSpeed(this RepetierConnection rc, int fanSpeed, int fanId = 0)
        {
            await rc.SendCommand(new SetFanSpeedCommand(Math.Clamp(fanSpeed, 0, 100) * 255 / 100, fanId));
        }

        public static void TurnOffFan(this RepetierConnection rc, int fanId = 0)
        {
            SetFanSpeed(rc, SetFanSpeedCommand.FAN_OFF, fanId);
        }

        public static void TurnOnFan(this RepetierConnection rc, int fanId = 0)
        {
            SetFanSpeed(rc, SetFanSpeedCommand.MAX_THROTTLE, fanId);
        }

        /// <summary>
        /// Sets the flow multiplier for extrusion. 
        /// <br> The flow multiplier is supplied in percent. E.g. 150 will result in 150% total flow rate. </br>
        /// </summary>
        /// <param name="rc"></param>
        /// <param name="flowMultiplier">The flow multiplier in percent.</param>
        public static async void SetFlowMultiplier(this RepetierConnection rc, int flowMultiplier)
        {
            await rc.SendCommand(new SetFlowMultiplyCommand(flowMultiplier));
        }

        /// <summary>
        /// Sets the speed mupltiplier for movements. 
        /// <br> The speed multiplier is supplied in percent. E.g. 150 will result in 150% total speed rate. </br>
        /// </summary>
        /// <param name="rc"></param>
        /// <param name="speedMultiplier">The speed multiplier in percent</param>
        public static async void SetSpeedMultiplier(this RepetierConnection rc, int speedMultiplier)
        {
            await rc.SendCommand(new SetSpeedMultiplyCommand(speedMultiplier));
        }

        public static async void QueryOpenMessages(this RepetierConnection rc)
        {
            await rc.SendCommand(MessagesCommand.Instance);
        }
    }
}

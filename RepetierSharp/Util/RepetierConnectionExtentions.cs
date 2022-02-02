using RepetierSharp.Models.Commands;
using RepetierSharp.Models.Events;

namespace RepetierSharp.Extentions
{
    public static class RepetierConnectionExtentions
    {
        /// <summary>
        /// Send a single "listPrinters" message to the repetier rerver.
        /// The response to a "listPrinters" command contains the current print progress.
        /// </summary>
        public static void QueryPrinterList(this RepetierConnection rc)
        {
            rc.SendCommand(ListPrinterCommand.Instance);
        }

        /// <summary>
        /// Send a single "stateList" message to the repetier server.
        /// The response to a "stateList" command contains information regarding the printer state.
        /// </summary>
        public static void QueryPrinterStateList(this RepetierConnection rc, bool includeHistory = false)
        {
            rc.SendCommand(new StateListCommand(includeHistory));
        }

        public static void PauseJob(this RepetierConnection rc)
        {
            rc.SendCommand(new SendCommand("@pause RepetierSharp requested pause."));
        }

        /// <summary>
        /// Send a single "stopJob" meassage to repetier server.
        /// The printer will stop the current print and trigger a "jobKilled" event
        /// </summary>
        public static void StopJob(this RepetierConnection rc)
        {
            rc.SendCommand(StopJobCommand.Instance);
        }

        /// <summary>
        /// Logout current active session.
        /// </summary>
        /// <param name="rc"></param>
        public static void Logout(this RepetierConnection rc)
        {
            rc.SendCommand(LogoutCommand.Instance);
        }

        /// <summary>
        /// Enqueue gcode/model into the print queue and start print by default.
        /// </summary>
        /// <param name="rc"></param>
        /// <param name="modelId"> Gcode id to enqueue into print queue </param>
        /// <param name="autostart"> True to automatically start print job, false for queueing only</param>
        public static void EnqueueJob(this RepetierConnection rc, int modelId, bool autostart = true)
        {
            rc.SendCommand(new CopyModelCommand(modelId, autostart));
        }

        /// <summary>
        /// Get info about a gcode (model).
        /// </summary>
        /// <param name="rc"></param>
        /// <param name="modelId"></param>
        public static void GetModelInfo(this RepetierConnection rc, int modelId)
        {
            rc.SendCommand(new ModelInfoCommand(modelId));
        }

        /// <summary>
        /// Get info about a gcode in the print queue.
        /// </summary>
        /// <param name="rc"></param>
        /// <param name="jobId"></param>
        public static void GetJobInfo(this RepetierConnection rc, int jobId)
        {
            rc.SendCommand(new JobInfoCommand(jobId));
        }

        /// <summary>
        /// Starts job already copied to print queue.
        /// </summary>
        /// <param name="rc"></param>
        /// <param name="jobId"> Id of job in queue to start</param>
        public static void StartJob(this RepetierConnection rc, int jobId)
        {
            rc.SendCommand(new StartJobCommand(jobId));
        }

        /// <summary>
        /// Continue active job.
        /// </summary>
        /// <param name="rc"></param>
        public static void ContinueJob(this RepetierConnection rc)
        {
            rc.SendCommand(ContinueJobCommand.Instance);
        }

        /// <summary>
        /// Remove job from print queue. Only works if job with same id is not currently running.
        /// </summary>
        /// <param name="rc"></param>
        /// <param name="jobId"> Id of job to remove from print queue. </param>
        public static void RemoveJob(this RepetierConnection rc, int jobId)
        {
            rc.SendCommand(new RemoveJobCommand(jobId));
        } 

        /// <summary>
        /// Activate printer with given printerSlug.
        /// </summary>
        /// <param name="rc"></param>
        /// <param name="printerSlug"> Printer to activate. </param>
        public static void ActivatePrinter(this RepetierConnection rc, string printerSlug)
        {
            rc.SendCommand(new ActivateCommand(printerSlug));
        }

        /// <summary>
        /// Deactivate printer with given printerSlug.
        /// </summary>
        /// <param name="rc"></param>
        /// <param name="printerSlug"> Printer to deactivate. </param>
        public static void DeactivatePrinter(this RepetierConnection rc, string printerSlug)
        {
            rc.SendCommand(new DeactivateCommand(printerSlug));
        }

        public static void CreateUser(this RepetierConnection rc, string user, string password, int permission)
        {
            rc.SendCommand(new CreateUserCommand(user, password, permission));
        } 

        public static void UpdateUser(this RepetierConnection rc, string user, int permission, string password = "")
        {
            rc.SendCommand(new UpdateUserCommand(user, permission, password));
        }

        public static void DeleteUser(this RepetierConnection rc, string user)
        {
            rc.SendCommand(new DeleteUserCommand(user));
        } 

        public static void Preheat(this RepetierConnection rc, int extruderNo, int heatedBedNo, int heatedChamberNo)
        {
            rc.SendCommand(new PreheatCommand(extruderNo, heatedBedNo, heatedChamberNo));
        }

        public static void PreheatAll(this RepetierConnection rc)
        {
            rc.SendCommand(new PreheatCommand((int)ExtruderConstants.All, 0, 0));
        }

        public static void PreheatActive(this RepetierConnection rc)
        {
            rc.SendCommand(new PreheatCommand((int)ExtruderConstants.Active, 0, 0));
        }

        public static void Cooldown(this RepetierConnection rc)
        {
            rc.SendCommand(new CooldownCommand((int)ExtruderConstants.All, 0, 0));
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

        public static void SetExtruderTemp(this RepetierConnection rc, int temperature, int extruderNo = 0)
        {
            rc.SendCommand(new SetExtruderTempCommand(temperature, extruderNo));
        }

        public static void SetHeatedBedTemp(this RepetierConnection rc, int temperature, int heatedBedId = 0)
        {
            rc.SendCommand(new SetHeatedBedTempCommand(temperature, heatedBedId));
        }

        public static void SetHeatedChamberTemp(this RepetierConnection rc, int temperature, int heatedChamberId = 0)
        {
            rc.SendCommand(new SetHeatedChamberTempCommand(temperature, heatedChamberId));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rc"></param>
        /// <param name="fanSpeed"> Fan speed in percent (Repetier Server usually uses values from 0-255 for voltage)</param>
        /// <param name="fanId"> Id of the fan (default = 0 for the first fan)</param>
        public static void SetFanSpeed(this RepetierConnection rc, double fanSpeed, int fanId = 0)
        {
            rc.SendCommand(new SetFanSpeedCommand((int)(fanSpeed * 255 / 100), fanId));
        }

        public static void TurnOffFan(this RepetierConnection rc, int fanId = 0)
        {
            SetFanSpeed(rc, SetFanSpeedCommand.FAN_OFF, fanId);
        }

        public static void TurnOnFan(this RepetierConnection rc, int fanId = 0)
        {
            SetFanSpeed(rc, SetFanSpeedCommand.MAX_THROTTLE, fanId);
        }

        public static void SetFlowMultiplier(this RepetierConnection rc, int flowMultiplier)
        {
            rc.SendCommand(new SetFlowMultiplyCommand(flowMultiplier));
        }

        public static void SetSpeedMultiplier(this RepetierConnection rc, int speedMultiplier)
        {
            rc.SendCommand(new SetSpeedMultiplyCommand(speedMultiplier));
        }

        public static void QueryOpenMessages(this RepetierConnection rc)
        {
            rc.SendCommand(MessagesCommand.Instance);
        }
    }
}

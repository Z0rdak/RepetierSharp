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
            rc.SendCommand(ListPrinterCommand.Instance, typeof(ListPrinterCommand));
        }

        /// <summary>
        /// Send a single "stateList" message to the repetier server.
        /// The response to a "stateList" command contains information regarding the printer state.
        /// </summary>
        public static void QueryPrinterStateList(this RepetierConnection rc, bool includeHistory = false)
        {
            rc.SendCommand(new StateListCommand(includeHistory), typeof(StateListCommand));
        }

        /// <summary>
        /// Send a single "stopJob" meassage to repetier server.
        /// The printer will stop the current print and trigger a "jobKilled" event
        /// </summary>
        public static void StopJob(this RepetierConnection rc)
        {
            rc.SendCommand(StopJobCommand.Instance, typeof(StopJobCommand));
        }

        public static void Logout(this RepetierConnection rc)
        {
            rc.SendCommand(LogoutCommand.Instance, typeof(LogoutCommand));
        }

        public static void EnqueueJob(this RepetierConnection rc, int modelId, bool autostart = true)
        {
            rc.SendCommand(new CopyModelCommand(modelId, autostart), typeof(CopyModelCommand));
        }

        public static void GetModelInfo(this RepetierConnection rc, int modelId)
        {
            rc.SendCommand(new ModelInfoCommand(modelId), typeof(ModelInfoCommand));
        }

        public static void GetJobInfo(this RepetierConnection rc, int jobId)
        {
            rc.SendCommand(new JobInfoCommand(jobId), typeof(JobInfoCommand));
        }

        public static void StartJob(this RepetierConnection rc, int jobId)
        {
            rc.SendCommand(new StartJobCommand(jobId), typeof(StartJobCommand));
        }

        public static void ContinueJob(this RepetierConnection rc)
        {
            rc.SendCommand(ContinueJobCommand.Instance, typeof(ContinueJobCommand));
        }

        public static void RemoveJob(this RepetierConnection rc, int jobId)
        {
            rc.SendCommand(new RemoveJobCommand(jobId), typeof(RemoveJobCommand));
        } 

        public static void ActivatePrinter(this RepetierConnection rc, string printerSlug)
        {
            rc.SendCommand(new ActivateCommand(printerSlug), typeof(ActivateCommand));
        }

        public static void DeactivatePrinter(this RepetierConnection rc, string printerSlug)
        {
            rc.SendCommand(new DeactivateCommand(printerSlug), typeof(DeactivateCommand));
        }

        public static void CreateUser(this RepetierConnection rc, string user, string password, int permission)
        {
            rc.SendCommand(new CreateUserCommand(user, password, permission), typeof(CreateUserCommand));
        } 

        public static void UpdateUser(this RepetierConnection rc, string user, int permission, string password = "")
        {
            rc.SendCommand(new UpdateUserCommand(user, permission, password), typeof(UpdateUserCommand));
        }

        public static void DeleteUser(this RepetierConnection rc, string user)
        {
            rc.SendCommand(new DeleteUserCommand(user), typeof(DeleteUserCommand));
        } 

        public static void Preheat(this RepetierConnection rc, int extruderNo, int heatedBedNo, int heatedChamberNo)
        {
            rc.SendCommand(new PreheatCommand(extruderNo, heatedBedNo, heatedChamberNo), typeof(PreheatCommand));
        }

        public static void PreheatAll(this RepetierConnection rc)
        {
            rc.SendCommand(new PreheatCommand((int)ExtruderConstants.All, 1, 1), typeof(PreheatCommand));
        }

        public static void PreheatActive(this RepetierConnection rc)
        {
            rc.SendCommand(new PreheatCommand((int)ExtruderConstants.Active, 1, 1), typeof(PreheatCommand));
        }


        public static void Cooldown(this RepetierConnection rc, int extruderNo, int heatedBedNo, int heatedChamberNo)
        {
            rc.SendCommand(new CooldownCommand((int)ExtruderConstants.All, heatedBedNo, heatedChamberNo), typeof(CooldownCommand));
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
            rc.SendCommand(new SetExtruderTempCommand(temperature, extruderNo), typeof(SetExtruderTempCommand));
        }

        public static void SetHeatedBedTemp(this RepetierConnection rc, int temperature, int heatedBedId = 0)
        {
            rc.SendCommand(new SetHeatedBedTempCommand(temperature, heatedBedId), typeof(SetHeatedBedTempCommand));
        }

        public static void SetHeatedChamberTemp(this RepetierConnection rc, int temperature, int heatedChamberId = 0)
        {
            rc.SendCommand(new SetHeatedChamberTempCommand(temperature, heatedChamberId), typeof(SetHeatedChamberTempCommand));
        }

        public static void SetFanSpeed(this RepetierConnection rc, int fanSpeed, int fanId = 0)
        {
            rc.SendCommand(new SetFanSpeedCommand(fanSpeed, fanId), typeof(SetFanSpeedCommand));
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
            rc.SendCommand(new SetFlowMultiplyCommand(flowMultiplier), typeof(SetFlowMultiplyCommand));
        }

        public static void SetSpeedMultiplier(this RepetierConnection rc, int speedMultiplier)
        {
            rc.SendCommand(new SetSpeedMultiplyCommand(speedMultiplier), typeof(SetSpeedMultiplyCommand));
        }

        public static void QueryOpenMessages(this RepetierConnection rc)
        {
            rc.SendCommand(MessagesCommand.Instance, typeof(MessagesCommand));
        }
    }
}

using RepetierMqtt.Models.Commands;
using RepetierMqtt.Util;
using System;
using System.Collections.Generic;
using System.Text;

namespace RepetierMqtt
{
    public static class RepetierConnectionExtentions
    {
        /// <summary>
        ///  Send a single "ping" message to the repetier server.
        /// </summary>
        public static void SendPing(this RepetierConnection rc)
        {
            rc.SendCommand(PingCommand.Instance);
        }

        /// <summary>
        /// Send a single "listPrinters" message to the repetier rerver.
        /// The response to a "listPrinters" command contains the current print progress.
        /// </summary>
        public static void SendListPrinters(this RepetierConnection rc)
        {
            rc.SendCommand(ListPrinterCommand.Instance);
        }

        /// <summary>
        /// Send a single "stateList" message to the repetier server.
        /// The response to a "stateList" command contains information regarding the printer state.
        /// </summary>
        public static void SendStateList(this RepetierConnection rc, bool includeHistory = false)
        {
            rc.SendCommand(new StateListCommand(includeHistory));
        }

        /// <summary>
        /// Send a single "stopJob" meassage to repetier server.
        /// The printer will stop the current print and triggers a "jobKilled" event
        /// </summary>
        public static void SendStopJob(this RepetierConnection rc)
        {
            rc.SendCommand(StopJobCommand.Instance);
        }

        public static void Logout(this RepetierConnection rc)
        {
            rc.SendCommand(LogoutCommand.Instance);
        }

        public static void EnqueueJob(this RepetierConnection rc, int modelId, bool autostart = true)
        {
            rc.SendCommand(new CopyModelCommand(modelId, autostart));
        }

        public static void GetModelInfo(this RepetierConnection rc, int modelId)
        {
            rc.SendCommand(new ModelInfoCommand(modelId));
        }

        public static void GetJobInfo(this RepetierConnection rc, int jobId)
        {
            rc.SendCommand(new JobInfoCommand(jobId));
        }

        public static void StartJob(this RepetierConnection rc, int jobId)
        {
            rc.SendCommand(new StartJobCommand(jobId));
        }

        public static void ContinueJob(this RepetierConnection rc)
        {
            rc.SendCommand(ContinueJobCommand.Instance);
        }

        public static void RemoveJob(this RepetierConnection rc, int jobId)
        {
            rc.SendCommand(new RemoveJobCommand(jobId));
        } 

        public static void ActivatePrinter(this RepetierConnection rc, string printerSlug)
        {
            rc.SendCommand(new ActivateCommand(printerSlug));
        }

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
    }
}

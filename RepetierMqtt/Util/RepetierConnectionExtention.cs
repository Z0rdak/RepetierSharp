using RepetierMqtt.Models.Commands;
using RepetierMqtt.Util;
using System;
using System.Collections.Generic;
using System.Text;

namespace RepetierMqtt
{
    public partial class RepetierConnection
    {
        /// <summary>
        ///  Send a single "ping" message to the repetier server.
        /// </summary>
        private void SendPing()
        {
            SendCommand(PingCommand.Instance);
        }

        /// <summary>
        /// Send a single "listPrinters" message to the repetier rerver.
        /// The response to a "listPrinters" command contains the current print progress.
        /// </summary>
        public void SendListPrinters()
        {
            SendCommand(ListPrinterCommand.Instance);
        }

        /// <summary>
        /// Send a single "stateList" message to the repetier server.
        /// The response to a "stateList" command contains information regarding the printer state.
        /// </summary>
        public void SendStateList(bool includeHistory = false)
        {
            SendCommand(new StateListCommand(includeHistory));
        }

        /// <summary>
        /// Send a single "stopJob" meassage to repetier server.
        /// The printer will stop the current print and triggers a "jobKilled" event
        /// </summary>
        public void SendStopJob()
        {
            SendCommand(StopJobCommand.Instance);
        }

        /// <summary>
        /// Attempt login with the previously set credentials
        /// </summary>
        public void Login()
        {
            if (!String.IsNullOrEmpty(LoginName) && !String.IsNullOrEmpty(Password))
            {
                Password = HashPassword(SessionKey, LoginName, Password);
                SendCommand(new LoginCommand(LoginName, Password));
            }
        }

        /// <summary>
        /// Attempt login with the given user and password
        /// </summary>
        /// <param name="user"></param>
        /// <param name="password"></param>
        public void Login(string user, string password)
        {
            if (!String.IsNullOrEmpty(SessionKey))
            {
                var pw = HashPassword(SessionKey, user, password);
                SendCommand(new LoginCommand(user, pw));
            }
        }

        private string HashPassword(string sessionKey, string login, string password)
        {
            return CommandManager.MD5(sessionKey + CommandManager.MD5(login + password));
        }

        public void Logout()
        {
            SendCommand(LogoutCommand.Instance);
        }

        public void EnqueueJob(int modelId, bool autostart = true)
        {
            SendCommand(new CopyModelCommand(modelId, autostart));
        }

        public void GetModelInfo(int modelId)
        {
            SendCommand(new ModelInfoCommand(modelId));
        }

        public void GetJobInfo(int jobId)
        {
            SendCommand(new JobInfoCommand(jobId));
        }

        public void StartJob(int jobId)
        {
            SendCommand(new StartJobCommand(jobId));
        }

        public void ContinueJob()
        {
            SendCommand(ContinueJobCommand.Instance);
        }

        public void RemoveJob(int jobId)
        {
            SendCommand(new RemoveJobCommand(jobId));
        }

        public void ActivatePrinter(string printerSlug)
        {
            SendCommand(new ActivateCommand(printerSlug));
        }

        public void DeactivatePrinter(string printerSlug)
        {
            SendCommand(new DeactivateCommand(printerSlug));
        }

        public void CreateUser(string user, string password, int permission)
        {
            SendCommand(new CreateUserCommand(user, password, permission));
        }

        public void UpdateUser(string user, int permission, string password = "")
        {
            SendCommand(new UpdateUserCommand(user, permission, password));
        }

        public void DeleteUser(string user)
        {
            SendCommand(new DeleteUserCommand(user));
        }
    }
}

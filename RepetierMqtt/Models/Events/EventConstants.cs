using System;
using System.Collections.Generic;
using System.Text;

namespace RepetierMqtt.Models.Events
{
    public static class EventConstants
    {
        public const string LOGOUT = "logout";
        public const string LOGIN_REQUIRED = "loginRequired";
        public const string USER_CREDENTIALS = "userCredentials";
        public const string PRINTER_LIST_CHANGED = "printerListChanged";
        public const string MESSAGES_CHANGED = "messagesChanged";
        public const string LOG = "log";
        public const string JOBS_CHANGED = "jobsChanged";
        public const string JOB_FINISHED = "jobFinished";
        public const string JOB_KILLED = "jobKilled";
        public const string JOB_STARTED = "jobStarted";
        public const string STATE = "state";
        public const string TEMP = "temp";
        public const string CHANGE_FILAMENT = "changeFilament";
    }
}

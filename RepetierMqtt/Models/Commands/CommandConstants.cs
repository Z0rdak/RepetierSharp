using System;
using System.Collections.Generic;
using System.Text;

namespace RepetierMqtt.Models.Commands
{
    public static class CommandConstants
    {
        public const string PING = "ping";
        public const string SEND = "send";
        public const string LOGIN = "login";
        public const string LOGOUT = "logout";
        public const string LIST_PRINTER = "listPrinter";
        public const string STATE_LIST = "stateList";
        public const string RESPONSE = "response";
        public const string MESSAGES = "messages";
        public const string REMOVE_MESSAGE = "removeMessage";
        public const string LIST_MODELS = "listModels";
        public const string LIST_JOBS = "listJobs";

        public const string COPY_MODEL = "copyModel";
        public const string MODEL_INFO = "modelInfo";
        public const string JOB_INFO = "jobInfo";
        public const string START_JOB = "startJob";
        public const string STOP_JOB = "stopJob";
        public const string CONTINUE_JOB = "continuejob";
        public const string REMOVE_JOB = "removeJob";
        public const string ACTIVATE = "activate";
        public const string DEACTIVATE = "deactivate";
        public const string CREATE_USER = "createUser";
        public const string UPDATE_USER = "updateUser";
        public const string DELETE_USER = "deleteUser";
        public const string USER_LIST = "userList";
        public const string EMERGENCY_STOP = "emergencyStop";

        
    }
}

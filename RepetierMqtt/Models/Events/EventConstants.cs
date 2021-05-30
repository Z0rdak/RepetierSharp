using System;
using System.Collections.Generic;
using System.Text;

namespace RepetierMqtt.Models.Events
{
    public static class EventConstants
    {
        public const string LOGIN_REQUIRED = "loginRequired";
        public const string LOGOUT = "logout";
        public const string USER_CREDENTIALS = "userCredentials";
        public const string PRINTER_LIST_CHANGED = "printerListChanged";
        public const string MESSAGES_CHANGED = "messagesChanged";
        public const string MOVE = "move";
        public const string LOG = "log";
        public const string JOBS_CHANGED = "jobsChanged";
        public const string JOB_FINISHED = "jobFinished";
        public const string JOB_KILLED = "jobKilled";
        public const string JOB_STARTED = "jobStarted";
        public const string PRINT_QUEUE_CHANGED = "printqueueChanged";
        public const string FOLDERS_CHANGED = "foldersChanged";
        public const string EEPROM_CLEAR = "eepromClear";
        public const string EEPROM_DATA = "eepromData";
        public const string STATE = "state";
        public const string CONFIG = "config";
        public const string FIRMWARE_CHANGED = "firmwareChanged";
        public const string TEMP = "temp";
        public const string SETTINGS_CHANGED = "settingsChanged";
        public const string PRINTER_SETTINGS_CHANGED = "printerSettingChanged";
        public const string MODEL_GROUPLIST_CHANGED = "modelGroupListChanged";
        public const string PREPARE_JOB = "prepareJob";
        public const string PREPARE_JOB_FINIHSED = "prepareJobFinished";
        public const string CHANGE_FILAMENT_REQUESTED = "changeFilamentRequested";
        public const string REMOTE_SERVERS_CHANGED = "remoteServersChanged";
        public const string GET_EXTERNAL_LINKS = "getExternalLinks";
    }
}

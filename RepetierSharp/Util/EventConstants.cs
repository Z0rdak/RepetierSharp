using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using RepetierSharp.Models.Events;

namespace RepetierSharp.Util
{
    public static class EventConstants
    {
        public const string LAYER_CHANGED = "layerChanged";
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
        public const string SETTING_CHANGED = "settingChanged";
        public const string PRINTER_CONDITION_CHANGED = "printerConditionChanged";
        public const string PRINTER_SETTING_CHANGED = "printerSettingChanged";
        public const string MODEL_GROUPLIST_CHANGED = "modelGroupListChanged";
        public const string PREPARE_JOB = "prepareJob";
        public const string PREPARE_JOB_FINIHSED = "prepareJobFinished";
        public const string CHANGE_FILAMENT_REQUESTED = "changeFilamentRequested";
        public const string REMOTE_SERVERS_CHANGED = "remoteServersChanged";
        public const string GET_EXTERNAL_LINKS = "getExternalLinks";
        public const string UPDATE_PRINTER_STATE = "updatePrinterState";
        public const string GLOBAL_ERRORS_CHANGED = "globalErrorsChanged";

        public const string TIMER_30 = "timer30";
        public const string TIMER_60 = "timer60";
        public const string TIMER_300 = "timer300";
        public const string TIMER_1800 = "timer1800";
        public const string TIMER_3600 = "timer3600";

        public static readonly ImmutableDictionary<string, Type> EventMap = ImmutableDictionary.CreateRange
        (new[]
            {
                KeyValuePair.Create("log", typeof(LogEntry)), KeyValuePair.Create("move", typeof(MoveEntry)),
                KeyValuePair.Create("layerChanged", typeof(LayerChanged))
            }
        );
    }
}

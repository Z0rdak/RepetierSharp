namespace RepetierSharp.Util
{
    public static class EventConstants
    {
        public const string LAYER_CHANGED = "layerChanged";
        public const string LOGIN_REQUIRED = "loginRequired";
        public const string LOGOUT = "logout";
        public const string USER_CREDENTIALS = "userCredentials";
        public const string PRINTER_LIST_CHANGED = "printerListChanged";
        public const string LICENSE_CHANGED = "licenceChanged";
        public const string MQTT_STATE_CHANGED = "mqttState";
        public const string MESSAGES_CHANGED = "messagesChanged";
        public const string MOVE = "move";
        public const string LOG = "log";
        public const string JOB_FINISHED = "jobFinished";
        public const string JOB_DEACTIVATED = "jobDeactivated";
        public const string JOB_KILLED = "jobKilled";
        public const string JOB_STARTED = "jobStarted";
        public const string ADD_ERROR_LOG_LINE = "addErrorLogLine";
        public const string PRINT_JOB_ADDED = "printJobAdded";
        public const string PRINT_QUEUE_CHANGED = "printqueueChanged"; // deprecated with 1.5
        public const string LAST_PRINTS_CHANGED = "lastPrintsChanged"; // deprecated with 1.5
        public const string JOBS_CHANGED = "jobsChanged"; // deprecated with 1.5
        public const string GCODE_STORAGE_CHANGED = "gcodestoragechanged"; // replaces jobsChanged and printqueueChanged
        public const string GCODE_INFO_UPDATED = "gcodeInfoUpdated"; // replaces jobsChanged and printqueueChanged
        public const string GCODE_ANALYSIS_FINISHED = "gcodeAnalysisFinished"; // replaces jobsChanged and printqueueChanged
        public const string FOLDERS_CHANGED = "foldersChanged";
        public const string EEPROM_CLEAR = "eepromClear";
        public const string EEPROM_DATA = "eepromData";
        public const string STATE = "state";
        public const string CONFIG = "config";
        public const string FIRMWARE_CHANGED = "firmwareChanged";
        public const string TEMP = "temp";
        public const string PONG = "pong";
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
        public const string RELOAD_KLIPPER = "reloadKlipper";
        public const string GPIO_PIN_CHANGED = "gpioPinChanged";
        public const string GPIO_LIST_CHANGED = "gpioListChanged";
        public const string HARDWARE_INFO = "hardwareInfo";
        public const string NEW_RENDER_IMAGE = "newRenderImage";
        public const string PROJECT_FOLDER_CHANGED = "projectFolderChanged";
        public const string PROJECT_STATE_CHANGED = "projectStateChanged";
        public const string RECOVER_CHANGED = "recoverChanged";
        public const string TIMELAPSE_CHANGED = "timelapseChanged";
        public const string WIFI_CHANGED = "wifiChanged";
        public const string WORKER_FINISHED = "workerFinished";
        public const string AUTO_UPDATE_STARTED = "autoupdateStarted";
        public const string EXTERNAL_LINKS_CHANGED = "externalLinksChanged";
        public const string PROJECT_CHANGED = "projectChanged";
        public const string PROJECT_DELETED = "projectDeleted";

        public const string TIMER_30 = "timer30";
        public const string TIMER_60 = "timer60";
        public const string TIMER_300 = "timer300";
        public const string TIMER_1800 = "timer1800";
        public const string TIMER_3600 = "timer3600";
        
        public const string DUET_DIALOG_OPENED = "duetDialogOpened";
        public const string DISPATCHER_COUNT = "dispatcherCount";

    }
}

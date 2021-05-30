using System;
using System.Collections.Generic;
using System.Text;

namespace RepetierMqtt.Models.Commands
{
    public static class CommandConstants
    {
        public const string PING = "ping";
        public const string EXTEND_PING = "extendPing";
        public const string LOGIN = "login";
        public const string LOGOUT = "logout";
        public const string LIST_PRINTER = "listPrinter";
        public const string SEND = "send";
        public const string STATE_LIST = "stateList";
        public const string RESPONSE = "response";
        public const string MOVE = "move";
        public const string MESSAGES = "messages";
        public const string REMOVE_MESSAGE = "removeMessage";
        public const string REMOVE_MODEL = "removeModel";
        public const string LIST_MODELS = "listModels";
        public const string COPY_MODEL = "copyModel";
        public const string LIST_JOBS = "listJobs";
        public const string MODEL_INFO = "modelInfo";
        public const string JOB_INFO = "jobInfo";
        public const string START_JOB = "startJob";
        public const string STOP_JOB = "stopJob";
        public const string CONTINUE_JOB = "continuejob";
        public const string REMOVE_JOB = "removeJob";
        public const string GET_PRINTER_CONFIG = "getPrinterConfig";
        public const string SET_PRINTER_CONFIG = "setPrinterConfig";
        public const string GET_SCRIPT = "getScript";
        public const string SET_SCRIPT = "setScript";
        public const string ACTIVATE = "activate";
        public const string DEACTIVATE = "deactivate";
        public const string COMMUNICATION_DATA = "communicationData";
        public const string GET_EEPROM = "getEeprom";
        public const string SET_EEPROM = "setEeprom";
        public const string LIST_EXTERNAL_COMMANDS = "listExternalCommands";
        public const string RUN_EXTERNAL_COMMAND = "runExternalCommand";
        public const string CREATE_CONFIGURATION = "createConfiguration";
        public const string REMOVE_CONFIGURATION = "removeConfiguration";
        public const string GET_SETTINGS = "getSettings";
        public const string UPDATE_SETTINGS = "updateSettings";
        public const string UPDATE_USER_SETTINGS = "updateUserSettings";
        public const string USER_LIST = "userList";
        public const string CREATE_USER = "createUser";
        public const string UPDATE_USER = "updateUser";
        public const string DELETE_USER = "deleteUser";
        public const string LIST_PORTS = "listPorts";
        public const string UPDATE_AVAILABLE = "updateAvailable";
        public const string IGNORE_UPDATE = "ignoreUpdate";
        public const string TEST_PUSH_MESSAGE = "testPushMessage";
        public const string LIST_FIRMWARE_NAMES = "listFirmwareNames";
        public const string START_IPDATE_FIRMWARE_SETTINGS = "startUpdateFirmwareSettings";
        public const string GET_FIRMWARE_SETTINGS = "getFirmwareSettings";
        public const string GET_FIRMWARE_DATA = "getFirmwareData";
        public const string LIST_LOGS = "listLogs";
        public const string REMOVE_LOG = "removeLog";
        public const string SET_LOG_LEVEL = "setLogLevel";
        public const string SEND_MOVES = "sendMoves";
        public const string HIDE_MOVES = "hideMoves";
        public const string SET_SPEED_MULTIPLY = "setSpeedMultiply";
        public const string SET_FLOW_MULTIPLY = "setFlowMultiply";
        public const string SET_FAN_SPEED = "setFanSpeed";
        public const string SET_EXTRUDER_TEMPERATURE = "setExtruderTemperature";
        public const string SET_BED_TEMPERATURE = "setBedTemperature";
        public const string SET_CHAMBER_TEMPERATURE = "setChamberTemperature";
        public const string EMERGENCY_STOP = "emergencyStop";
        public const string VERSION = "version";
        public const string GET_PRINTER_SETTING = "getPrinterSetting";
        public const string SET_PRINTER_SETTING = "setPrinterSetting";
        public const string GET_PRINTER_SETTINGS = "getPrinterSettings";
        public const string LIST_MODEL_GROUPS = "listModelGroups";
        public const string ADD_MODEL_GROUP = "addModelGroup";
        public const string DEL_MODEL_GROUP = "delModelGroup";
        public const string MOVE_MODEL_FILE_TO_GROUP = "moveModelFileToGroup";
        public const string LIST_FILESYSTEM_FOLDERS = "listFilesystemFolders";
        public const string GET_FOLDERS = "getFolders";
        public const string SET_FOLDERS = "setFolders";
        public const string BROWSE_FOLDER = "browseFolder";
        public const string IMPORT_URL = "importURL";
        public const string CAN_UPLOAD_GCODE_WITH_SIZE = "canUploadGCodeWithSize";
        public const string COOLDOWN = "cooldown";
        public const string PREHEAT = "preheat";
        public const string BABY_STEP = "babystep";
        public const string SEND_QUICK_COMMAND = "sendQuickCommand";

        // undocumented commands
        public const string FREE_SPACE = "freeSpace";
    }
}

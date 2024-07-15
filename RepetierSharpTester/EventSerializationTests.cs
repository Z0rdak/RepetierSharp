using System.Text.Json;
using System.Text.Json.Serialization;
using RepetierSharp.Models;
using RepetierSharp.Models.Events;
using RepetierSharp.Util;
using Xunit;

namespace RepetierSharpTester
{
    /// <summary>
    ///  Write tests for serialization tests for every type in RepetierSharp.Models.Events
    /// using RepetierSharp.Util.RepetierEventSerializer and xunit
    /// 
    /// </summary>
    public class EventSerializationTests
    {
        private static JsonSerializerOptions EventSerializationOptions = new JsonSerializerOptions
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Converters = { new RepetierBaseEventConverter() },
        };

        private static PrinterState ExamplePrinterState = new ()
        {
            ActiveExtruder = 0,
            AutostartNextPrint = false,
            DebugLevel = 6,
            IsDoorOpen = false,
            Extruders = new List<TemperatureEntry>()
            {
                new () { Error = 0, Out = 0.0, Read = 59.2, Set = 0.0 },
                new () { Error = 0, Out = 0.0, Read = 48.5, Set = 0.0 }
            },
            Fans = new List<Fan>()
            {
                new () { On = false, Voltage = 0 }
            },
            F = 1000.0,
            FilterFan = false,
            Firmware = "Marlin",
            FirmwareUrl = "https://www.evo-tech.eu/",
            FlowMultiplier = 100,
            HasXHome = true,
            HasYHome = true,
            HasZHome = true,
            Heatedbeds = new List<TemperatureEntry>()
            {
                new () { Error = 0, Out = 0.0, Read = 86.5, Set = 0.0 }
            },
            HeatedChambers = new List<TemperatureEntry>()
            {
                new () { Error = 0, Out = 200.0, Read = 39.7, Set = 0.0 }
            },
            CurrentLayer = 0,
            Lights = 0,
            Notification = "",
            ExtruderCount = 2,
            IsPowerOn = true,
            IsRecording = false,
            IsSDCardMounted = true,
            ShutDownAfterPrint = false,
            SpeedMultiplier = 100,
            IsVolumetric = false,
            Webcams = new List<Webcam>()
            {
                new () { Recording = false },
                new () { Recording = false }
            },
            X = 0.0, Y = 0.0, Z = 4.5
        };
        
        [Fact]
        public void ShouldSerializeUserCredentials()
        {
            // Arrange
            var obj = new UserCredentials
            {
                LoginName = "login",
                PermissionLevel = 1,
                Settings = new UserSettings()
                {
                    GCodeGroup = "{\"EVOlizer\":\"#\"}",
                    GCodeSortedBy = "0",
                    GCodeViewMode = "2",
                    TempDiagActive = "1",
                    TempDiagAll = "0",
                    TempDiagBed = "1",
                    TempDiagChamber = "1",
                    TempDiagMode = "0",
                    Theme = "dark",
                    Preview2DOptions =
                        "{\"showPrint\":true,\"showTravel\":true,\"showRetract\":true,\"showGcode\":2,\"extrusionWidth\":0.4,\"showExtruder\":false,\"show\":0}",
                    TimelapseViewFailed = "0",
                    TimelapseViewMode = "1",
                    PrinterOrder = "dynamic",
                    LeftPrinterList = "0"
                }
            };

            // Act
            var json =  JsonSerializer.Serialize(obj);
            var result = JsonSerializer.Deserialize<UserCredentials>(json, EventSerializationOptions);
            var json2 = JsonSerializer.Serialize(obj);
            
            Assert.NotNull(result);
            Assert.NotNull(json2);
            // Assert
            Assert.Equal(json, json2);
            Assert.Equal(obj.LoginName, result.LoginName);
            Assert.Equal(obj.PermissionLevel, result.PermissionLevel);
            // TODO Assert.Equal(obj.Settings, result.Settings);
   
        }

        [Fact]
        public void ShouldSerializeMoveEntry()
        {
            
            // Arrange
            var obj = new MoveEntry { X = 1, Y = 2, Z = 3, Exruder = 4, Speed = 5, RelativeMove = true };

            // Act
            var json =  JsonSerializer.Serialize(obj);
            var result = JsonSerializer.Deserialize<MoveEntry>(json, EventSerializationOptions);
            var json2 =  JsonSerializer.Serialize(obj);

            Assert.NotNull(result);
            Assert.NotNull(json2);
            // Assert
            Assert.Equal(obj.X, result.X);
            Assert.Equal(obj.Y, result.Y);
            Assert.Equal(obj.Z, result.Z);
            Assert.Equal(obj.Exruder, result.Exruder);
            Assert.Equal(obj.Speed, result.Speed);
            Assert.Equal(obj.RelativeMove, result.RelativeMove);
        }

        [Fact]
        public void ShouldSerializeLogEntry()
        {
            // Arrange
            var obj = new LogEntry { Id = 0, Message = "message", LogType = LogType.Commands, Timestamp = 1721030739 };

            // Act
            var json =  JsonSerializer.Serialize(obj);
            var result = JsonSerializer.Deserialize<LogEntry>(json, EventSerializationOptions);
            var json2 =  JsonSerializer.Serialize(obj);

            Assert.NotNull(result);
            Assert.NotNull(json2);
            // Assert
            Assert.Equal(obj.Message, result.Message);
            Assert.Equal(obj.Id, result.Id);
            Assert.Equal(obj.LogType, result.LogType);
            Assert.Equal(obj.Timestamp, result.Timestamp);
        }

        [Fact]
        public void ShouldSerializeGcodeInfoUpdated()
        {
            // Arrange
            var obj = new GcodeInfoUpdated
            {
                ModelId = 0,
                ModelPath = "/this/is/a/path.lol",
                Slug = "EVOlizer"
            };

            // Act
            var json =  JsonSerializer.Serialize(obj);
            var result = JsonSerializer.Deserialize<GcodeInfoUpdated>(json, EventSerializationOptions);
            var json2 = JsonSerializer.Serialize(obj);

            Assert.NotNull(result);
            Assert.NotNull(json2);
            // Assert
            Assert.Equal(obj.ModelId, result.ModelId);
            Assert.Equal(obj.ModelPath, result.ModelPath);
            Assert.Equal(obj.Slug, result.Slug);
        }
        
        
        [Fact]
        public void ShouldSerializeProjectFolderChanged()
        {
            // Arrange
            var obj = new ProjectFolderChanged { Idx = 1, ServerUuid = "server_uuid", Version = 2 };

            // Act
            
            var json =  JsonSerializer.Serialize(obj);
            var result = JsonSerializer.Deserialize<ProjectFolderChanged>(json, EventSerializationOptions);
            var json2 =  JsonSerializer.Serialize(obj);

            Assert.NotNull(result);
            Assert.NotNull(json2);
            // Assert
            Assert.Equal(obj.Idx, result.Idx);
            Assert.Equal(obj.ServerUuid, result.ServerUuid);
            Assert.Equal(obj.Version, result.Version);
        }
        
        [Fact]
        public void ShouldSerializeGpioPinChanged()
        {
            // Arrange
            var obj = new GpioPinChanged { Uuid = "uuid", State = true, PwmDutyCycle = 1 };

            // Act
            
            var json =  JsonSerializer.Serialize(obj);
            var result = JsonSerializer.Deserialize<GpioPinChanged>(json, EventSerializationOptions);
            var json2 =  JsonSerializer.Serialize(obj);
            
            Assert.NotNull(result);
            Assert.NotNull(json2);
            // Assert
            Assert.Equal(obj.Uuid, result.Uuid);
            Assert.Equal(obj.State, result.State);
            Assert.Equal(obj.PwmDutyCycle, result.PwmDutyCycle);
        }
        
        [Fact]
        public void ShouldSerializeProjectStateChanged()
        {
            // Arrange
            var obj = new ProjectStateChanged { Uuid = "uuid", Version = 1 };

            // Act
            
            var json =  JsonSerializer.Serialize(obj);
            var result = JsonSerializer.Deserialize<ProjectStateChanged>(json, EventSerializationOptions);
            var json2 =  JsonSerializer.Serialize(obj);

            Assert.NotNull(result);
            Assert.NotNull(json2);
            // Assert
            Assert.Equal(obj.Uuid, result.Uuid);
            Assert.Equal(obj.Version, result.Version);
        }
        
        [Fact]
        [EventId("state", "updatePrinterState")]
        public void ShouldSerializePrinterState()
        {
            // Arrange
            var obj = ExamplePrinterState;

            // Act
            var json =  JsonSerializer.Serialize(obj);
            var result = JsonSerializer.Deserialize<PrinterState>(json, EventSerializationOptions);
            var json2 =  JsonSerializer.Serialize(obj);

            Assert.NotNull(result);
            Assert.NotNull(json2);
            // Assert
            // TODO: Assert.Equal(obj, result);
        }
        
    }
}

using System.Text.Json;
using System.Text.Json.Serialization;
using RepetierSharp.Config;
using RepetierSharp.Models;
using RepetierSharp.Models.Common;
using RepetierSharp.Models.Config;
using RepetierSharp.Models.Events;
using RepetierSharp.Models.Responses;
using RepetierSharp.Serialization;
using RepetierSharp.Util;

namespace RepetierSharp.Tests
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
            Webcams = new List<WebcamState>()
            {
                new () { Recording = false },
                new () { Recording = false }
            },
            X = 0.0, Y = 0.0, Z = 4.5
        };

        private static PrinterConfig ExampleConfig = new PrinterConfig()
        {
            
        };
        
        private static Printer ExamplePrinter = new Printer()
        {
            
        };
        
        private static TimeLapseEntry ExampleTimeLapseEntry = new TimeLapseEntry()
        {
            Bitrate = 1000,
            ConversionError = "error",
            ConversionMode = 1,
            CreatedSeconds = 1721076191,
            Dir = "/this/is/a/path.lol",
            Framerate = 30,
            ImageCounter = 1000,
            Name = "timelapse",
            Valid = true,
            VideoLength = 1000,
            WebcamId = 1
        };
        
        private static TimeLapseEntry ExampleTimeLapseEntry1 = new TimeLapseEntry()
        {
            Bitrate = 1000,
            ConversionError = "error",
            ConversionMode = 1,
            CreatedSeconds = 1721076191,
            Dir = "/this/is/a/path.lol",
            Framerate = 30,
            ImageCounter = 1000,
            Name = "timelapse",
            Valid = true,
            VideoLength = 1000,
            WebcamId = 0
        };
        
        [Fact]
        [EventId("loginRequired")]
        public void ShouldSerializeLoginRequired()
        {
            // Arrange
            var obj = new LoginRequired();

            // Act
            var json =  JsonSerializer.Serialize(obj);
            var result = JsonSerializer.Deserialize<LoginRequired>(json, EventSerializationOptions);
            var json2 =  JsonSerializer.Serialize(obj);

            Assert.NotNull(result);
            Assert.NotNull(json2);
            // Assert
            Assert.Equal(obj.SessionId, result.SessionId);
            Assert.Equal(json, json2);
        }
        
        [Fact]
        [EventId("userCredentials")]
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
            Assert.Equivalent(obj.Settings, result.Settings);
   
        }

        [Fact]
        [EventId("printerListChanged")]
        public void ShouldSerializePrinterListChanged()
        {
            // Arrange
            var obj = new PrinterListChanged
            {
                Printers = new List<Printer>()
                {
                    
                }
            };

            // Act
            var json =  JsonSerializer.Serialize(obj);
            var result = JsonSerializer.Deserialize<PrinterListChanged>(json, EventSerializationOptions);
            var json2 =  JsonSerializer.Serialize(obj);

            Assert.NotNull(result);
            Assert.NotNull(json2);
            // Assert
            Assert.Equal(json, json2);
            Assert.Equal(obj.Printers, result.Printers);
        }
        
        [Fact]
        [EventId("messagesChanged")]
        public void ShouldSerializeMessagesChanged()
        {
            // Arrange
            var obj = new List<Message>() 
                {
                    new Message 
                    { 
                        Id = 0, 
                        DateString = "Date ISO 8601",
                        FinishLink = "link",
                        IsPaused = true,
                        Msg = "msg",
                        PrinterSlug = "slug"
                    }
                };

            // Act
            var json =  JsonSerializer.Serialize(obj);
            var result = JsonSerializer.Deserialize<List<Message>>(json, EventSerializationOptions);
            var json2 =  JsonSerializer.Serialize(obj);

            Assert.NotNull(result);
            Assert.NotNull(json2);
            // Assert
            Assert.Equal(json, json2);
            
        }
        
        [Fact]
        [EventId("move")]
        public void ShouldSerializeMoveEntry()
        {
            
            // Arrange
            var obj = new MoveEntry { X = 1, Y = 2, Z = 3, Extruder = 4, Speed = 5, RelativeMove = true };

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
            Assert.Equal(obj.Extruder, result.Extruder);
            Assert.Equal(obj.Speed, result.Speed);
            Assert.Equal(obj.RelativeMove, result.RelativeMove);
        }

        [Fact]
        [EventId("log")]
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
        [EventId("gcodeInfoUpdated")]
        public void ShouldSerializeGcodeInfoUpdated()
        {
            // Arrange
            var obj = new GcodeInfo
            {
                ModelId = 0,
                ModelPath = "/this/is/a/path.lol",
                Slug = "EVOlizer"
            };

            // Act
            var json =  JsonSerializer.Serialize(obj);
            var result = JsonSerializer.Deserialize<GcodeInfo>(json, EventSerializationOptions);
            var json2 = JsonSerializer.Serialize(obj);

            Assert.NotNull(result);
            Assert.NotNull(json2);
            // Assert
            Assert.Equal(obj.ModelId, result.ModelId);
            Assert.Equal(obj.ModelPath, result.ModelPath);
            Assert.Equal(obj.Slug, result.Slug);
        }
        
        [Fact]
        [EventId("jobFinished", "jobKilled", "jobDeactivated")]
        public void ShouldSerializeJobState()
        {
            // Arrange
            var obj = new JobState 
            {
                StartTime = 1721076191,
                Duration = 12346,
                EndTime = 1721088537L,
                PrintedLines = 234,
            };

            // Act
            var json =  JsonSerializer.Serialize(obj);
            var result = JsonSerializer.Deserialize<JobState>(json, EventSerializationOptions);
            var json2 =  JsonSerializer.Serialize(obj);

            Assert.NotNull(result);
            Assert.NotNull(json2);
            // Assert
            Assert.Equal(obj.StartTime, result.StartTime);
            Assert.Equal(obj.Duration, result.Duration);
            Assert.Equal(obj.EndTime, result.EndTime);
            Assert.Equal(obj.PrintedLines, result.PrintedLines);
        }
        
        [Fact]
        [EventId("jobStarted")]
        public void ShouldSerializeJobStarted()
        {
            // Arrange
            var obj = new JobStarted
            {
                StartTime = 1721076191,
            };

            // Act
            var json =  JsonSerializer.Serialize(obj);
            var result = JsonSerializer.Deserialize<JobStarted>(json, EventSerializationOptions);
            var json2 =  JsonSerializer.Serialize(obj);

            Assert.NotNull(result);
            Assert.NotNull(json2);
            // Assert
            Assert.Equal(obj.StartTime, result.StartTime);
        }
        
        [Fact]
        [EventId("printerConditionChanged")]
        public void ShouldSerializePrinterConditionChanged()
        {
            // Arrange
            var obj = new PrinterConditionChanged
            {
                Condition = PrinterCondition.Ready,
                Reason = "reason"
            };

            // Act
            var json =  JsonSerializer.Serialize(obj);
            var result = JsonSerializer.Deserialize<PrinterConditionChanged>(json, EventSerializationOptions);
            var json2 =  JsonSerializer.Serialize(obj);

            Assert.NotNull(result);
            Assert.NotNull(json2);
            // Assert
            Assert.Equal(obj.Condition, result.Condition);
        }
        
        [Fact]
        [EventId("eepromData")]
        public void ShouldSerializeEepromData()
        {
            // Arrange
            var obj = new EepromData
            {
                Pos = "pos",
                Text = "text",
                Type = "type",
                Value = "value",
                ValueOrig = "valueOrig"
            };

            // Act
            var json =  JsonSerializer.Serialize(obj);
            var result = JsonSerializer.Deserialize<EepromData>(json, EventSerializationOptions);
            var json2 =  JsonSerializer.Serialize(obj);

            Assert.NotNull(result);
            Assert.NotNull(json2);
            // Assert
            Assert.Equal(obj.Pos, result.Pos);
            Assert.Equal(obj.Text, result.Text);
            Assert.Equal(obj.Type, result.Type);
            Assert.Equal(obj.Value, result.Value);
            Assert.Equal(obj.ValueOrig, result.ValueOrig);
        }
        
        [Fact]
        [EventId("config")]
        public void ShouldSerializeConfig()
        {
            // Arrange
            var obj = ExampleConfig;

            // Act
            var json =  JsonSerializer.Serialize(obj);
            var result = JsonSerializer.Deserialize<PrinterConfig>(json, EventSerializationOptions);
            var json2 =  JsonSerializer.Serialize(obj);

            Assert.NotNull(result);
            Assert.NotNull(json2);
            // Assert
            Assert.Equivalent(obj, result);
        }
        
        [Fact]
        [EventId("firmwareChanged")]
        public void ShouldSerializeFirmwareChanged()
        {
            // Arrange
            var obj = new FirmwareData
            {
                Firmware = new FirmwareInfo()
                {
                    Eeprom = "eeprom",
                    Firmware = "firmware",
                    FirmwareURL = "firmwareURL",
                    Name = "name"
                }
            };

            // Act
            var json =  JsonSerializer.Serialize(obj);
            var result = JsonSerializer.Deserialize<FirmwareData>(json, EventSerializationOptions);
            var json2 =  JsonSerializer.Serialize(obj);

            Assert.NotNull(result);
            Assert.NotNull(json2);
            // Assert
            Assert.Equivalent(obj.Firmware, result.Firmware);
        }
        
        [Fact]
        [EventId("temp")]
        public void ShouldSerializeTemp()
        {
            // Arrange
            var obj = new TempEntry()
            {
                Id = 1,
                Timestamp = 1721076191,
                Measured = 123.2345d,
                Output = 234.3456d,
                Set = 345.4567d,
            };

            // Act
            var json =  JsonSerializer.Serialize(obj);
            var result = JsonSerializer.Deserialize<TempEntry>(json, EventSerializationOptions);
            var json2 =  JsonSerializer.Serialize(obj);

            Assert.NotNull(result);
            Assert.NotNull(json2);
            // Assert
            Assert.Equal(obj.Id, result.Id);
            Assert.Equal(obj.Timestamp, result.Timestamp);
            Assert.Equal(obj.Measured, result.Measured);
            Assert.Equal(obj.Output, result.Output);
            Assert.Equal(obj.Set, result.Set);
        }
        
        [Fact]
        [EventId("settingChanged")]
        public void ShouldSerializeSettingChanged()
        {
            // Arrange
            var obj = new SettingChanged
            {
                
            };

            // Act
            var json =  JsonSerializer.Serialize(obj);
            var result = JsonSerializer.Deserialize<SettingChanged>(json, EventSerializationOptions);
            var json2 =  JsonSerializer.Serialize(obj);

            Assert.NotNull(result);
            Assert.NotNull(json2);
            // Assert
            
        }
        
              
        [Fact]
        [EventId("printerSettingChanged")]
        public void ShouldSerializePrinterSettingChanged()
        {
            // Arrange
            var obj = new PrinterSettingChanged {  };

            // Act
            var json =  JsonSerializer.Serialize(obj);
            var result = JsonSerializer.Deserialize<PrinterSettingChanged>(json, EventSerializationOptions);
            var json2 =  JsonSerializer.Serialize(obj);

            Assert.NotNull(result);
            Assert.NotNull(json2);
            // Assert
            
        }
        
        [Fact]
        [EventId("timelapseChanged")]
        public void ShouldSerializeTimelapseChanged()
        {
            // Arrange
            var obj = new TimelapseChanged
            {
                Running = true,
                RunningEntries = new List<TimeLapseEntry>() {ExampleTimeLapseEntry, ExampleTimeLapseEntry1},
                Timelapses = new List<TimeLapseEntry>() {ExampleTimeLapseEntry, ExampleTimeLapseEntry1}
            };

            // Act
            var json =  JsonSerializer.Serialize(obj);
            var result = JsonSerializer.Deserialize<TimelapseChanged>(json, EventSerializationOptions);
            var json2 =  JsonSerializer.Serialize(obj);

            Assert.NotNull(result);
            Assert.NotNull(json2);
            // Assert
            Assert.Equal(obj.Running, result.Running);
            for ( int i = 0; i < obj.RunningEntries.Count; i++ )
            {
                Assert.Equivalent(obj.RunningEntries[i], result.RunningEntries[i]);
            }
            for ( int i = 0; i < obj.Timelapses.Count; i++ )
            {
                Assert.Equivalent(obj.Timelapses[i], result.Timelapses[i]);
            }
        }
        
        
        [Fact]
        [EventId("duetDialogOpened")]
        public void ShouldSerializeDuetDialogOpened()
        {
            // Arrange
            var obj = new DuetDialogOpened
            {
                Message = "message",
                Mode = 1,
                Seq = 2,
                Timeout = 3,
                Title = "title",
                DialogId = 4
            };

            // Act
            var json =  JsonSerializer.Serialize(obj);
            var result = JsonSerializer.Deserialize<DuetDialogOpened>(json, EventSerializationOptions);
            var json2 =  JsonSerializer.Serialize(obj);

            Assert.NotNull(result);
            Assert.NotNull(json2);
            // Assert
            Assert.Equal(obj.Message, result.Message);
            Assert.Equal(obj.Mode, result.Mode);
            Assert.Equal(obj.Seq, result.Seq);
            Assert.Equal(obj.Timeout, result.Timeout);
            Assert.Equal(obj.Title, result.Title);
            Assert.Equal(obj.DialogId, result.DialogId);
        }
        
        [Fact]
        [EventId("projectFolderChanged")]
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
        [EventId("gpioPinChanged")]
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
        [EventId("projectChanged", "projectDeleted")]
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
        [EventId("layerChanged")]
        public void ShouldSerializeLayerChanged()
        {
            // Arrange
            var obj = new LayerChanged { Layer = 1 };

            // Act
            var json =  JsonSerializer.Serialize(obj);
            var result = JsonSerializer.Deserialize<LayerChanged>(json, EventSerializationOptions);
            var json2 =  JsonSerializer.Serialize(obj);

            Assert.NotNull(result);
            Assert.NotNull(json2);
            // Assert
            Assert.Equal(obj.Layer, result.Layer);
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
            Assert.Equivalent(obj, result);
        }
        
    }
}

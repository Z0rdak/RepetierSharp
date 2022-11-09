# RepetierSharp - A simple, event driven [Repetier Server](https://www.repetier-server.com/ "Repetier Server") client

![NuGet downloads](https://img.shields.io/nuget/dt/RepetierSharp)
![Nuget version](https://img.shields.io/nuget/v/RepetierSharp)
[![Donate](https://img.shields.io/badge/Donate-PayPal-green.svg)](https://www.paypal.com/donate/?hosted_button_id=9DEWUBKE4YT3S)

## Introduction

RepetierSharp is a simple, event-driven client which encapsulates the WebSocket API (as well as the REST API where needed) to control the Repetier Server.

### What is Repetier Server?

> "Repetier-Server is the professional all-in-one solution to control and manage your 3d printers and to get the most out of it."

https://www.repetier-server.com/

### Versioning

This library is up to date with RepetierServer version 1.2.0. The serialization for most commands and events should be working with earlier versions, but there is the possibilty of crashes when using RepetierSharp with earlier versions due to undocumented changes.

Note that you are still able to use RepetierSharp by just using the version independent event handlers for events and command responses: `OnRawEvent(string eventName, string printer, byte[] payload)` and `OnRawResponse(int callbackID, string command, byte[] response)` respectively.

See the Changelo.md for an more detailed overview off added/changed features.

### Framework support

Currently RepetierSharp supports .NET Core 3.1, .NET 5 and .NET 6.

## Getting started

**DISCLAIMER:** *RepetierSharp is still in beta. Bugs are to be expected - please help improving RepetierSharp by submitting issues on [GitHub](https://github.com/Z0rdak/RepetierSharp/issues).*

The following sections show some examples on how to use the RepetierSharp client. The examples are not exhaustive. I will try to write a more thourough documentation as soon as possible.

RepetierSharp uses a fluent builder api to reduce the complexity when creating a new client instance.

### Establish a connection

Simply start by creating an instance of the `RepetierConnection` class. To establish a connection it is possible to provide the Repetier Server API-Key or user credentials.

The most basic configuration to setup a working `RepetierConnection` looks like this:

```csharp
RepetierConnection rc = new RepetierConnectionBuilder()
	.WithHost("demo.repetier-server.com", 4000)
	.Build();

rc.Connect();
```

This gives you access to the repetier server with the global user profile.

In most cases you would want to create a connection by suppling a API-Key or user credentials:

Example using user credentials:
```csharp
RepetierConnection rc = new RepetierConnectionBuilder()
	.WithHost("demo.repetier-server.com:4000")
	.WithCredentials("user", "password", rememberSession: true)
	.Build();

rc.Connect();
```

Example using a Repetier Server API-Key:
```csharp
RepetierConnection rc = new RepetierConnectionBuilder()
	.WithHost("demo.repetier-server.com", 4000)
	.WithApiKey("6ed22859-9e72-4f24-928f-0430ef08e3b9")
	.Build();

rc.Connect();
```
When both, API-Key and user credentials are supplied, the last option will be used.

### More examples

Create a connection, register an event handler for successfull connection, which activates the printer with the slug "Delta" and enqueues and starts the job with the id 6.

```csharp
RepetierConnection rc = new RepetierConnectionBuilder()
	.WithHost("demo.repetier-server.com", 4000)
	.WithApiKey("6ed22859-9e72-4f24-928f-0430ef08e3b9")
	.Build();

rc.OnRepetierConnected += () =>
{
	Console.WriteLine("Connected!");
	rc.ActivatePrinter("Delta");
	rc.EnqueueJob(6);
};

rc.Connect();
```

## Features

The following commands and events are named after the Repetier Server commands/events from the API documentation
At the moment RepetierSharp supports the following features:

### Events

To get notified about repetier events it is possible to register and event handler like this:
```csharp
rc.OnEvent += (eventName, printer, eventData) => 
{
	// handle event
};
```

Where `eventData` is a `IRepetierEvent` instance. The `eventName` can be used to determine the event and cast the event data to the corresponding type provided in the RepetierSharp namespace.

At the moment the following repetier events are supported:

<details>
  <summary>**[Click to expand the list of supported events]**</summary>

  - timerX
- loginRequired
- userCredentials
- printerListChanged
- messagesChanged
- jobFinished
- jobKilled
- jobStarted
- eepromData
- state
- config
- firmwareChanged
- temp
- printerSettingChanged
- jobsChanged
- logout
- printQueueChanged
- foldersChanged
- eepromClear
- modelGroupListChanged
- prepareJob
- prepareJobFinished
- changeFilamentRequested
- remoteServersChanged
- getExternalLinks
  </details>

Since there are many events and serialization for all events is still not implemented RepetierSharp also provides an event handler for the raw event data:

```csharp
rc.OnRawEvent += (eventName, printer, eventData) => 
{
	// handle eventData
};
```

Where `eventData` is of type `byte[]` and contains the data from the `data` field of the event from original json sent by the server (see [documentation](https://prgdoc.repetier-server.com/v1/docs/index.html#/en/web-api/websocket/events)). 

### Commands

To get responses for the sent commands it is possible to register event handler similar as for the events:
```csharp
rc.OnResponse += (callbackId, command, response) =>
{
	// handle response
};
```

Where `callbackId` is the id corresponding to the sent command, `command` is the name of the command and `response` of the type `IRepetierMessage` is the actual response data. This data can be cast to the corresponding type by determining the command and using the provided types within the namespace - analogous to the events.

At the moment the following commands (inclusive responses) are suppored: 

<details>
  <summary>**[Click to expand the list of supported commands]**</summary>

- login
- logout
- listPrinter
- stateList
- messages
- listModels
- listJobs
- modelInfo
- jobInfo
- removeJob
- send
- copyModel
- emergencyStop
- activate
- deactivate
- updateUser
- createUser
- deleteUser
- userList
- startJob
- stopJob
- continueJob
</details>

Analogous to the events there are many commands and serialization for all is not yet implemented. Therefore RepetierSharp also provides an event handler for the raw command response data:

```csharp
rc.OnRawResponse += (callbackId, command, responseData) => 
{
	// handle command response
};
```

Where `responseData` is of type `byte[]` and contains the data from the `data` field of the command from original json sent by the server (see [documentation](https://prgdoc.repetier-server.com/v1/docs/index.html#/en/web-api/websocket/index)). 

### REST-API

Additionally there are some functions levering the REST-API directly:

- To start a print directly use:

```csharp
rc.UploadAndStartPrint("/path/to/gcode/file.gcode", "printerSlug");
```

```csharp
byte[] file = Files.ReadAllBytes("/path/to/gcode/file.gcode"):
rc.UploadAndStartPrint("file.gcode", fileBytes, "printerSlug");
```

- or just upload a gcode file by using:

```csharp
rc.UploadGCode("/path/to/gcode/file.gcode", "printerSlug", "group");
```

```csharp
byte[] file = Files.ReadAllBytes("/path/to/gcode/file.gcode"):
rc.UploadGCode("file.gcode", fileBytes, "printerSlug", "group");
```


### Event handler

Beside the already discussed event handlers for events and command responses there are some additional implemented handlers for common used events (to reduces the effort of checking for these events and casting the data):

- OnLogReceived
- OnJobFinishedReceived
- OnJobStartedReceived
- OnJobKilledReceived
- OnJobsChanged
- OnPrinterStateReceived
- OnTempChangeReceived
- OnPrinterListChanged
- OnPrinterSettingsChanged
- OnUserCredentialsReceived
- OnLoginRequired
- OnMessagesReceived
- OnLoginReceived
- OnPrinterListReceived
- OnModelListReceived
- OnModelInfoReceived
- OnJobInfoReceived
- OnJobListReceived

### Cyclic command execution

The Repetier Server web interface uses some commands to cyclic query information from the server. The same is possible with RepetierSharp:

```csharp
RepetierConnection rc = new RepetierConnectionBuilder()
	.WithHost("demo.repetier-server.com", 4000)
	.WithApiKey("6ed22859-9e72-4f24-928f-0430ef08e3b9")
	.QueryPrinterInterval(RepetierTimer.Timer60)
	.QueryStateInterval(RepetierTimer.Timer30)
	.Build();
```

In lines 4 and 5 a cyclic call is added to query the printers and the printer status respectively. The RepetierTimer parameters correspond to the timer events of the Repetier server. The Timer30 event is triggered every 30 seconds, the Timer60 event every 60 seconds.
This puts the queries in a queue whose contents are executed each time the corresponding event is triggered by the server.

Additionaly there is also a method to add any other commands to these command queues: 

```csharp
RepetierConnection rc = new RepetierConnectionBuilder()
	.WithHost("demo.repetier-server.com", 4000)
	.WithApiKey("6ed22859-9e72-4f24-928f-0430ef08e3b9")
	.WithCyclicCommand(RepetierTimer.Timer3600, UpdateAvailableCommand.Instance)
	.Build();
```
In line 4 the command `updateAvailable` is added to the 1 hour timer queue so every hour RepetierSharp queries if there is a update for the Repetier Server available.

## Documentation

The wiki is still under construction. If you have any issues feel free to open an issue.

## Roadmap

The goal for this project is to add most (or even all) used WebSocket commands and events provided by the Repetier Server API to enable a full programmatic control of the Repetier Server.

There are also some events and commands that are not documented in the API - with some time and effort, I will try to reconstruct these and integrate them into RepetierSharp.

There is also a [MQTT client](https://github.com/Z0rdak/RepetierMqtt) using this library, to forward the events and command responses to a configurable broker. Since version 1.3 Repetier Server brings native MQTT support, which is very similar to the interface RepetierMqtt is providing.

## Contribution

If you have any issues or found bugs feel free to open an issue.

# RepetierSharp - Simple, event driven .NET [Repetier Server](https://www.repetier-server.com/ "Repetier Server") client


## Introduction

RepetierSharp is a simple, event-driven client which encapsulates the WebSocket API (as well as the REST API where needed) to control the Repetier Server.

### What is Repetier Server?

> "Repetier-Server is the professional all-in-one solution to control and manage your 3d printers and to get the most out of it."

https://www.repetier-server.com/

### Problem

I quite enjoy using the Repetier Server software to control/manage our 3d printers at my work. What I was not enjoying is the lack of a proper API documentation and needed a client which leverages the Repetier Server API. This was needed because I wanted to control the Repetier Server programmatically to automate certain tasks.

Unfortunally the [Repetier Server Web API](https://www.repetier-server.com/manuals/programming/API/index.html "Repetier Server Web API") is incomplete and poorly structured.

But luckily it is possible to reverse engineer the WebSocket API by using the Repetier Server frontend which uses the same API to communicate with the server.
information given by the  as well as some information gathered by reverse engineering the API by using the Repetier Server web interface to write a client which solves some of my requirements.

A (work-in-progress) [AsyncAPI documentation](https://www.asyncapi.com/ "AsyncAPI documentation") for the Repetier Server WebSocket API can be found [here](TODO).

## Features

* Event-driven interface provides real time print job information like
	* printer state
	* temperatures
	* job progress
	* ...
* Methods for the most common used tasks
	* upload gcode
	* start print job
	* pause and resume print job
	* stop print job
	* emergency stop
	* ...

## Getting started

The following sections show some examples on how to use the RepetierSharp client. The examples are not exhaustive. Please refer to the documentation for a full overview.

### Establish a connection
Simply start by creating an instance from the `RepetierConnection` class. To establish a connection it is possible to provide the Repetier Server API-Key or by using defined user credentials.

Example using user credentials:
```csharp
var RepetierConnection = new RepetierConnection("127.0.0.1:3344", "User", "pa55w0rd");

 RepetierConnection.Connect();
```

Example using a Repetier Server API-Key:
```csharp
var rc = new RepetierConnection("127.0.0.1:3344", "7075e377-7472-447a-b77e-86d481995e7b");

 RepetierConnection.Connect();
```

### Event handler

To receive information about certain events or print job information use the corresponding event handler.

```csharp
rc.OnJobStartedReceived += (printerName, jobStartedEvent, timestamp) =>
{
    /* */
};

rc.OnJobKilledReceived += (printerName, jobKilledEvent, timestamp) =>
{
    /* */
};

rc.OnJobFinishedReceived += (printer, jobFinishedEvent, timestamp) =>
{
    Console.WriteLine($"[Event]: Print job finished at {timestamp}");
    Console.WriteLine($"Printer name: {printer}");
    Console.WriteLine($"Started at: {jobFinishedEvent.StartTime}");
    Console.WriteLine($"Finished at: {jobFinishedEvent.EndTime}");
    Console.WriteLine($"Print duration: {jobFinishedEvent.Duration}");
    Console.WriteLine($"Total printed lines: {jobFinishedEvent.PrintedLines}");
};

rc.OnChangeFilamentReceived += (printer, timestamp) =>
{
    /* */
};

rc.OnTempChangeReceived += (printer, tempChangeEvent, timestamp) =>
{
    /* Information about extruder temperature */
};

```

### Commands

Upload and start a print:

```csharp
rc.UploadAndStartPrint($@"/path/to/gcode/file.gcode", "printerSlug");
```

Start a already uploaded job (gcode):
```csharp
rc.StartJob(jobId);
```

Activate a printer (some commands target the currently active printer):
```csharp
rc.ActivatePrinter(printerSlug);
```

Stop currently running print job:
```csharp
rc.StopJob();
```

Continue the currently stopped print job:
```csharp
rc.StopJob();
```

### Query information

Some commands are used to query information about the printers or information. The receive this information a corresponding event handler needs to be added first:

Query printer list (includes print job progress):
```csharp
rc.OnPrinterListReceived += (sender, printerList) => 
{
    /* QueryPrinterList queries the list of all printers
        * which contains information like print job progress */
};
rc.QueryPrinterList();
```

Query printer state:

```csharp
rc.OnPrinterStateReceived += (printerName, printerState, timestamp) =>
{
    /* QueryPrinterStateList queries the state of all printers
        * which includes information like temperatures of extruder, heatedbed and so on */
};
rc.QueryPrinterStateList();
```

## Documentation

TODO: Wiki

## Roadmap

The goal for this project is to add most (or even all) used WebSocket commands and events used by the Repetier Server frontend. This enables a full programmatic control of the Repetier Server.

* GCODE and Project management
* User management
* ...

## Contribute


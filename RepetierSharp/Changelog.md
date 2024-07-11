# [Unversioned] - YYYY-MM-DD

## Added

* RestSharp Authenticator for ApiKey authentication. Please refer to the readme for code examples.
* Add proper logging support for the RepetierConnection instead of using Console.WriteLine.

## Changed

* Reimplemented the event system to be async. It is still possible to register event handlers for the events of a
  repetier connection.
  Credit to the authors of [MQTTnet](https://github.com/dotnet/MQTTnet) for the inspiration on how to implement this.
* All Http requests are now async. This mainly is relevant for the methods which upload gcode files or upload and start
  gcode.
* Streamlined the RepetierConnectionBuilder. To establish a connection you can now supply a RestClient and
  WebsocketClient instance directly.
  This gives more flexibility to the end user and makes it easier to set up a correct RepetierConnection. Please refer
  to the readme for code examples.
* Commands are now called Requests
* Messages are now called Responses
* Updated System.Text.Json to 8.0.4

## Removed

* Various overloads and confusing methods for building a RepetierConnection instance when using the builder.

## Fixed

* Rework some methods to be async instead of sync.

# [0.1.9.1] - 2024-06-27

## Added

* Add support for .net8.0

## Changed

* Updated RestSharp to 111.3.0
* Updated System.Text.Json 8.0.3
* Updated Websocket.Client 5.1.2

## Removed

* Support for .net5.0 and .netcore3.1

# [0.1.9.0] - 2023-06-16

## Added

* Add types for new 1.4 events: `printerConditionChanged` and `globalErrorsChanged`
* Add event handler for event `printerConditionChanged`: `OnPrinterConditionChanged`

## Changes

* Fan speed is now supplied as int to be more consistent with other methods.

# [0.1.8.1] - 2023-06-15

## Changed

* `OnRepetierConnected` event now supplies the associated sessionId and is now fired later to ensure the sessionId is
  always set.

## Fixed

* Fix broken parameter keys in HTTP requests for `UploadAndStartPrint` and `UploadGCode`

# [0.1.8.0] - 2023-06-15

## Added

* Add option for `RepetierConnectionBuilder` to use TLS for both, `https` and `wss` when connecting to a Repetier Server

## Misc

* Using https://keepachangelog.com/en/1.0.0/ as changelog format from now on

# 0.1.7.2 - Bug fixes

## Fixes

* Fix type `StartJobCommand` not being public

# 0.1.7.1 - New versioning schema and breaking changes

## Breaking changes:

* Change autostart argument type for `UploadAndStartPrint` from bool to `StartBehavior` to represent the possibilities
  of the API correctly. See: https://prgdoc.repetier-server.com/v1/docs/index.html#/en/web-api/direct?id=upload
* Remove the default value for the group for both `UploadGCode` methods again. Sorry.

## Changes

* Change to more granular versioning schema.
* Made some `SendCommand(...)` overloads private/protected to reduce confusion on which method to use and added method
  documentation.
* Update System.Text.Json dependency from 6.0.2 to 6.0.7

## Fixes

* Fix small typos
* Fix missing/misleading method documentation

# 0.1.7

## Changes

* Internal changes as attempt to make websocket calls async

# 0.1.6 - QoL features and API changes

## Additions

* Add overloads for start print and upload print, which offers the possibility to provide the gcode as bytes instead of
  the whole file path
* Add ExtendPing command for big file uploads
* Add optional `autostart` url parameter for upload and start print; defaults to autostart = 1, which starts the print
  if print queue is empty

## Changes

* Remove CyclicCallHelper, Ping is now send on a regular basis after connect/reconnect when a message is received
* Add class holding session information to reduce the massive amount of properties in RepetierConnection
* Reduce Reconnection timeout
* Remove ByteArrayConverter as JsonSerializerOption

## Fixes

* Fix Ping not immediately send after connection is established or reconnection

# 0.1.5 - Add support for newer TFW

* Add build target for net5.0
* Add build target for net6.0

# 0.1.4 - Update dependencies

* Update RestSharp to 107.2.1
* Update System.Text.Json to 6.0.2

# 0.1.3 - Fix visibility

* Fix visibility of previous implemented methods

# 0.1.2 - Add support for raw commands

* Add methods to send raw commands (mainly for use with RepetierMqtt)

# 0.1.1 - Dependency version bump

* Update RestSharp to 106.15.0 due to potential vulnerability

# 0.1.0 - Initial release

* TBD


* Reprint/copy/store last prints.
* Keep alive test for TCP connections to detect offline printers.
    * Improved user management with new privileges.
* New server command: @sendToPrinter
    * Start/stop timelapse in webcam during print.
    * Added possibility to reorder the print queue.
* Option to disable print queues per printer.
    * Added possibility to upload multiple gcode and project files at once.

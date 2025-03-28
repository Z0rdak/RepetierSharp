# [0.3.0] - 2025-03-28

## Added

- Add Background Worker example to repo, as starting point and to test stuff.
- Responses can now be distinguished being a server or printer related response.
- Add separate event handler for server responses.
- Add separate event handler for printer responses, including the printer slug.
- Add separate event handler for server and printer events
- Add methods to schedule commands after initial repetier connection setup through builder (see example).
- Add separate response filter in builder (separates it from the command filter)
- Add custom logger for example

## Changed

- CommandManager now stores extended info about commands (server or printer command and printer slug)
- Change internal logging of commands, events, responses (debug, trace)

## Fixed

- Fix event filter not being applied correctly, causing events not being handled correctly
- Fix response filter not being applied correctly, causing response not being handled correctly

# [0.2.2] - 2025-03-27

## Added 

- Add command serializer to fix problems with serialization
- Add type mapping for new events
- Add event handling for jobsChanged, printQueueChanged and gcodeStorageChanged events. The corresponding event handlers are now invoked accordingly.

## Changed

- Reduce overly excessive error logging from exceptions during message handling

## Fixed

- Fix commands not being serialized properly
- Fix parsed json being discard to early causing errors
- Fix wrong type cast for state event serialization
- Fix wrong type cast for printerListChanged event serialization
- Fix wrong type cast for userCredentials event serialization

# [0.2.1] - 2025-03-26

## Fixed

- Fix logger factories not using the `using` statement, causing potential memory issues
- Fix JsonDocument during message handling not using the `using` statement, causing potential memory issues

# [0.2.0] - 2025-03-25

## Added

* Add proper logging support for the RepetierConnection instead of using Console.WriteLine. You can supply your own logger when using the RepetierConnectionBuilder or use a default console logger.
* Add a whole array of new events which are triggered for the repetier server events and other client related events.
* Add more dedicated events and event handlers for print job related events
* Add a filter for events and commands (and their responses) for the RepetierConnectionBuilder. This filters events so they don't get fired in the first place.
* Add separate .snupkg for debugging symbols

## Changed

* Reimplemented the event system to be async. It is still possible to register event handlers for the events of a
  repetier connection.
  Credit to the authors of [MQTTnet](https://github.com/dotnet/MQTTnet) for the inspiration on how to implement this.
* All Http requests are now async. This mainly is relevant for the methods which upload gcode files or upload and start
  gcode.
* ``ActivatePrinter`` and ``DeactivePrinter`` no longer select the active printer to send commands to. Instead, they work like the repetier server api describes them. Set the `SelectedPrinter` for this: `repetierCon.SelectedPrinter = "AwesomePrinterSlug"`. Alternatively you can select the printer when setting up the connection with the ``RepetierConnectionBuilder``.
* Streamlined the ``RepetierConnectionBuilder``. It's possible to supply a ``RestClient`` and/or 
  a ``WebsocketClient`` instance directly.
  This gives more flexibility to the end user and makes it easier to set up a correct ``RepetierConnection``. Please refer
  to the readme for code examples.
* Split commands into PrinterCommand and ServerCommand. PrinterCommands require a printer slug to address the proper printer.
* Extract HttpRequests (upload, getServerInfo, etc. ) into its own abstraction: `RemoteRepetierServer`
* Extract PrinterCommands (websocket commands) into its own abstraction: `RemoteRepetierPrinter`
* Reworked whole serialization and event and response handling.
* Updated System.Text.Json to 9.0.3
* Updated Microsoft.Extensions.* to 9.0.3
* Updated RestSharp to 112.1.0
* Rename the ``RepetierConnectionBuilder`` method ``WithCyclicCommand`` to ``ScheduleCommand``
* Rename ``GetRepetierServerInfoAsync`` to ``GetRepetierServerInfo``
* PingInterval is now called `KeepAlivePing`, is a TimeSpan and moved in ``RepetierSession``
* Move some properties from the `RepetierSession` to type ``RepetierAuthentication``. This includes only the info which needs to be supplied whenever a `loginRequired` event is fired. When supplied with the `RepetierConnectionBuilder` this is automatically used for authentication.
* Implement custom JsonSerializer for repetier server events to streamline to process of deserializing the events. It is possible to add custom event entries to a dictionary in the `RepetierEventConverter` alongside with a type which is used for deserialization.
* A **lot** of internal refactoring

## Removed

* Drop support for .net6.0
* SelectedPrinter property from RepetierConnection
* Various overloads and confusing methods for building a RepetierConnection instance when using the builder.
* Helper methods for the ``RepetierConnectionBuilder`` which could be used to schedule querying the printer state and printer list. That's now all covered by the `ScheduleCommand` method.

## Fixed

* Too many bugs to track

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
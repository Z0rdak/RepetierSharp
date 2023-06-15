# [0.1.8.1] - 2023-06-15
## Changed
* `OnRepetierConnected` event now supplies the associated sessionId and is now fired later to ensure the sessionId is always set.
## Fixed
* Fix broken paramter keys in http requests for `UploadAndStartPrint` and `UploadGCode`
# [0.1.8.0] - 2023-06-15
## Added
* Add option for `RepetierConnectionBuilder` to use TLS for both, `https` and `wss` when connecting to a Repetier Server # 0.1.7.2 - Bug fixes
## Misc
* Using https://keepachangelog.com/en/1.0.0/ as changelog format from now on
 # 0.1.7.2 - Bug fixes
## Fixes
  * Fix type `StartJobCommand` not being public
# 0.1.7.1 - New versioning schema and breaking changes
## Breaking changes: 
  * Change autostart argument type for `UploadAndStartPrint` from bool to `StartBehavior` to represent the possibilites of the API correctly. See: https://prgdoc.repetier-server.com/v1/docs/index.html#/en/web-api/direct?id=upload
  * Remove the default value for the group for both `UploadGCode` methods again. Sorry.
## Changes 
  * Change to more granular versioning schema.
  * Made some `SendCommand(...)` overloads private/protected to reduce confusion on which method to use and added method documentation.
  * Update System.Text.Json dependency from 6.0.2 to 6.0.7
## Fixes 
  * Fix small typos
  * Fix missing/misleading method documentation
# 0.1.7
## Changes
  * Internal changes as attempt to make websocket calls async
# 0.1.6 - QoL features and API changes
## Additions
  * Add overloads for start print and upload print, which offers the possibility to provide the gcode as bytes instead of the whole file path
  * Add ExtendPing command for big file uploads
  * Add optional `autostart` url parameter for upload and start print; defaults to autostart = 1, which starts the print if print queue is empty
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

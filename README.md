# RepetierSharp - A simple, event driven [Repetier Server](https://www.repetier-server.com/ "Repetier Server") client

![NuGet downloads](https://img.shields.io/nuget/dt/RepetierSharp)
![Nuget version](https://img.shields.io/nuget/v/RepetierSharp)
[![Donate](https://img.shields.io/badge/Donate-PayPal-green.svg)](https://www.paypal.com/donate/?hosted_button_id=9DEWUBKE4YT3S)

## Introduction

RepetierSharp is a simple, event-driven client which encapsulates the WebSocket and REST API of the Repetier Server
software to manage your printers.

### What is Repetier Server?

> "Repetier-Server is the professional all-in-one solution to control and manage your 3d printers and to get the most
> out of it."

https://www.repetier-server.com/

### Versioning

This library is up-to-date with RepetierServer version 1.4.x. The serialization for most commands and events should be
working with earlier versions, but there is the possibility of incompatibility when using RepetierSharp with earlier
versions due to undocumented changes in the Repetier Server API.

See the Changelog.md for a more detailed overview off added/changed features.

### Framework support

Currently, RepetierSharp supports .NET 6 and .NET 8.

## Documentation

The wiki is still under construction for the version 0.2.0. If you have any issues feel free to open an issue.

## Roadmap

The goal for this project is to add most (or even all) used WebSocket commands and events provided by the Repetier
Server API to enable a full programmatic control of the Repetier Server.

There are also some events and commands that are not documented in the API - with some time and effort, I will try to
reconstruct these and integrate them into RepetierSharp.

## Contribution

If you have any issues or found bugs feel free to open an issue.

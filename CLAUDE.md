# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

FalconBMS Arduino Connector is a C# Windows Forms application that bridges Falcon BMS flight simulator data to Arduino-compatible microcontrollers via serial communication. It reads the Falcon BMS shared memory interface using F4SharedMem library and transmits flight data (LightBits, BlinkBits, DED/PFL displays, instruments) to connected Arduino devices over COM ports.

## Build Commands

This is a .NET Framework 4.8 Windows Forms application using Visual Studio project format.

**Build the project:**
```bash
msbuild FalconBMSArduinoConnector.sln /p:Configuration=Debug
# or for release build:
msbuild FalconBMSArduinoConnector.sln /p:Configuration=Release
```

**Run the application:**
```bash
# Debug build output
./FalconBMSArduinoConnector/bin/Debug/FalconBMSArduinoConnector.exe

# Release build output
./FalconBMSArduinoConnector/bin/Release/FalconBMSArduinoConnector.exe
```

Alternatively, open `FalconBMSArduinoConnector.sln` in Visual Studio and use F5 to build and run.

## Architecture

### Core Components

**FalconConnector.cs** - Falcon BMS shared memory interface
- Uses F4SharedMem.Reader to access flight simulator data
- Provides helper methods to check individual LightBits/LightBits2/LightBits3 states
- Detects if Falcon BMS (or test tools like F4SharedMemoryRecorder) is running
- Returns current flight data including version, flying state, and full FlightData structure

**ArduinoConnector.cs** - Serial communication protocol handler
- Manages COM port connections with handshake protocol (0xA5/0x5A)
- Request/response protocol: Arduino sends command byte (0x01-0x41), PC responds with packet
- Packet format: `[0xAA][type][length][data...][checksum]`
- Runs background read thread to poll for Arduino requests
- Implements `NormalizeLine()` to convert DED/PFL text with inversion markers to custom font encoding
- Maps 65+ flight data fields to command codes (see README.md table)
- Handles disconnection/reconnection automatically

**Form1.cs** - Main UI using MetroFramework
- Multi-tab interface for managing multiple Arduino connections simultaneously
- Live display of all LightBits states via checkboxes
- DED line preview using custom Falcon DED font loaded from embedded resources
- Theme/style persistence using XML serialization to `%USERPROFILE%\Documents\My Games\FBAC\`
- System tray support for background operation
- 50ms timer polling Falcon status and updating UI

**DCSConnector.cs** - Placeholder TCP listener (port 31090) for future DCS World support (not currently used)

### Data Flow

1. `FalconConnector` continuously reads Falcon BMS shared memory via F4SharedMem library
2. `ArduinoConnector` instances (one per tab) listen for command bytes from Arduino devices
3. When Arduino sends command (e.g., 0x01 for LightBits), ArduinoConnector:
   - Fetches current FlightData
   - Converts requested field to byte array (BitConverter for primitives, NormalizeLine for text)
   - Sends response packet with checksum
4. UI timer (50ms) updates checkboxes and labels from latest FlightData

### Serial Protocol Details

The full spec (protocol v2) is in README.md. Command IDs live in the `FbacCommand` enum in ArduinoConnector.cs and must match `FBAC_Commands.h` in the Arduino library repo.

- Baud rate: 115200, 8N1, no flow control
- Handshake on connect: PC sends 0xA5 every 250 ms for up to 4 s until the Arduino answers 0x5A
- DTR enable option for Arduino Leonardo/ProMicro/Micro variants
- Command/response cycle: Arduino requests specific data by command code; the response type always equals the command (unknown commands get type 0x00)
- Checksum validation: `sum(type + length + all_data_bytes) & 0xFF`
- When BMS data is unavailable the PC still replies with a zero-filled payload of the correct length
- A bare 0x5A from a connected Arduino (older libraries) is answered with a bare 0xA5 so it can re-handshake
- v2 additions: 0x43 protocol version, 0x44 all lights + ECM in one packet

### Font Handling

Custom TrueType font `falconded.ttf` embedded as resource, loaded via `PrivateFontCollection` to render DED display accurately with inverted characters (lowercase = inverted uppercase in custom font).

### Settings Persistence

Two XML files in `%USERPROFILE%\Documents\My Games\FBAC\`:
- `user_settings.xml` - Theme (Light/Dark) and ColorStyle
- `arduino_tabs.xml` - List of Arduino tab configurations (port name, custom tab name)

## Dependencies

- **F4SharedMem.dll** - Falcon BMS shared memory interface (referenced from Downloads folder, not in repo)
- **MetroFramework** - Modern UI library (NuGet package)
- **Microsoft.VisualBasic** - Used for InputBox dialog

## Important Notes

- F4SharedMem.dll is resolved from `src\lib\F4SharedMem.dll` if present, otherwise the old Downloads path; override with `/p:F4SharedMemPath=...`
- The application expects Falcon BMS process or F4SharedMemoryRecorder.exe to be running
- Multiple Arduino connections are supported simultaneously via tab interface
- All flight data fields are documented in README.md table (cases 0x01-0x41)
- DED/PFL line normalization accounts for special characters (arrows, asterisks, underscores) and inversion

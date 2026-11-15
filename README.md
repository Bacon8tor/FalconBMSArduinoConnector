# FalconBMS Arduino Connector

**FalconBMS Arduino Connector** is a desktop application designed to enable **serial communication between Falcon BMS** (via its shared memory interface) and **any Arduino-compatible microcontroller**. This bridge makes it easy to build custom cockpit hardware that reflects real-time game state, including lights, display lines, and other interactive cockpit elements.

---

## ✨ Features

The application currently supports the following Falcon BMS data outputs:

- ✅ **All LightBits and BlinkBits**
  - Master Caution
  - Brow lights
  - Warning indicators
  - Auxiliary lights
  - And many more from the Falcon BMS shared memory
- ✅ **All DED Lines**
  - Full support for the 5-line DED (Data Entry Display) content
  - Compatible with external OLEDs or LCDs using Arduino
- ✅ **All PFL Lines**
  - Full support for the 5-line PFL content
  - Compatible with external OLEDs or LCDs using Arduino
- ✅ **Fuel Flow**
  - Full support for the Fuel Flow
- ✅ **Chaff / Flare Count**
  
---

## 🛠 How It Works

1. The application reads Falcon BMS shared memory using the [F4SharedMem](https://github.com/BMS-Development/F4SharedMem) interface.
2. It extracts relevant data such as LightBits, BlinkBits, and DED lines.
3. The data is then serialized and sent over a COM port to a connected Arduino.
4. The Arduino parses and uses this data to control physical outputs like LEDs, screens, etc.

---

## 📦 Arduino Library

An official Arduino library is available to simplify integration on the microcontroller side. It handles:

- Serial communication and parsing
- Bitwise checking of all LightBits and BlinkBits
- Reading and rendering DED lines to displays

➡️ **Library Repository**:  
[https://github.com/Bacon8tor/FalconBMSArduinoConnector-Arduino](https://github.com/Bacon8tor/FalconBMSArduinoConnector-Arduino)

**Platformio Blank Sketch**:
[FBAC PIO Example](https://github.com/Bacon8tor/FBAC_PIO_Example)
---

## 🚀 Getting Started

### Prerequisites

- Falcon BMS installed and configured
- Arduino (Uno, Mega, Leonardo, ESP32, etc.)
- Visual Studio (or your preferred IDE) for the desktop app
- Arduino IDE or PlatformIO for programming your Arduino
- Serial USB cable between your PC and Arduino

### Quick Steps

1. Clone this repository and open the solution in Visual Studio.
2. Compile and run the FalconBMSArduinoConnector app.
3. Connect your Arduino and upload the provided example using the Arduino library.
4. Match the COM port in the desktop app to your Arduino’s port.
5. Launch Falcon BMS and watch your hardware respond in real time!

---

## 📷 Screenshots

*Coming soon — show off your cockpit!*

---
## Serial Protocol (v2)

115200 baud by default, 8N1, no flow control. The Arduino asks and the PC answers.

* **Handshake:** the PC sends `0xA5` every 250 ms (for up to 4 s) until the Arduino replies `0x5A`. The retries cover boards that reset when the port opens (Uno/Nano).
* **Request:** the Arduino sends a single command byte (table below).
* **Response:** the PC sends `[0xAA][type][len][data...][checksum]`. `type` equals the command, multi-byte values are little-endian, and `checksum = (type + len + sum(data)) & 0xFF`.
* **Unknown command:** the PC replies with type `0x00`.
* **No BMS data:** the PC still replies, with the correct type and length and all bytes zero.
* **Version check:** after the handshake the library sends `0x43` once. An older PC app answers `0x00`, so the library falls back to protocol v1 and requests lights one word at a time.
* **Timeouts:** each request waits up to 250 ms (`bms.setRequestTimeout(ms)` to change). After 5 failed requests in a row, `isConnected()` turns false. The library then pings (`0x0F`) once a second to reconnect, and also answers a new handshake.
* Don't `Serial.print()` debug output on the port the PC app uses. The PC reads those bytes as commands.

| Cmd  | Description                       | Arduino field                    | Payload               |
| ---- | --------------------------------- | -------------------------------- | --------------------- |
| 0x01 | LightBits                         | `isMasterCaution()` …            | `uint32_t`            |
| 0x02 | LightBits2                        | `isHandOff()` …                  | `uint32_t`            |
| 0x03 | LightBits3                        | `isFlcsPmg()` …                  | `uint32_t`            |
| 0x04 | BlinkBits                         | `is…Blinking()`                  | `uint32_t`            |
| 0x05 | DED Lines (normalized)            | `dedLines[5]`                    | 5 × 24 chars          |
| 0x06 | Fuel Flow                         | `fuelFlow`                       | `float`               |
| 0x07 | Instrument Light Level            | `getInstrLightStatus()`          | `uint8_t`             |
| 0x08 | PFL Lines (normalized)            | `pflLines[5]`                    | 5 × 24 chars          |
| 0x09 | Chaff Count                       | `chaffCount`                     | `float`               |
| 0x0F | Ping                              | –                                | `0xAB`                |
| 0x10 | Flare Count                       | `flareCount`                     | `float`               |
| 0x11 | Flood Console Brightness          | `getFloodConsoleStatus()`        | `uint8_t` (0-6)       |
| 0x12 | RPM                               | `rpm`                            | `float`               |
| 0x13 | ECM Bits                          | `ecm[5]`, `getECMStatus(i)`      | 5 × `uint32_t`        |
| 0x14 | Oil Pressure 1                    | `oilPress`                       | `float`               |
| 0x15 | Oil Pressure 2                    | `oilPress2`                      | `float`               |
| 0x16 | Nozzle Position 1                 | `nozzlePos`                      | `float`               |
| 0x17 | Nozzle Position 2                 | `nozzlePos2`                     | `float`               |
| 0x18 | FTIT 1                            | `ftit`                           | `float`               |
| 0x19 | FTIT 2                            | `ftit2`                          | `float`               |
| 0x20 | Cabin Altitude                    | `cabinAlt`                       | `float`               |
| 0x21 | KIAS (Speed)                      | `kias`                           | `float`               |
| 0x22 | Internal Fuel                     | `internalFuel`                   | `float`               |
| 0x23 | External Fuel                     | `externalFuel`                   | `float`               |
| 0x24 | EPU Fuel                          | `epuFuel`                        | `float`               |
| 0x25 | Hydraulic Pressure A              | `hydPressA`                      | `float`               |
| 0x26 | Hydraulic Pressure B              | `hydPressB`                      | `float`               |
| 0x27 | CMDS Mode                         | `cmdsMode`                       | `int32_t`             |
| 0x28 | UHF Preset                        | `uhfPreset`                      | `int32_t`             |
| 0x29 | UHF Frequency                     | `uhfFreq`                        | `int32_t`             |
| 0x30 | Speed Brake                       | `speedBrake`                     | `float`               |
| 0x31 | IFF Mode 1 Digit 1                | `IFFMode1Digit1`                 | `uint8_t`             |
| 0x32 | IFF Mode 1 Digit 2                | `IFFMode1Digit2`                 | `uint8_t`             |
| 0x33 | IFF Mode 3A Digit 1               | `IFFMode3Digit1`                 | `uint8_t`             |
| 0x34 | IFF Mode 3A Digit 2               | `IFFMode3Digit2`                 | `uint8_t`             |
| 0x35 | Fuel FWD                          | `fwd`                            | `float`               |
| 0x36 | Fuel AFT                          | `aft`                            | `float`               |
| 0x37 | Fuel Total                        | `totalFuel`                      | `float`               |
| 0x38 | Desired Course                    | `desiredCourse`                  | `float`               |
| 0x39 | Course Deviation                  | `courseDeviation`                | `float`               |
| 0x40 | Distance To Beacon                | `distanceToBearing`              | `float`               |
| 0x41 | Bearing To Beacon                 | `bearingToBearing`               | `float`               |
| 0x42 | Sim Status (bit 0 = in 3D)        | `simStatus`, `isIn3D()`          | `uint32_t`            |
| 0x43 | Protocol Version *(v2)*           | `protocolVersion`                | `[version, flags]`    |
| 0x44 | All Lights *(v2)*                 | used by `checkAllLights()`       | 4 × `uint32_t` (LightBits, LightBits2, LightBits3, BlinkBits) + 5 × `uint32_t` ECM |

The command IDs are defined in `FBAC_Commands.h` (Arduino) and in the `FbacCommand` enum in `ArduinoConnector.cs` (PC). Keep both in sync.

## 📄 License

This project is open source and distributed under the **CC**.

---

## 📬 Contact

For support, ideas, or collaboration:

- GitHub Issues: [Submit here](https://github.com/Bacon8tor/FalconBMSArduinoConnector/issues)
- [!["Buy Me A Coffee"](https://www.buymeacoffee.com/assets/img/custom_images/orange_img.png)](https://buymeacoffee.com/bacon8tor)

---

**Fly safe and enjoy building your virtual cockpit!**

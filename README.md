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

## 📄 License

This project is open source and distributed under the **CC**.

---

## 📬 Contact

For support, ideas, or collaboration:

- GitHub Issues: [Submit here](https://github.com/Bacon8tor/FalconBMSArduinoConnector/issues)
- Email: [your-email@example.com]

---

**Fly safe and enjoy building your virtual cockpit!**

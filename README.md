```markdown
# FalconBMSArduinoReader

**FalconBMSArduinoReader** is a cross-platform library and Windows app combo that reads real-time data from Falcon BMS (via shared memory) and relays it to an Arduino or ESP32 over serial. Perfect for building custom cockpit panels, LED displays, and flight sim accessories.

---

## 🚀 Features

- Reads `lightBits`, `lightBits2`, `blinkBits`, and other shared-memory data from Falcon BMS (Block 52+)
- Sends compact binary packets to a microcontroller using a handshake-based protocol
- Supports multiple board types: ESP32, Arduino Uno/Nano/Mega, Leonardo, Teensy
- Includes a simple Arduino/PlatformIO library:
  - Call `update()` in `.loop()`
  - Query status using functions like `isMasterCaution()`, `isEngFire()`, etc.

---

## 📁 Project Structure

```

FalconBMSArduinoReader/
├── lib/
│   └── FalconBMSArduinoConnector/
│       ├── FalconBMSArduinoConnector.h
│       ├── FalconBMSArduinoConnector.cpp
│       └── LightBits.h
├── src/
│   └── main.cpp               ← Example usage for any board
├── WindowsApp/
│   └── (C# Visual Studio code)
├── platformio.ini
└── README.md

````

---

## 🧩 Arduino Library (PlatformIO-ready)

- Drop the `FalconBMSArduinoConnector` folder into your PlatformIO `lib/` directory
- In your sketch:

```cpp
#include <FalconBMSArduinoConnector.h>
FalconBMSArduinoConnector bms;
````

---

## 🔌 Usage Example (`main.cpp`)

```cpp
#include <FalconBMSArduinoConnector.h>

FalconBMSArduinoConnector bms;

#if defined(ESP32)
  const int ledPin = 2;
#else
  const int ledPin = LED_BUILTIN;
#endif

void setup() {
  pinMode(ledPin, OUTPUT);
  Serial.begin(115200);
  bms.begin(Serial);
}

void loop() {
  bms.update();

  digitalWrite(ledPin, bms.isConnected() && bms.isMasterCaution() ? HIGH : LOW); //Simply checks whether MasterCaution is ON or OFF then sets LED High or Low
  delay(50);
}
```

---

## 🖥 Windows C# App

* Reads from Falcon BMS shared memory (`F4SharedMem.dll`)
* Implements handshake-based serial send (`WAIT for READY → send packet`)
* Automatically reconnects if Falcon BMS closes
* Customize mappings from `lightBits` and `blinkBits` to servos, LEDs, and more

---

## ⚙ PlatformIO Configuration

Example `platformio.ini`:

```ini
[env:esp32dev]
platform = espressif32
board = esp32dev
framework = arduino
monitor_speed = 115200

[env:uno]
platform = atmelavr
board = uno
framework = arduino
monitor_speed = 115200

[env:nanoatmega328]
platform = atmelavr
board = nanoatmega328
framework = arduino
monitor_speed = 115200


[env:megaatmega2560]
platform = atmelavr
board = megaatmega2560
framework = arduino
monitor_speed = 115200

[env:nodemcuv2]
platform = espressif8266
board = nodemcuv2
framework = arduino
monitor_speed = 115200


```

---

## 📌 Acknowledgments

* Checkout Other Projects like this  [HummerX’s BMSAIT](https://github.com/HummerX/BMSAIT)  [F4toSerial](https://f4toserial.com/)
* This project would not have been possible without [lightningtools](https://github.com/lightningviper/lightningstools). 

---


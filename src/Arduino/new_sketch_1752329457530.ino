#include <Wire.h>
#include <LiquidCrystal_I2C.h>
#include "LightBits.h"

LiquidCrystal_I2C lcd(0x3F, 16, 2);

uint8_t buffer[10];
uint8_t idx = 0;
bool isReading = false;
unsigned long lastSerialActivity = 0;
const unsigned long timeoutMs = 5000;
bool isConnected = false;

uint32_t lightBits = 0;
uint32_t lightBits2 = 0;

void setup() {
  Serial.begin(115200);
  while (!Serial);

  lcd.init();
  lcd.backlight();
  lcd.clear();
  lcd.setCursor(0, 0);
  lcd.print("WAITING...");

  Serial.println("READY");
}

void loop() {
  while (Serial.available()) {
    uint8_t b = Serial.read();

    if (!isReading) {
      if (b == 0xAA) {
        isReading = true;
        idx = 0;
      }
      continue;
    }

    buffer[idx++] = b;

    if (idx >= 2) {
      uint8_t expectedLen = buffer[1]; // buffer[1] = payload length
      if (idx == 2 + expectedLen + 1) { // type + len + data + checksum
        // Packet complete
        uint8_t type = buffer[0];
        uint8_t len = buffer[1];
        uint8_t* data = &buffer[2];
        uint8_t checksum = buffer[2 + len];

        // Verify checksum
        uint8_t sum = type + len;
        for (int i = 0; i < len; i++) sum += data[i];

        if (checksum == (sum & 0xFF)) {
          handlePacket(type, data, len);
        }

        isReading = false;
      }
    }
  }

  // Handle timeout/disconnection
  if (isConnected && (millis() - lastSerialActivity > timeoutMs)) {
    isConnected = false;
    lcd.clear();
    lcd.setCursor(0, 0);
    lcd.print("WAITING...");
  }
}

void handlePacket(uint8_t type, uint8_t* data, uint8_t len) {
  lastSerialActivity = millis();

  if (!isConnected) {
    isConnected = true;
    lcd.clear();
    lcd.setCursor(0, 0);
    lcd.print("Connected!");
    delay(1000);
  }

  if (type == 0x01 && len == 4) {
    
    memcpy(&lightBits, data, 4);
    lcd.clear();
lcd.setCursor(0, 0);
lcd.print("TF:");
lcd.print((lightBits & TF) ? "Y" : "N");

lcd.setCursor(5, 0);
lcd.print("ENG:");
lcd.print((lightBits & ENG_FIRE) ? "Y" : "N");

lcd.setCursor(11, 0);
lcd.print("HYD:");
lcd.print((lightBits & HYD) ? "Y" : "N");

lcd.setCursor(0, 1);
lcd.print("MC:");
lcd.print((lightBits & MasterCaution) ? "Y" : "N");


    
  } else if (type == 0x02 && len == 4) {
    memcpy(&lightBits2, data, 4);
    lcd.clear();
    lcd.setCursor(0, 0);
    lcd.print("SeatArmed:");
    lcd.setCursor(0, 1);
    lcd.print((lightBits2 & (1UL << 25)) ? "YES" : "NO");
  } else {
    lcd.clear();
    lcd.setCursor(0, 0);
    lcd.print("Invalid pkt");
  }
}


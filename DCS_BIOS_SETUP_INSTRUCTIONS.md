# DCS-BIOS Integration Setup Instructions

This guide explains how to set up DCS-BIOS to work with your FalconBMS Arduino Connector for enhanced DCS World integration.

## What is DCS-BIOS?

DCS-BIOS is a community-developed standard for DCS World that provides:
- **Standardized data export** for all aircraft systems
- **Precise control inputs** for cockpit controls
- **Real-time data streaming** via UDP multicast
- **Aircraft-specific data mapping** for over 40 aircraft modules
- **Much more detailed data** than basic export scripts

## Advantages Over Simple DCS Export

| Feature | Simple DCS Export | DCS-BIOS |
|---------|------------------|----------|
| Data Detail | Basic flight parameters | Full cockpit state |
| Aircraft Support | Generic data only | Aircraft-specific systems |
| Data Accuracy | Approximate values | Precise, validated data |
| Control Input | Not supported | Full cockpit control |
| Maintenance | Manual updates needed | Community maintained |
| Protocol | Custom TCP | Standard UDP multicast |

## Prerequisites

- DCS World (any version)
- FalconBMS Arduino Connector application
- Administrator privileges for DCS folder access

## Installation Steps

### 1. Download DCS-BIOS

1. Visit: https://github.com/DCS-Skunkworks/dcs-bios
2. Download the latest release (not the source code)
3. Extract to a temporary folder

### 2. Install DCS-BIOS

1. **Locate your DCS Scripts folder:**
   - Stable: `C:\Users\[YourUsername]\Saved Games\DCS\Scripts\`
   - Open Beta: `C:\Users\[YourUsername]\Saved Games\DCS.openbeta\Scripts\`

2. **Backup existing Export.lua (if present):**
   ```
   Rename "Export.lua" to "Export.lua.backup"
   ```

3. **Copy DCS-BIOS files:**
   ```
   Copy the entire "DCS-BIOS" folder to your Scripts directory
   Copy "Export.lua" to your Scripts directory
   ```

4. **Verify installation:**
   Your Scripts folder should contain:
   ```
   Scripts/
   ├── DCS-BIOS/          (folder with all DCS-BIOS files)
   ├── Export.lua         (DCS-BIOS export script)
   └── Export.lua.backup  (your old export script, if any)
   ```

### 3. Configure DCS-BIOS

1. **Edit Export.lua** (only if needed for custom settings):
   ```lua
   -- Default settings work for most users
   -- UDP Export: 239.255.50.10:5010 (multicast)
   -- UDP Import: 127.0.0.1:7778 (commands)
   ```

2. **Optional: Configure specific aircraft** (advanced users):
   - Check `DCS-BIOS/docs/` for aircraft-specific documentation
   - Aircraft modules are loaded automatically when entering cockpit

## Usage Instructions

### 1. Start the Arduino Connector

1. Launch `FalconBMSArduinoConnector.exe`
2. The DCS-BIOS listener starts automatically
3. Status shows "DCS-BIOS Not Connected" initially
4. Connect your Arduino devices as usual

### 2. Start DCS World

1. Launch DCS World
2. Load any mission with a supported aircraft
3. Enter the cockpit (3D cockpit view)
4. DCS-BIOS will begin streaming data automatically

### 3. Verify Connection

**In Arduino Connector:**
- Status changes to "DCS World (BIOS)"
- Connection status: "DCS-BIOS Connected & Synchronized"
- DED display shows "DCS-BIOS CONNECTED"
- Flight data updates in real-time

**In DCS World:**
- No visual indicators needed
- Data streams automatically when in cockpit

## Supported Aircraft Modules

### Fully Supported (Complete cockpit integration):
- A-10C/A-10C II
- F/A-18C Hornet
- F-16C Viper
- AV-8B Harrier
- F-14 Tomcat
- AH-64D Apache
- And many more...

### Basic Support (Flight data only):
- All other aircraft modules receive basic flight parameters
- Altitude, speed, heading, fuel, gear status

### Checking Aircraft Support:
1. Check: `DCS-BIOS/doc/aircrafts/` folder
2. Each supported aircraft has its own documentation file
3. Visit: https://dcs-bios.readthedocs.io/ for online documentation

## Data Available to Arduino

### Basic Flight Data (All Aircraft):
- **Altitude** - Precise altitude above sea level
- **Airspeed** - Indicated airspeed
- **Heading** - Magnetic heading
- **Fuel Quantity** - Internal fuel remaining
- **Landing Gear** - Gear position status
- **Master Caution/Warning** - General warning status

### Advanced Data (Supported Aircraft):
- **All cockpit lights** - Warning, caution, indicator lights
- **Engine parameters** - RPM, temperature, pressure
- **Navigation data** - Course, bearing, distance
- **Radio frequencies** - COM/NAV radio settings
- **Weapon systems** - Selected weapons, countermeasures
- **Flight controls** - Flap position, trim settings
- **And much more...**

## Arduino Command Mapping

Your existing Arduino library works without changes. DCS-BIOS data is automatically mapped to the same commands:

| Arduino Command | DCS-BIOS Data | Description |
|----------------|---------------|-------------|
| 0x01 | LightBits | Warning/caution lights |
| 0x05 | DED Display | Data entry display info |
| 0x21 | Airspeed | Indicated airspeed |
| 0x22 | Fuel | Internal fuel quantity |
| 0x38 | Heading | Magnetic heading |
| And more... | | All existing commands supported |

## Advanced Configuration

### Custom Aircraft Address Mapping

For aircraft-specific data, you can configure custom address mappings:

```csharp
// Example: Configure F/A-18C specific addresses
var addresses = new Dictionary<string, ushort>
{
    ["master_caution"] = 0x1012,  // F/A-18C master caution light
    ["gear_down"] = 0x1014,       // F/A-18C gear position
    ["altitude"] = 0x108C,        // F/A-18C altitude
    // Add more aircraft-specific addresses
};

dcsBios.ConfigureAircraftAddresses(addresses);
```

### Finding Aircraft Addresses

1. **Check DCS-BIOS documentation:**
   - `DCS-BIOS/doc/aircrafts/[aircraft_name].lua`
   - Online docs: https://dcs-bios.readthedocs.io/

2. **Use DCS-BIOS Control Reference:**
   - Start DCS-BIOS Hub
   - Browse aircraft controls and their addresses

3. **Monitor data stream:**
   - Use DCS-BIOS data viewer tools
   - Watch for changing addresses during flight

## Troubleshooting

### Connection Issues

**Problem**: "DCS-BIOS Not Connected"
**Solutions**:
1. Ensure DCS-BIOS is properly installed in Scripts folder
2. Check Windows Firewall settings for multicast traffic
3. Verify DCS is running and you're in cockpit view
4. Restart both DCS and Arduino Connector

**Problem**: "Connected but not synchronized"
**Solutions**:
1. Enter aircraft cockpit (3D view required)
2. Wait 10-15 seconds for initial data sync
3. Switch to a different aircraft and back
4. Check DCS-BIOS console for errors

### Data Issues

**Problem**: No data updates from DCS
**Solutions**:
1. Ensure you're in cockpit view (not external view)
2. Try a different aircraft module
3. Check if aircraft is DCS-BIOS compatible
4. Verify multicast traffic isn't blocked

**Problem**: Partial data only
**Solutions**:
1. This is normal for unsupported aircraft (basic data only)
2. Check aircraft compatibility list
3. Update to latest DCS-BIOS version

### Performance Issues

**Problem**: DCS frame rate drops
**Solutions**:
1. DCS-BIOS impact is minimal (<1% CPU)
2. Close unnecessary Arduino Connector windows
3. Reduce export frequency in DCS-BIOS settings
4. Check for other export scripts conflicts

## Multiple Export Scripts

If you need other export scripts alongside DCS-BIOS:

1. **Create combined Export.lua:**
```lua
-- Load DCS-BIOS
dofile(lfs.writedir()..[[Scripts\DCS-BIOS\BIOS.lua]])

-- Load other scripts
dofile(lfs.writedir()..[[Scripts\YourOtherScript.lua]])
```

2. **Keep scripts separate:**
   - DCS-BIOS handles its own export automatically
   - Other scripts should not conflict with UDP multicast

## Support and Updates

### Getting Help:
1. **DCS-BIOS Community**: https://github.com/DCS-Skunkworks/dcs-bios/issues
2. **Documentation**: https://dcs-bios.readthedocs.io/
3. **Discord**: DCS-BIOS community channels

### Staying Updated:
1. Check GitHub releases regularly
2. Update when new aircraft modules are added
3. Follow community announcements for compatibility changes

### Configuration Backup:
Always backup your Scripts folder before major DCS updates:
```
Copy entire Scripts folder to safe location
DCS updates may overwrite Export.lua
Restore from backup after DCS updates
```

## Comparison: DCS-BIOS vs Simple Export

**Choose DCS-BIOS if you want:**
- ✅ Maximum detail and accuracy
- ✅ Aircraft-specific systems data
- ✅ Community support and updates
- ✅ Standard protocol compatibility
- ✅ Future-proof solution

**Choose Simple Export if you want:**
- ✅ Minimal setup complexity
- ✅ Basic flight data only
- ✅ Custom data format control
- ✅ Lightweight solution

**Recommendation**: Use DCS-BIOS for the best experience. It provides much more detailed and accurate data with minimal additional complexity.
# DCS World Integration Setup Instructions

This guide explains how to set up DCS World to send flight data to your FalconBMS Arduino Connector.

## Prerequisites

- DCS World installed
- FalconBMS Arduino Connector application
- LuaSocket installed with DCS World (usually included)

## Installation Steps

### 1. Locate DCS Scripts Folder

Find your DCS Scripts folder:
- **Default location**: `C:\Users\[YourUsername]\Saved Games\DCS\Scripts\`
- **Alternative**: `C:\Users\[YourUsername]\Saved Games\DCS.openbeta\Scripts\` (for open beta)

### 2. Backup Existing Export Script (If Present)

If you already have an `Export.lua` file:
```
1. Navigate to the Scripts folder
2. Rename "Export.lua" to "Export.lua.backup"
```

### 3. Install the FBAC Export Script

```
1. Copy "DCS_Export_FBAC.lua" to your DCS Scripts folder
2. Rename it to "Export.lua"
```

### 4. Verify Installation

Your Scripts folder should now contain:
- `Export.lua` (the FBAC export script)
- `Export.lua.backup` (if you had an existing script)

## Usage Instructions

### 1. Start the Arduino Connector

1. Launch `FalconBMSArduinoConnector.exe`
2. The DCS listener will start automatically on port 31090
3. Connect your Arduino devices as usual

### 2. Start DCS World

1. Launch DCS World
2. Load any mission or training scenario
3. Enter the cockpit of any aircraft

### 3. Verify Connection

**In DCS World:**
- Look for chat messages showing connection status:
  - `"FBAC: Connected to Arduino Connector"` (success)
  - `"FBAC: Failed to connect"` (connection failed)

**In Arduino Connector:**
- Status should show "DCS World" and "DCS Connected"
- DED display should show "DCS WORLD CONNECTED"
- Flight data should update in real-time

## Supported Data

The script exports the following data to your Arduino:
- **Altitude** - Current altitude above sea level
- **Speed** - Indicated airspeed
- **Heading** - Aircraft heading (0-360 degrees)
- **Fuel Level** - Internal fuel quantity
- **Landing Gear** - Gear up/down status
- **Master Caution** - Warning light status
- **Aircraft Type** - Current aircraft name

## Troubleshooting

### Connection Issues

**Problem**: "FBAC: Failed to connect" message in DCS
**Solutions**:
1. Ensure Arduino Connector is running before starting DCS
2. Check Windows Firewall isn't blocking the connection
3. Verify port 31090 isn't used by another application

**Problem**: No data showing in Arduino Connector
**Solutions**:
1. Restart both DCS and Arduino Connector
2. Try a different aircraft/mission
3. Check the Export.lua file is in the correct Scripts folder

### Performance Issues

**Problem**: DCS stuttering or low FPS
**Solutions**:
1. The script exports at 10Hz (every 100ms) by default
2. To reduce frequency, edit line: `local export_interval = 0.2` (for 5Hz)
3. Only significant data changes trigger exports

### Multiple Export Scripts

If you need to use multiple export scripts:

1. Rename `Export.lua` to `Export_FBAC.lua`
2. Create a new `Export.lua` with this content:
```lua
-- Master Export.lua - loads multiple export scripts
local fbac = loadfile("Scripts/Export_FBAC.lua")
local other = loadfile("Scripts/Export_Other.lua")

function LuaExportStart()
    if fbac then fbac().LuaExportStart() end
    if other then other().LuaExportStart() end
end

function LuaExportActivityNextEvent(t)
    local tNext = math.huge
    if fbac then
        local tNextFBAC = fbac().LuaExportActivityNextEvent(t)
        tNext = math.min(tNext, tNextFBAC)
    end
    if other then
        local tNextOther = other().LuaExportActivityNextEvent(t)
        tNext = math.min(tNext, tNextOther)
    end
    return tNext
end

function LuaExportStop()
    if fbac then fbac().LuaExportStop() end
    if other then other().LuaExportStop() end
end
```

## Data Format

The script sends data in key=value format:
```
altitude=5000
speed=250.5
heading=090
fuel=0.75
master_caution=false
gear_down=true
aircraft_type=F/A-18C
```

## Supported Aircraft

The script works with all DCS aircraft, but some features may vary:
- **Basic data** (altitude, speed, heading): All aircraft
- **Fuel level**: Most aircraft
- **Landing gear**: All aircraft with retractable gear
- **Master caution**: May require aircraft-specific modifications

## Advanced Configuration

### Modify Export Frequency
Edit line in `Export.lua`:
```lua
local export_interval = 0.1  -- 10Hz (default)
local export_interval = 0.05 -- 20Hz (higher frequency)
local export_interval = 0.2  -- 5Hz (lower frequency)
```

### Change Connection Settings
Edit lines in `Export.lua`:
```lua
local FBAC_HOST = "127.0.0.1"  -- Change IP if Arduino Connector is on different PC
local FBAC_PORT = 31090        -- Change port if needed
```

### Add Custom Data
The script can be extended to export additional aircraft-specific data by modifying the `GetCurrentData()` function.

## Support

For issues or questions:
1. Check DCS logs for error messages
2. Verify Arduino Connector shows "Listening for DCS" status
3. Test with a simple training mission first
4. Ensure both applications are running as the same user (admin/non-admin)
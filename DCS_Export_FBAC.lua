-- DCS Export Script for FalconBMS Arduino Connector
-- Place this file in: DCS World\Scripts\Export.lua (rename existing Export.lua to Export.lua.bak first)
-- This script exports basic flight data to the FalconBMS Arduino Connector via TCP

-- Network configuration
local FBAC_HOST = "127.0.0.1"
local FBAC_PORT = 31090

-- Global variables
local fbac_socket = nil
local fbac_connected = false
local last_export_time = 0
local export_interval = 0.1  -- Export every 100ms (10 Hz)

-- Previous values to detect changes
local prev_data = {}

-- Initialize network connection
function LuaExportStart()
    -- Load socket library
    package.path = package.path .. ";.\\LuaSocket\\?.lua"
    package.cpath = package.cpath .. ";.\\LuaSocket\\?.dll"

    local socket = require("socket")

    -- Create TCP socket
    fbac_socket = socket.tcp()
    if fbac_socket then
        fbac_socket:settimeout(0) -- Non-blocking
        local result, err = fbac_socket:connect(FBAC_HOST, FBAC_PORT)
        if result then
            fbac_connected = true
            LoSetCommand(371, "FBAC: Connected to Arduino Connector")
        else
            LoSetCommand(371, "FBAC: Failed to connect - " .. (err or "unknown error"))
        end
    else
        LoSetCommand(371, "FBAC: Failed to create socket")
    end
end

-- Clean up on exit
function LuaExportStop()
    if fbac_socket then
        fbac_socket:close()
        fbac_socket = nil
        fbac_connected = false
    end
end

-- Main export function called every frame
function LuaExportActivityNextEvent(t)
    local time_to_next_export = export_interval

    if fbac_connected and (t - last_export_time >= export_interval) then
        -- Get current flight data
        local current_data = GetCurrentData()

        -- Check if data has changed significantly or if it's first export
        if ShouldExportData(current_data) then
            ExportDataToFBAC(current_data)
            prev_data = current_data
            last_export_time = t
        end
    end

    return time_to_next_export
end

-- Get current aircraft data
function GetCurrentData()
    local self_data = LoGetSelfData()
    local pilot_name = LoGetPilotName()
    local model_time = LoGetModelTime()
    local indicated_airspeed = LoGetIndicatedAirSpeed()
    local true_airspeed = LoGetTrueAirSpeed()
    local altitude = LoGetAltitudeAboveSeaLevel()
    local altitude_agl = LoGetAltitudeAboveGroundLevel()

    -- Default values
    local data = {
        altitude = altitude or 0,
        speed = indicated_airspeed or 0,
        heading = 0,
        fuel = 0,
        master_caution = false,
        gear_down = false,
        aircraft_type = "Unknown"
    }

    if self_data then
        -- Position and heading
        data.heading = math.deg(self_data.Heading or 0)
        if data.heading < 0 then data.heading = data.heading + 360 end

        -- Aircraft type
        data.aircraft_type = self_data.Name or "Unknown"

        -- Fuel (estimate from engine data if available)
        local engine_info = LoGetEngineInfo()
        if engine_info and engine_info.fuel_internal then
            data.fuel = engine_info.fuel_internal
        end

        -- Landing gear status
        local mech_info = LoGetMechInfo()
        if mech_info and mech_info.gear then
            data.gear_down = (mech_info.gear.value > 0.5)
        end

        -- Master caution/warning lights
        local cockpit_info = LoGetCockpitDisplayData()
        if cockpit_info then
            -- This varies by aircraft - you may need to adjust based on specific aircraft
            data.master_caution = false -- Default, would need specific aircraft data
        end
    end

    return data
end

-- Check if data should be exported (significant change or first time)
function ShouldExportData(current_data)
    if not prev_data.altitude then
        return true  -- First export
    end

    -- Check for significant changes
    local alt_change = math.abs(current_data.altitude - (prev_data.altitude or 0))
    local speed_change = math.abs(current_data.speed - (prev_data.speed or 0))
    local heading_change = math.abs(current_data.heading - (prev_data.heading or 0))

    -- Export if changes are significant
    return alt_change > 10 or speed_change > 5 or heading_change > 2 or
           current_data.gear_down ~= prev_data.gear_down or
           current_data.master_caution ~= prev_data.master_caution
end

-- Export data to FBAC
function ExportDataToFBAC(data)
    if not fbac_connected or not fbac_socket then
        return
    end

    -- Format data as key=value pairs (compatible with FBAC DCS connector)
    local export_string = string.format(
        "altitude=%.0f\nspeed=%.1f\nheading=%.0f\nfuel=%.2f\nmaster_caution=%s\ngear_down=%s\naircraft_type=%s\n",
        data.altitude,
        data.speed,
        data.heading,
        data.fuel,
        tostring(data.master_caution),
        tostring(data.gear_down),
        data.aircraft_type
    )

    -- Send data
    local result, err = fbac_socket:send(export_string)
    if not result then
        if err == "closed" then
            fbac_connected = false
            LoSetCommand(371, "FBAC: Connection closed")
        else
            -- Try to reconnect
            AttemptReconnect()
        end
    end
end

-- Attempt to reconnect to FBAC
function AttemptReconnect()
    if fbac_socket then
        fbac_socket:close()
    end

    local socket = require("socket")
    fbac_socket = socket.tcp()
    if fbac_socket then
        fbac_socket:settimeout(0)
        local result, err = fbac_socket:connect(FBAC_HOST, FBAC_PORT)
        if result then
            fbac_connected = true
            LoSetCommand(371, "FBAC: Reconnected to Arduino Connector")
        else
            fbac_connected = false
            fbac_socket = nil
        end
    end
end

-- Handle aircraft events
function LuaExportBeforeNextFrame()
    -- Called before each frame - can be used for additional processing
end

function LuaExportAfterNextFrame()
    -- Called after each frame
end

-- Handle multiplayer events
function LuaExportPlayerChangeSlot()
    -- Reset previous data when changing aircraft
    prev_data = {}
end
# RastreadorDeProcesos - Process Tracker

## Overview

RastreadorDeProcesos is a Windows Forms-based process monitoring application that tracks the runtime duration of selected executable programs [1](#0-0) . The application operates as a background service with system tray integration, continuously monitoring target processes and logging their activity patterns [2](#0-1) .

## Features

- **Real-time Process Monitoring**: Tracks when selected processes start and stop with 1-second precision [3](#0-2) 
- **System Tray Integration**: Runs in background with minimal UI footprint [4](#0-3) 
- **Automatic Startup**: Registers itself to start with Windows [5](#0-4) 
- **Session Logging**: Maintains detailed logs of process sessions with start/stop times and duration [6](#0-5) 
- **Administrator Privileges**: Automatically elevates to admin rights when needed [7](#0-6) 

## Technical Specifications

### Runtime Environment
- **Framework**: .NET Framework 4.7.2 [8](#0-7) 
- **Application Type**: Windows Forms executable (WinExe) [9](#0-8) 
- **Version**: 1.0.0.0 [10](#0-9) 

### Architecture
- **Main Class**: `Form1` - Primary UI container and application controller [11](#0-10) 
- **Monitoring Engine**: Timer-based polling system checking process state every second [12](#0-11) 
- **Configuration Storage**: Text-based config file and Windows Registry integration [13](#0-12) 

## Installation & Deployment

### Prerequisites
- Windows operating system
- .NET Framework 4.7.2 or higher
- Administrator privileges (application will request elevation automatically)

### Deployment Options

#### ClickOnce Deployment
The application is configured for ClickOnce deployment with certificate-based signing [14](#0-13) :
- **Publish URL**: Configurable desktop publishing [15](#0-14) 
- **Code Signing**: Uses certificate thumbprint for manifest signing [16](#0-15) 

#### Manual Installation
1. Build the application in Release mode
2. Deploy the executable to target machine
3. Run the application (it will request administrator privileges)
4. The application automatically registers for Windows startup

## Configuration

### File-based Configuration
- **config.txt**: Stores the path of the selected executable to monitor [17](#0-16) 
- **log.txt**: Contains session history with timestamps and durations [18](#0-17) 

### Registry Configuration
The application registers itself in the Windows startup registry: [19](#0-18) 
```
HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\Run
```

## Usage

### Initial Setup
1. Launch the application (administrator elevation will be requested)
2. Click "Selecciona Aplicacion" to choose an executable to monitor [20](#0-19) 
3. The application minimizes to system tray and begins monitoring

### Monitoring Operations
- **Process Detection**: Uses `Process.GetProcessesByName()` to detect target process [21](#0-20) 
- **Session Tracking**: Records start time when process begins and calculates duration when it ends [22](#0-21) 
- **Real-time Updates**: UI updates every second showing current time and elapsed runtime [23](#0-22) 

### System Tray Interaction
- **Double-click**: Restore application window [24](#0-23) 
- **Right-click menu**: Access "Abrir" (Open) and "Salir" (Exit) options [25](#0-24) 

## Logging Format

Process sessions are logged in the following format:
```
Proceso: {processName} | Inicio: {HH:mm} | Fin: {HH:mm} | Duración: {HH:mm}
``` [26](#0-25) 

## Security Considerations

- **Administrator Privileges**: Required for registry access and comprehensive process monitoring [27](#0-26) 
- **Privilege Elevation**: Automatic restart with elevated permissions if not running as administrator [28](#0-27) 
- **Code Signing**: Application manifests are digitally signed for security [29](#0-28) 

## Error Handling

The application implements comprehensive error handling across all major operations:
- Configuration file operations with user notification [30](#0-29) 
- Process monitoring with exception catching [31](#0-30) 
- Registry access with error messaging [32](#0-31) 

## Notes

The application was developed by noe in commit [0b13527f](https://github.com/thepuag/RastreadorDeProcesos/commit/0b13527f) on March 5, 2025. The codebase is entirely in Spanish with UI elements and comments in Spanish, but the core functionality is language-agnostic. The application requires Windows-specific APIs for process monitoring and registry access, making it Windows-only compatible.

Wiki pages you might want to explore:
- [Overview (thepuag/RastreadorDeProcesos)](/wiki/thepuag/RastreadorDeProcesos#1)
- [Application Architecture (thepuag/RastreadorDeProcesos)](/wiki/thepuag/RastreadorDeProcesos#2)

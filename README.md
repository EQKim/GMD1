# GMD1
GMD1 - VIA Unity WebGL Project

## Overview
This is a Unity project configured for WebGL builds. The project includes basic setup and a simple controller script to demonstrate WebGL functionality.

## Project Structure
- **Assets/Scenes/** - Contains the main scene (MainScene.unity)
- **Assets/Scripts/** - Contains C# scripts including WebGLController
- **ProjectSettings/** - Unity project configuration files
- **Packages/** - Unity package dependencies

## Unity Version
- Unity 2021.3.0f1 or compatible version

## WebGL Build Settings
This project is configured with:
- WebGL as a supported build target
- IL2CPP scripting backend for WebGL
- Optimized settings for web deployment
- Memory size: 16MB
- Exception support enabled
- Data caching enabled

## Getting Started

### Prerequisites
- Unity 2021.3 or later with WebGL Build Support module installed

### Opening the Project
1. Clone this repository
2. Open Unity Hub
3. Click "Open" and select the project folder
4. Wait for Unity to import all assets

### Building for WebGL
1. Open Unity Editor
2. Go to File > Build Settings
3. Select "WebGL" platform
4. Click "Switch Platform" if not already selected
5. Click "Build" or "Build And Run"
6. Choose a folder for the build output

### Running the Project
- **In Unity Editor**: Open Assets/Scenes/MainScene.unity and click Play
- **WebGL Build**: Host the build folder on a web server and access via browser

## Features
- Basic WebGL configuration
- Simple controller script demonstrating Unity/WebGL integration
- Platform detection and logging
- Keyboard input handling (Space key example)

## Development
The WebGLController script demonstrates:
- Platform-specific compilation directives
- Unity logging for debugging
- Basic input handling compatible with WebGL

## License
All rights reserved.

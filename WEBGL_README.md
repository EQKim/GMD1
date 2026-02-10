# GMD1 - Unity WebGL Project

This is a Unity project configured for WebGL builds.

## Project Information

- **Project Name**: GMD1-VIA
- **Unity Version**: 2022.3.10f1
- **Target Platform**: WebGL
- **Scripting Backend**: IL2CPP (WebGL)

## Project Structure

```
GMD1/
├── Assets/
│   ├── Scenes/
│   │   └── MainScene.unity      # Main game scene
│   └── Scripts/
│       └── WebGLDemo.cs         # WebGL demonstration script
├── ProjectSettings/             # Unity project settings
├── Packages/                    # Package dependencies
└── README.md
```

## Building for WebGL

### Prerequisites
- Unity 2022.3.10f1 or compatible version
- WebGL Build Support module installed

### Build Steps

1. **Open the Project**
   - Open Unity Hub
   - Add this project folder
   - Open with Unity 2022.3.10f1

2. **Configure Build Settings**
   - Go to `File > Build Settings`
   - Select `WebGL` platform
   - Click `Switch Platform` if not already selected
   - Add `Assets/Scenes/MainScene.unity` to Scenes In Build

3. **Player Settings** (already configured)
   - Company Name: GMD1
   - Product Name: GMD1-VIA
   - WebGL Memory Size: 256 MB
   - Compression Format: Gzip
   - Exception Support: Explicitly Thrown Exceptions Only

4. **Build the Project**
   - Click `Build` or `Build And Run`
   - Choose an output folder (e.g., `Build/WebGL`)
   - Wait for the build to complete

5. **Test the WebGL Build**
   - Use `Build And Run` to automatically test in browser
   - Or serve the build folder with a local web server:
     ```bash
     # Using Python
     cd Build/WebGL
     python -m http.server 8000
     
     # Using Node.js
     npx http-server Build/WebGL -p 8000
     ```
   - Open `http://localhost:8000` in your web browser

## WebGL Features

### Included Features
- Basic 3D scene with camera and lighting
- WebGL platform detection
- Browser console integration
- Sample interactive script (WebGLDemo.cs)

### WebGL-Specific Considerations
1. **File Size**: WebGL builds are compressed using Gzip for optimal loading
2. **Memory**: Configured with 256MB memory size (adjustable in ProjectSettings)
3. **Threading**: WebGL doesn't support multithreading
4. **Platform Detection**: Use `Application.platform == RuntimePlatform.WebGLPlayer`
5. **Browser Communication**: Use `Application.ExternalEval()` for JavaScript interaction

## Scripts

### WebGLDemo.cs
A demonstration script showing:
- Platform detection
- WebGL-specific conditional compilation
- Browser console logging
- Basic Unity-WebGL interaction

## Development

### Adding New Scenes
1. Create scene in `Assets/Scenes/`
2. Add to `File > Build Settings > Scenes In Build`
3. Update `EditorBuildSettings.asset` if needed

### WebGL Best Practices
- Keep build size small by removing unused assets
- Optimize textures and meshes for web delivery
- Use texture compression
- Test in multiple browsers (Chrome, Firefox, Safari, Edge)
- Consider loading screens for larger projects

## Browser Compatibility

WebGL builds work best in modern browsers:
- ✅ Google Chrome (recommended)
- ✅ Mozilla Firefox
- ✅ Microsoft Edge
- ✅ Safari (macOS/iOS)

## Troubleshooting

### Build Issues
- Ensure WebGL Build Support is installed in Unity Hub
- Check that IL2CPP is properly configured
- Clear the Library folder if experiencing cache issues

### Runtime Issues
- Check browser console for JavaScript errors
- Ensure web server has proper MIME types configured
- Some browsers may block WebGL without HTTPS in production

## Additional Resources

- [Unity WebGL Documentation](https://docs.unity3d.com/Manual/webgl.html)
- [WebGL Browser Compatibility](https://docs.unity3d.com/Manual/webgl-browsercompatibility.html)
- [Unity Manual](https://docs.unity3d.com/Manual/index.html)

## License

This project is part of GMD1-VIA.

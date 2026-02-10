using UnityEngine;
using UnityEngine.UI;
using System.Runtime.InteropServices;

/// <summary>
/// WebGL Demo Script - Demonstrates basic WebGL functionality
/// This script shows how to detect WebGL platform and interact with the browser
/// </summary>
public class WebGLDemo : MonoBehaviour
{
#if UNITY_WEBGL && !UNITY_EDITOR
    // Import JavaScript functions from WebGLBridge.jslib
    [DllImport("__Internal")]
    private static extern void WebGLLogToConsole(string message);
    
    [DllImport("__Internal")]
    private static extern string WebGLGetBrowserInfo();
    
    [DllImport("__Internal")]
    private static extern bool WebGLCheckSupport();
#endif

    [Header("UI Elements")]
    public Text statusText;
    public Text platformText;
    public Button testButton;

    private Camera mainCamera;

    private void Start()
    {
        // Cache camera reference for performance
        mainCamera = Camera.main;
        
        // Detect if running on WebGL platform
        bool isWebGL = Application.platform == RuntimePlatform.WebGLPlayer;
        
        if (platformText != null)
        {
            platformText.text = "Platform: " + Application.platform.ToString();
        }

        if (statusText != null)
        {
            if (isWebGL)
            {
                statusText.text = "Running on WebGL!";
                statusText.color = Color.green;
                
#if UNITY_WEBGL && !UNITY_EDITOR
                // Log to browser console using safe jslib plugin
                WebGLLogToConsole("GMD1 Unity WebGL Project initialized successfully");
#endif
            }
            else
            {
                statusText.text = "Not running on WebGL";
                statusText.color = Color.yellow;
            }
        }

        // Setup button if it exists
        if (testButton != null)
        {
            testButton.onClick.AddListener(OnTestButtonClick);
        }

        Debug.Log("GMD1 WebGL Project Initialized");
    }

    private void OnTestButtonClick()
    {
        Debug.Log("Button clicked in WebGL!");
        
        if (statusText != null)
        {
            statusText.text = $"Button Clicked at {Time.time:F2}s";
        }

#if UNITY_WEBGL && !UNITY_EDITOR
        // WebGL-specific code that only runs in WebGL builds
        // Using safe jslib plugin for JavaScript interaction
        WebGLLogToConsole($"Button clicked from Unity WebGL at {Time.time:F2}s");
#endif
    }

    private void Update()
    {
        // Example: Rotate the camera slightly for visual effect
        if (mainCamera != null)
        {
            mainCamera.transform.Rotate(0, Time.deltaTime * 5, 0);
        }
    }
}

using UnityEngine;

/// <summary>
/// WebGL Controller for GMD1 Project
/// This script demonstrates basic WebGL functionality in Unity
/// </summary>
public class WebGLController : MonoBehaviour
{
    private void Start()
    {
        Debug.Log("GMD1 WebGL Project Started!");
        
        #if UNITY_WEBGL && !UNITY_EDITOR
        Debug.Log("Running in WebGL build");
        #else
        Debug.Log("Running in Unity Editor or non-WebGL platform");
        #endif
        
        // Display platform information
        Debug.Log($"Platform: {Application.platform}");
        Debug.Log($"Unity Version: {Application.unityVersion}");
    }
    
    private void Update()
    {
        // Simple interaction example - Log when space is pressed
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Space key pressed - WebGL is responsive!");
        }
    }
}

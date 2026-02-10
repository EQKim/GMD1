using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// WebGL Demo Script - Demonstrates basic WebGL functionality
/// This script shows how to detect WebGL platform and interact with the browser
/// </summary>
public class WebGLDemo : MonoBehaviour
{
    [Header("UI Elements")]
    public Text statusText;
    public Text platformText;
    public Button testButton;

    private void Start()
    {
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
            statusText.text = "Button Clicked at " + Time.time.ToString("F2") + "s";
        }

#if UNITY_WEBGL && !UNITY_EDITOR
        // WebGL-specific code that only runs in WebGL builds
        Application.ExternalEval("console.log('Button clicked from Unity WebGL!');");
#endif
    }

    private void Update()
    {
        // Example: Rotate the camera slightly for visual effect
        if (Camera.main != null)
        {
            Camera.main.transform.Rotate(0, Time.deltaTime * 5, 0);
        }
    }
}

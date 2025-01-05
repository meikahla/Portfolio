using UnityEngine;

/// <summary>
/// Adjusts the background image to fit the screen size based on the device's aspect ratio.
/// Scales and positions the background to take up a specific percentage of the screen height,
/// while maintaining the correct aspect ratio based on the camera's orthographic view.
/// </summary>
public class BGFitToScreen : MonoBehaviour
{
    [SerializeField] GameObject backgroundImage;
    [SerializeField] Camera m_Camera;
    public float targetHeightPercentage = 0.75f; // Percentage of screen height for the background

    private void Start()
    {
        FitBackgroundToScreen();
    }

    // Adjust the background size to fit the screen
    void FitBackgroundToScreen()
    {
        // Get the device screen resolution and aspect ratio
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;
        float aspectRatio = screenWidth / screenHeight;

        // Set the camera's aspect ratio to match the device
        m_Camera.aspect = aspectRatio;

        // Calculate camera's height and width based on orthographic size and aspect ratio
        float camHeight = m_Camera.orthographicSize * 2f;
        float camWidth = camHeight * aspectRatio;

        // Get the background image size from the sprite renderer
        SpriteRenderer bgRenderer = backgroundImage.GetComponent<SpriteRenderer>();
        float bgHeight = bgRenderer.sprite.bounds.size.y;
        float bgWidth = bgRenderer.sprite.bounds.size.x;

        // Calculate the scale ratios to fit the background
        float scaleRatioH = camHeight * targetHeightPercentage / bgHeight;
        float scaleRatioW = camWidth / bgWidth;

        // Apply the new scale to the background
        backgroundImage.transform.localScale = new Vector3(scaleRatioW, scaleRatioH, 1);

        // Calculate the position to center the background at the top of the screen
        Vector2 topMid = new Vector2(m_Camera.pixelWidth / 2, m_Camera.pixelHeight - (m_Camera.pixelHeight * targetHeightPercentage / 2));
        backgroundImage.transform.position = m_Camera.ScreenToWorldPoint(topMid);
    }

    // Simple debug log helper
    public void debugText(string dText)
    {
        Debug.Log(dText);
    }
}

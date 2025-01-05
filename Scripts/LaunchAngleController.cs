using Blobcreate.ProjectileToolkit;
using UnityEngine;
using UnityEngine.UI;
using System;

public class LaunchAngleController : Singleton<LaunchAngleController>
{
    [SerializeField] private SimpleLauncher launcher;
    [SerializeField] private Slider sliderVisual;
    [SerializeField] private float minAngle = -90f;
    [SerializeField] private float maxAngle = 90f;
    [SerializeField] private float dragThreshold = 10f;

    private Vector2 dragStartPos;
    private float currentAngle;
    private bool isDragging;
    private bool isInputBlocked;

    // Event to notify when input blocking status changes
    public event Action OnInputBlockedChanged;

    void Start()
    {
        if (launcher == null)
        {
            Debug.LogError("Launcher is not assigned to LaunchAngleController.");
            return;
        }

        currentAngle = launcher.GetLaunchAngleX();
        UpdateSliderVisual();
    }

    void Update()
    {
        if (isInputBlocked)
            return; // Skip input handling if blocked

        if (Input.touchCount > 0)
        {
            HandleTouchInput();
        }
        else if (Input.GetMouseButton(0)) // Mouse input for Editor
        {
            HandleMouseInput();
        }
        else if (Input.GetMouseButtonUp(0)) // Mouse release for Editor
        {
            EndDrag();
        }
    }

    // Handles touch input
    private void HandleTouchInput()
    {
        Touch touch = Input.GetTouch(0);

        switch (touch.phase)
        {
            case TouchPhase.Began:
                StartDrag(touch.position);
                break;
            case TouchPhase.Moved:
                Drag(touch.position);
                break;
            case TouchPhase.Ended:
            case TouchPhase.Canceled:
                EndDrag();
                break;
        }
    }

    // Handles mouse input (for testing in Editor)
    private void HandleMouseInput()
    {
        Vector2 mousePos = Input.mousePosition;

        if (!isDragging)
        {
            StartDrag(mousePos);
        }
        else
        {
            Drag(mousePos);
        }
    }

    // Start the drag operation
    private void StartDrag(Vector2 startPos)
    {
        dragStartPos = startPos;
        isDragging = true;
        launcher.projectileController.hitEnabled = false;
    }

    // Handle the dragging behavior and calculate angle change
    private void Drag(Vector2 currentPos)
    {
        float dragDelta = currentPos.y - dragStartPos.y;

        if (Mathf.Abs(dragDelta) > dragThreshold)
        {
            float normalizedDrag = dragDelta / (Screen.height * 0.3f);
            float angleChange = normalizedDrag * (maxAngle - minAngle);
            currentAngle = Mathf.Clamp(currentAngle + angleChange, minAngle, maxAngle);

            launcher.SetAngleX(currentAngle);
            UpdateSliderVisual();

            dragStartPos = currentPos;

            // Update the launch vector directly after angle change
            launcher.UpdateLaunchVector();
        }
    }

    // End the drag operation, launch the projectile and block further input
    private void EndDrag()
    {
        if (!isDragging) return;

        isDragging = false;
        isInputBlocked = true; // Block further input
        launcher.LaunchProjectile();

        launcher.SetTSync(true); // Sync after launch
    }

    // Update the slider visual to match the current angle
    private void UpdateSliderVisual()
    {
        if (sliderVisual != null)
        {
            float normalizedValue = Mathf.InverseLerp(minAngle, maxAngle, currentAngle);
            sliderVisual.value = Mathf.Lerp(sliderVisual.minValue, sliderVisual.maxValue, normalizedValue);
        }
    }

    // Call this method to reset input block (e.g., after projectile is launched)
    public void ResetInputBlock()
    {
        isInputBlocked = false;
    }
}

using UnityEngine;

/// <summary>
/// Top-down city camera with an optional LookAt target.
///
/// Inspector setup:
///   • LookAt Target  — drag a Transform the camera pivots around / snaps to.
///                      If left empty the camera uses its current world position
///                      projected onto the XZ plane as the initial focus.
///   • Adjust Height, Pitch, and zoom bounds to suit your city scale.
/// </summary>
public class CameraController : MonoBehaviour
{
    // ── LookAt / pivot ────────────────────────────────────────────────────────
    [Header("LookAt Target")]
    [Tooltip("Transform the camera looks at and orbits. Leave empty to free-roam.")]
    [SerializeField] private Transform lookAtTarget;

    [Tooltip("Smoothing speed for following the LookAt target.")]
    [SerializeField] private float followSmoothing = 6f;

    // ── Top-down framing ──────────────────────────────────────────────────────
    [Header("Top-Down Framing")]
    [Tooltip("Height of the camera above the look-at point.")]
    [SerializeField] private float height = 120f;

    [Tooltip("Down-tilt in degrees (90 = straight down, lower = more angled).")]
    [SerializeField][Range(30f, 90f)] private float pitch = 70f;

    // ── Zoom ──────────────────────────────────────────────────────────────────
    [Header("Zoom")]
    [SerializeField] private float zoomSpeed    = 20f;
    [SerializeField] private float minHeight    = 30f;
    [SerializeField] private float maxHeight    = 250f;
    [SerializeField] private float zoomSmoothing = 8f;

    // ── Pan (only active when lookAtTarget is null) ───────────────────────────
    [Header("Free Pan (no LookAt target)")]
    [SerializeField] private float panSpeed          = 60f;
    [SerializeField] private float mousePanSensitivity = 0.4f;

    // ── Y-axis rotation (kept for RotateToBorough compatibility) ─────────────
    [Header("Rotation")]
    [SerializeField] private float rotationSpeed   = 60f;
    [SerializeField] private float lerpSpeed       = 0.12f;
    [SerializeField] private float minAngle        = -180f;
    [SerializeField] private float maxAngle        =  180f;

    // ── Runtime state ─────────────────────────────────────────────────────────
    private float targetYRotation  = 0f;
    private float currentYRotation = 0f;
    private float targetHeight;
    private Vector3 focusPoint;          // where the camera looks / orbits
    private bool isDragging = false;
    private Vector3 lastMouseWorld;

    // ── Lifecycle ─────────────────────────────────────────────────────────────
    void Start()
    {
        targetHeight = height;

        // Initialise focus: use target if assigned, else camera's XZ position
        if (lookAtTarget != null)
            focusPoint = new Vector3(lookAtTarget.position.x, 0f, lookAtTarget.position.z);
        else
            focusPoint = new Vector3(transform.position.x, 0f, transform.position.z);

        SnapToPosition();
    }

    void Update()
    {
        // Don't move camera while a UI panel is blocking input
        if (UIManager.Instance != null && UIManager.Instance.IsDecisionPanelOpen) return;
        if (PauseMenuController.Instance != null && PauseMenuController.Instance.IsPaused) return;

        HandleZoom();
        HandleRotation();

        if (lookAtTarget != null)
            TrackTarget();
        else
            HandlePan();

        ApplyTransform();
    }

    // ── Input handlers ────────────────────────────────────────────────────────

    void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        targetHeight -= scroll * zoomSpeed;
        targetHeight  = Mathf.Clamp(targetHeight, minHeight, maxHeight);
        height = Mathf.Lerp(height, targetHeight, Time.deltaTime * zoomSmoothing);
    }

    void HandleRotation()
    {
        // Keyboard rotate (A/D or arrow keys while holding Alt, or Q/E)
        float keyInput = 0f;
        if (Input.GetKey(KeyCode.Q)) keyInput = -1f;
        if (Input.GetKey(KeyCode.E)) keyInput =  1f;
        targetYRotation += keyInput * rotationSpeed * Time.deltaTime;
        targetYRotation  = Mathf.Clamp(targetYRotation, minAngle, maxAngle);

        currentYRotation = Mathf.Lerp(currentYRotation, targetYRotation, lerpSpeed);
    }

    void TrackTarget()
    {
        // Smoothly move focus toward the living target position
        Vector3 targetFlat = new Vector3(lookAtTarget.position.x, 0f, lookAtTarget.position.z);
        focusPoint = Vector3.Lerp(focusPoint, targetFlat, Time.deltaTime * followSmoothing);
    }

    void HandlePan()
    {
        // WASD / arrow keys
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        // Convert input to world-space pan accounting for current Y rotation
        Quaternion yRot = Quaternion.Euler(0f, currentYRotation, 0f);
        Vector3 move = yRot * new Vector3(h, 0f, v);
        focusPoint += move * panSpeed * Time.deltaTime;

        // Middle-mouse drag pan
        if (Input.GetMouseButtonDown(2))
        {
            isDragging    = true;
            lastMouseWorld = GetMouseWorldPoint();
        }
        if (Input.GetMouseButtonUp(2)) isDragging = false;

        if (isDragging)
        {
            Vector3 current = GetMouseWorldPoint();
            Vector3 delta   = lastMouseWorld - current;
            delta.y = 0f;
            focusPoint   += delta;
            lastMouseWorld = GetMouseWorldPoint();
        }
    }

    // ── Apply final camera transform ──────────────────────────────────────────

    void ApplyTransform()
    {
        // Build offset: rotate back by yaw, tilt by pitch
        Quaternion rotation = Quaternion.Euler(pitch, currentYRotation, 0f);
        Vector3    offset   = rotation * new Vector3(0f, 0f, -height);

        transform.position = new Vector3(focusPoint.x, 0f, focusPoint.z) + offset;
        transform.LookAt(new Vector3(focusPoint.x, 0f, focusPoint.z));
    }

    void SnapToPosition()
    {
        // Set instantly without lerp on first frame
        Quaternion rotation = Quaternion.Euler(pitch, currentYRotation, 0f);
        Vector3    offset   = rotation * new Vector3(0f, 0f, -height);
        transform.position = new Vector3(focusPoint.x, 0f, focusPoint.z) + offset;
        transform.LookAt(new Vector3(focusPoint.x, 0f, focusPoint.z));
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    /// <summary>
    /// Projects the mouse ray onto the Y=0 world plane to get a world point
    /// for middle-mouse drag panning.
    /// </summary>
    private Vector3 GetMouseWorldPoint()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        float t;
        Plane ground = new Plane(Vector3.up, Vector3.zero);
        if (ground.Raycast(ray, out t))
            return ray.GetPoint(t);
        return focusPoint;
    }

    // ── Public API ────────────────────────────────────────────────────────────

    /// <summary>
    /// Rotates the camera to face a compass angle. Kept from original
    /// CameraController for compatibility with any existing callers.
    /// </summary>
    public void RotateToBorough(float angle)
    {
        targetYRotation = Mathf.Clamp(angle, minAngle, maxAngle);
    }

    /// <summary>
    /// Instantly snap the LookAt focus to a world position (e.g. jump to borough).
    /// </summary>
    public void JumpToPoint(Vector3 worldPoint)
    {
        focusPoint = new Vector3(worldPoint.x, 0f, worldPoint.z);
    }

    /// <summary>
    /// Smoothly repoint the camera at a different Transform at runtime.
    /// </summary>
    public void SetLookAtTarget(Transform target)
    {
        lookAtTarget = target;
    }
}

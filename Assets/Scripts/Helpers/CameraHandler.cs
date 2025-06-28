using UnityEngine;
using UnityEngine.EventSystems;
/// <summary>
/// Camera handler class made using 
/// https://kylewbanks.com/blog/unity3d-panning-and-pinch-to-zoom-camera-with-touch-and-mouse-input 
/// as a base and adapted to personal preference
/// TODO: rewrite yourself if using for uni
/// </summary>
public class CameraHandler : MonoBehaviour
{
    private static readonly float PanSpeed = 4.5f;//5 works on pc, is playable on android but a bit fast. 4.5 looks good.
    private static readonly float ZoomSpeedTouch = 0.03f;//0.01 was slow but playable, 0.1 was fast but playable, 0.03 looks good.
    private static readonly float ZoomSpeedMouse = 1.5f;

    private static readonly float[] BoundsX = new float[] { -11f, 11f };//Can be changed if needed
    private static readonly float[] BoundsY = new float[] { -10f, 11f };//Can be changed if needed 
    private static readonly float[] ZoomBounds = new float[] { 2f, 10f };//Can be changed if needed

    private Camera cam;

    private Vector3 lastPanPosition;
    private int panFingerId; // Touch mode only
    private bool wasZoomingLastFrame; // Touch mode only
    private Vector2[] lastZoomPositions; // Touch mode only



    bool devMode;
    TileSelectionHandler tileSelectionHandler;
    TowerSelectionHandler towerSelectionHandler;
    GameManager gameManager;

    LayerMask defaultLayerMask;
    bool defaultMaskOn;
    Physics2DRaycaster raycaster;
    [SerializeField]
    GameObject cameraActivatedText;
    // Start is called before the first frame update
    void Start()
    {
        raycaster = GetComponent<Physics2DRaycaster>();
        defaultLayerMask = raycaster.eventMask;
        defaultMaskOn = true;
        cameraActivatedText.SetActive(false);
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        devMode = true;
        tileSelectionHandler = GameObject.Find("TileSelectionManager").GetComponent<TileSelectionHandler>();
        GameObject towerManagerObject = GameObject.Find("TowerSelectionManager");//Attached to player UI
        if (towerManagerObject != null)
        {
            devMode = false;
        }
    }
    void Awake()
    {
        cam = GetComponent<Camera>();
    }
    // Update is called once per frame
    void Update()
    {

        if (!defaultMaskOn)
        {
            if (Input.touchSupported)
            {
                HandleTouch();
            }
            else
            {
                HandleMouse();
            }
        }
    }

    /// <summary>
    /// Method called when clicking the camera button, toggles raycast event mask between all and ui only.
    /// Activates the camera controls when in ui only mode.
    /// (References do not show up, it is working)
    /// </summary>
    public void EnableControls()
    {
        if (defaultMaskOn)
        {
            raycaster.eventMask = 1 << 5;
        }
        else
        {
            raycaster.eventMask = defaultLayerMask;
        }
        //If mask was on camera was off and will be on now
        cameraActivatedText.SetActive(defaultMaskOn);
        defaultMaskOn = !defaultMaskOn;
    }
    /// <summary>
    /// Camera controls for devices not supporting touch
    /// </summary>
    void HandleMouse()
    {
        // On mouse down, capture it's position.
        // Otherwise, if the mouse is still down, pan the camera.
        if (Input.GetMouseButtonDown(0))
        {
            lastPanPosition = Input.mousePosition;
        }
        else
        { 
            if (Input.GetMouseButton(0))
            {
                PanCamera(Input.mousePosition);
            }
        }

        // Check for scrolling to zoom the camera
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        ZoomCamera(scroll, ZoomSpeedMouse);
    }

    /// <summary>
    /// Camera controls for devices supporting touch
    /// </summary>
    void HandleTouch()
    {

        switch (Input.touchCount)
        {
            case 1: // Panning
                wasZoomingLastFrame = false;
                // If the touch began, capture its position and its finger ID.
                // Otherwise, if the finger ID of the touch doesn't match, skip it.
                Touch touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Began)
                {
                    lastPanPosition = touch.position;
                    panFingerId = touch.fingerId;
                }
                else if (touch.fingerId == panFingerId && touch.phase == TouchPhase.Moved)
                {
                    PanCamera(touch.position);
                }
                break;

            case 2: // Zooming
                Vector2[] newPositions = new Vector2[] { Input.GetTouch(0).position, Input.GetTouch(1).position };
                if (!wasZoomingLastFrame)
                {
                    lastZoomPositions = newPositions;
                    wasZoomingLastFrame = true;
                }
                else
                {
                    // Zoom based on the distance between the new positions compared to the 
                    // distance between the previous positions.
                    float newDistance = Vector2.Distance(newPositions[0], newPositions[1]);
                    float oldDistance = Vector2.Distance(lastZoomPositions[0], lastZoomPositions[1]);
                    float offset = newDistance - oldDistance;

                    ZoomCamera(offset, ZoomSpeedTouch);

                    lastZoomPositions = newPositions;
                }
                break;

            default:
                wasZoomingLastFrame = false;
                break;
        }
    }

    void PanCamera(Vector3 newPanPosition)
    {
        // Determine how much to move the camera
        Vector3 offset = cam.ScreenToViewportPoint(lastPanPosition - newPanPosition);
        //The (PanSpeed + cam.orthographicSize) helps with the speed feeling, without it - too slow
        Vector3 moveTo = new Vector3(offset.x * (PanSpeed + cam.orthographicSize), offset.y * (PanSpeed + cam.orthographicSize), 0);

        // Perform the movement
        transform.Translate(moveTo, Space.World);

        // Ensure the camera remains within bounds.
        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(transform.position.x, BoundsX[0], BoundsX[1]);
        pos.y = Mathf.Clamp(transform.position.y, BoundsY[0], BoundsY[1]);
        transform.position = pos;

        // Cache the position
        lastPanPosition = newPanPosition;
    }

    void ZoomCamera(float offset, float speed)
    {
        if (offset == 0)
        {
            return;
        }
        cam.orthographicSize = Mathf.Clamp(cam.orthographicSize - (offset * speed), ZoomBounds[0], ZoomBounds[1]);
    }
}

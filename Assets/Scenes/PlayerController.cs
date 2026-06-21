using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Lanes")]
    public float laneDistance = 2f;       // gap between lanes
    public float laneChangeSpeed = 12f;   // how fast you slide across
    private int currentLane = 1;          // 0 = left, 1 = middle, 2 = right

    [Header("Run")]
    public float forwardSpeed = 8f;
    public float maxSpeed = 22f;
    public float acceleration = 0.1f;     // speed added per second

    [Header("Jump / Gravity")]
    public float jumpForce = 8f;
    public float gravity = -24f;
    private float verticalVelocity;

    [Header("Slide")]
    public float slideDuration = 0.8f;
    private bool isSliding;
    private float slideTimer;
    private float defaultHeight;

    [Header("Swipe")]
    public float minSwipeDistance = 50f;

    private CharacterController controller;
    private Vector2 touchStart;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        defaultHeight = controller.height;
    }

    void Update()
    {
        forwardSpeed = Mathf.Min(forwardSpeed + acceleration * Time.deltaTime, maxSpeed);
        HandleInput();
        HandleSlideTimer();
        Move();
    }

    void HandleInput()
    {
        // Keyboard — for testing in the editor
        if (Input.GetKeyDown(KeyCode.LeftArrow))  MoveLane(-1);
        if (Input.GetKeyDown(KeyCode.RightArrow)) MoveLane(1);
        if (Input.GetKeyDown(KeyCode.UpArrow))    Jump();
        if (Input.GetKeyDown(KeyCode.DownArrow))  StartSlide();

        // Touch — on the device
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
                touchStart = touch.position;
            else if (touch.phase == TouchPhase.Ended)
                DetectSwipe(touchStart, touch.position);
        }
    }

    void DetectSwipe(Vector2 start, Vector2 end)
    {
        Vector2 delta = end - start;
        if (delta.magnitude < minSwipeDistance) return;

        if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
            MoveLane(delta.x > 0 ? 1 : -1);   // horizontal swipe
        else if (delta.y > 0)
            Jump();                           // swipe up
        else
            StartSlide();                     // swipe down
    }

    void MoveLane(int direction)
    {
        currentLane = Mathf.Clamp(currentLane + direction, 0, 2);
    }

    void Jump()
    {
        if (controller.isGrounded)
            verticalVelocity = jumpForce;
    }

    void StartSlide()
    {
        if (!isSliding)
        {
            isSliding = true;
            slideTimer = slideDuration;
            controller.height = defaultHeight * 0.5f;
        }
    }

    void HandleSlideTimer()
    {
        if (!isSliding) return;
        slideTimer -= Time.deltaTime;
        if (slideTimer <= 0f)
        {
            isSliding = false;
            controller.height = defaultHeight;
        }
    }

    void Move()
    {
        if (controller.isGrounded && verticalVelocity < 0f)
            verticalVelocity = -1f;                  // stick to ground
        verticalVelocity += gravity * Time.deltaTime;

        float targetX = (currentLane - 1) * laneDistance;
        float newX = Mathf.Lerp(transform.position.x, targetX, laneChangeSpeed * Time.deltaTime);
        float lateral = newX - transform.position.x;

        Vector3 motion = new Vector3(
            lateral,
            verticalVelocity * Time.deltaTime,
            forwardSpeed * Time.deltaTime);
        controller.Move(motion);
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.CompareTag("Obstacle"))
            GameManager.Instance.GameOver();
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    #region Singleton Code
    private static Player _instance;

    public static Player Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Debug.LogWarning("Attempted to Instantiate multiple Players in one scene!");
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    private void OnDestroy()
    {
        if (this == _instance) { _instance = null; }
    }
    #endregion

    public bool canMove;

    // If the player is standing on an object tagged with "Ground", they are considered grounded
    [SerializeField]
    private bool isGrounded;


    public const float jumpVelocity = 7;

    public Rigidbody2D rb;


    private double horizVelocity;
    private double movementAcceleration;
    [SerializeField]
    private float accelerationRate;
    [SerializeField]
    private float maximumHorizSpeed;
    [SerializeField]
    private float jumpForce;
    [SerializeField]
    private float airborneHorizAccelerationMultiplier;

    public bool allowExceedingMaxHorizVelocity;

    public Launchpad launchpadWithAuthority; // Player can only be affected by one launch pad at a time

    // We can add to this list to reduce or increase acceleration further and apply them all when calculating
    public List<float> additionalHorizAccelerationModifers;

    public int abilityLevel; // 1 = Stronger Light, 2 = Dash, 3 = Light Shot

    public Vector3 lastSaveLocation;

    [SerializeField]
    private float dashForce;

    public bool dashReady;

    private bool isDashing;

    public LightOrb lightOrbWithinRange;

    [SerializeField]
    private SpriteRenderer spriteRenderer;

    // Delegate the responsibility for handling lantern stuff to LightHandler if it is present
    public Lantern LanternWithinRange
    {
        get { return lanternWithinRange; }
        set
        {
            if (LightHandler.Instance != null)
            {
                LightHandler.Instance.lanternWithinRange = value;
            }
            lanternWithinRange = value;
        }
    }
    private Lantern lanternWithinRange; // The lantern the player is currently able to interact with

    public Gate gateWithinRange;

    // Start is called before the first frame update
    void Start()
    {
        TextManager.Instance.DisplayFixedText("Press <b><i>Enter</b></i> to dismiss text", "Use <b><i>W</b></i> and <b><i>D</b></i> to move");
        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
        }
        if (accelerationRate == 0)
        {
            accelerationRate = 1;
            Debug.LogWarning("Acceleration Rate was 0, defaulting to 1");
        }
        if (maximumHorizSpeed == 0)
        {
            maximumHorizSpeed = 1;
            Debug.LogWarning("MaxHorizSpeed was 0, defaulting to 1");
        }
        if (jumpForce == 0)
        {
            jumpForce = 200;
            Debug.LogWarning("Jump Force was 0, defaulting to 200");
        }
        if (dashForce == 0)
        {
            dashForce = 200;
        }
        if (lastSaveLocation == Vector3.zero)
        {
            lastSaveLocation = transform.position;
        }
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Update isGrounded every frame
        isGrounded = (GetGroundColliderUnderPlayer() != null);
        if (isGrounded)
        {
            dashReady = true;
        }
        if (transform.position.y < -0.7f)
        {
            Die();
        }

        HandleMovement();
    }

    private void HandleMovement()
    {
        if (canMove)
        {
            float xForceToAdd = (float) movementAcceleration * accelerationRate;
            if (!isGrounded)
            {
                xForceToAdd *= airborneHorizAccelerationMultiplier;
            }
            foreach (float f in additionalHorizAccelerationModifers)
            {
                xForceToAdd *= f;
            }
            rb.AddForce(new Vector2(xForceToAdd * Time.deltaTime, 0));

            if (!allowExceedingMaxHorizVelocity && !isDashing)
            {
                rb.velocity = new Vector2(Mathf.Clamp(rb.velocity.x, -maximumHorizSpeed, maximumHorizSpeed), rb.velocity.y);
            }
        }
    }

    public void OnMove(InputValue value)
    {
        movementAcceleration = value.Get<float>();
        if (canMove)
        {
            if (movementAcceleration > 0)
            {
                spriteRenderer.flipX = true;
            }
            if (movementAcceleration < 0)
            {
                spriteRenderer.flipX = false;
            }
        }
    }

    public void OnJump()
    {
        if (canMove && isGrounded)
        {
            rb.AddForce(new Vector2(0, jumpForce));
        }
    }

    public void OnInterract()
    {
        if (lanternWithinRange != null)
        {
            if (abilityLevel >= 1)
            {
                lanternWithinRange.SetLitState(true);
                TextManager.Instance.DisplayFloatingText("");
            }
            else
            {

            }
        }
        if (gateWithinRange != null && gateWithinRange.isUnlocked)
        {
            if (gateWithinRange.nextScene == "")
            {
                Debug.LogError("Gate had no attached Scene to move to!");
            } else
            {
                SceneManager.LoadSceneAsync(gateWithinRange.nextScene);
                TextManager.Instance.DisplayFloatingText("");
            }
        }
        if (lightOrbWithinRange != null)
        {
            lightOrbWithinRange.Pickup();
            TextManager.Instance.DisplayFloatingText("");
        }
    }

    public void OnNextSentence()
    {
        TextManager.Instance.NextSentence();
    }

    public void OnDash()
    {
        if (abilityLevel >= 2 && dashReady && canMove)
        {
            StartCoroutine(HandleDash());
        }
    }

    private IEnumerator HandleDash()
    {
        isDashing = true;
        rb.velocity = new Vector2(rb.velocity.x, 0);
        additionalHorizAccelerationModifers.Add(0);
        Vector2 dashVector = new Vector2(dashForce, 125);
        if (!spriteRenderer.flipX)
        {
            dashVector.x *= -1;
        }
        rb.AddForce(dashVector);
        dashReady = false;
        yield return new WaitForSeconds(0.35f);
        additionalHorizAccelerationModifers.Remove(0);
        isDashing = false;
    }

    public void Die()
    {
        transform.position = lastSaveLocation;
    }

    // This returns the nearest Collider in case we want to use it for something. For now we're just checking that it is non-null
    // to see if the player is grounded.
    private Collider2D GetGroundColliderUnderPlayer()
    {
        // These values are complete trial and error. I tried calculating it but got totally lost and just decided to eyeball it.
        Collider2D[] cols = Physics2D.OverlapAreaAll(new Vector2(transform.position.x - 0.45f, transform.position.y - 0.5f),
            new Vector2(transform.position.x + 0.45f, transform.position.y - 0.75f));

        if (cols != null)
        {
            foreach (Collider2D col in cols)
            {
                if (col.gameObject.CompareTag("Ground"))
                {
                    return col;
                }
            }
        }
        return null;
    }
}

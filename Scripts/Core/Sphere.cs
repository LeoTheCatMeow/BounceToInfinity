using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sphere : MonoBehaviour
{
    public float xAxisAcceleration;
    public float zAxisAcceleration;
    public float maxGroundSpeed;
    public float jumpSpeed;
    [Tooltip("baseJumpChargeFactor, as if released immediately")]
    public float baseJumpChargeFactor;
    [Tooltip("maxJumpChargeFactor, jump charged based on delta time")]
    public float maxJumpChargeFactor;
    [Tooltip("airTimeAccelerationReductionFactor")]
    public float airTimeAccelerationReductionFactor;
    [Tooltip("groundedFrictionFactor")]
    public float groundedFrictionFactor;
    //[Tooltip("groundPointDetectionTolerance, how far can the contact point be from the bottom of the sphere?")]
    //public float groundPointDetectionTolerance;
    public Transform tracker;
    ////public Transform groundPoint;  
    public GameObject theme1;
    public GameObject theme2;
  
    private Rigidbody rb;
    private AudioSource audioSource;
    private ParticleSystem particles;
    private ParticleSystem flare;
    private GameObject theme;
    private GameObject ground;
    private bool grounded;
    private bool jumpReady;
    private bool charging;
    private float jumpCharge;

    void Awake()
    {
        GameEvents._GameReset += OnGameReset;
        GameEvents._GameStart += OnGameStart;
        GameEvents._ReturnToMenu += OnReturnToMenu;
        GameEvents._RelicActivated += SelectTheme;
    }

    void OnDestroy()
    {
        GameEvents._GameReset -= OnGameReset;
        GameEvents._GameStart -= OnGameStart;
        GameEvents._ReturnToMenu -= OnReturnToMenu;
        GameEvents._RelicActivated -= SelectTheme;
    }

    void Start()
    {
        SelectTheme(PlayerData.relicActivated);

        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        particles = theme.transform.GetChild(2).GetComponent<ParticleSystem>();
        flare = theme.transform.GetChild(3).GetComponent<ParticleSystem>();
    }

    private void OnGameStart()
    {
        rb.isKinematic = false;
    }

    void Update()
    {
        //update tracker
        tracker.position = transform.position;

        //rotation (tracked by camera)
        tracker.eulerAngles += new Vector3(0f, MobileInput.screenDragDelta * 90f, 0f) * (1f + PlayerData.turnSensitivityPref);

        //jump
        if (MobileInput.buttonPressed && jumpReady && !charging)
        {
            StartCoroutine(ChargeJump());
        }
        if (!MobileInput.buttonPressed && charging)
        {
            rb.velocity += tracker.up * jumpSpeed * jumpCharge * (PlayerData.relicActivated == 3 ? 1.2f : 1f);
            jumpReady = false;
            charging = false;      
        }
    }

    IEnumerator ChargeJump()
    {
        jumpCharge = baseJumpChargeFactor;
        charging = true;
        while (MobileInput.buttonPressed)
        {
            jumpCharge = Mathf.Min(jumpCharge + Time.deltaTime / (0.5f + PlayerData.jumpSensitivityPref), maxJumpChargeFactor);
            GameEvents.UpdateJumpCharge((jumpCharge - baseJumpChargeFactor) / (maxJumpChargeFactor - baseJumpChargeFactor));
            yield return new WaitForFixedUpdate();
        }
        GameEvents.UpdateJumpCharge(0f);
    }

    void FixedUpdate()
    {
        //update tracker
        tracker.position = transform.position;

        //horizontal motion (x and z axis)
        float xInput = MobileInput.JoystickInput.x;
        float zInput = MobileInput.JoystickInput.y;

        float airTimeAccelerationReduction = grounded ? 1f : airTimeAccelerationReductionFactor * (PlayerData.relicActivated == 1 ? 1f : 3f);
        rb.velocity += tracker.forward * zInput * zAxisAcceleration * airTimeAccelerationReduction * (0.5f + PlayerData.moveSensitivityPref) + 
        tracker.right * xInput * xAxisAcceleration * airTimeAccelerationReduction * (0.5f + PlayerData.moveSensitivityPref);

        //friction (built-in physics friction does not apply well to spheres, modifying drag would affect air speed)
        if (grounded)
        {
            rb.velocity *= groundedFrictionFactor;
        }

        //ground speed limit
        Vector2 groundSpeed = new Vector2(rb.velocity.x, rb.velocity.z);
        Vector2 limitedGroundSpeed = groundSpeed.normalized * Mathf.Min(groundSpeed.magnitude, maxGroundSpeed);
        rb.velocity = new Vector3(limitedGroundSpeed.x, rb.velocity.y, limitedGroundSpeed.y);
    }

    void OnCollisionEnter(Collision collision)
    {
        //update tracker again for better groundPoint precision
        tracker.position = transform.position;

        if (!grounded)
        {
            grounded = true;
            jumpReady = true;
            ground = collision.gameObject;

            flare.Emit(1);
            audioSource.pitch = Random.Range(0.5f, 2f);
            audioSource.Play();                                                             
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject == ground)
        {
            grounded = false;
            ground = null;
        }
    }

    private void OnGameReset()
    {
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;
        rb.velocity = Vector3.zero;
        grounded = false;
        ground = null;
        charging = false;

        particles.Clear();
    }

    private void OnReturnToMenu()
    {
        rb.isKinematic = true;
    }

    private void SelectTheme(int i)
    {
        if (PlayerData.relicActivated == 5)
        {
            theme1.SetActive(false);
            theme = theme2;
        }
        else if (theme != theme1)
        {
            theme2.SetActive(false);
            theme = theme1;
        }
        theme.SetActive(true);
    }
}

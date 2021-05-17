/*
 * written by Joseph Hocking 2017
 * released under MIT license
 * text of license https://opensource.org/licenses/MIT
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]

// basic WASD-style movement control
public class FpsMovement : MonoBehaviour
{
    public Camera headCam;
    public float walkSpeed;
    public float runSpeed;
    
    AudioSource m_AudioSource;
    private CharacterController charController;
    private Animator animator;
    private PlayerStats playerStats;
    // private Rigidbody rb;

    public float gravity = -9.8f;
    public float rollForce = 5.0f;
    public float mass;
    public float sensitivityHor = 9.0f;
    public float sensitivityVert = 9.0f;
    public float minimumVert = -45.0f;
    public float maximumVert = 45.0f;
    public bool isSprintingForward;
    
    private Vector3 impact = Vector3.zero;
    private Vector3 rollPos;
    private Vector3 fpsPos;
    private float moveSpeed;
    private float rotationVert;
    private bool rotateCameraEnabled = true;
    private Quaternion cameraRot;
    private float rollRate = 1f;
    private float nextRoll;
    private bool isRolling;
    
    private int potionHealth = 20;
    private int potionOvershield = 10;
    private float zoomSpeed = 10f;
    private float startZoomSpeed = 2f;
    private bool started;
    private bool camAtPlayer;

    private PlayerCombat combat;
    private MazeConstructor maze;

    private void Start()
    {
        m_AudioSource = GetComponent<AudioSource>();
        charController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        playerStats = GetComponent<PlayerStats>();
        maze = GetComponent<MazeConstructor>();
        combat = GetComponent<PlayerCombat>();
        fpsPos = new Vector3(0.3f, 1.6f, -1.05f);
        rollPos = new Vector3 (0f, 1.4f, -2f);
        StartCoroutine(LookAtPlayerFor(2.5f));
    }

    private void Update()
    {
        // Ensure the camera always looks at the player
        transform.LookAt(transform.parent);
        
        if (impact.magnitude > 0.2) 
            charController.Move(impact * Time.deltaTime);
        // consumes the impact energy each cycle:
        impact = Vector3.Lerp(impact, Vector3.zero, 5*Time.deltaTime);
        
        if (combat.controlEnabled)
        {
            MoveCharacter();
            if (!combat.heavyAttack)
            {
                if (isRolling)
                {
                    headCam.transform.localPosition =
                        Vector3.Lerp(headCam.transform.localPosition, rollPos, zoomSpeed * Time.deltaTime);
                }
                else
                {
                    RotateCamera();
                    RotateCharacter();
                    if (fpsPos != headCam.transform.localPosition)
                    {
                        headCam.transform.localPosition = Vector3.Lerp(headCam.transform.localPosition, fpsPos,
                            zoomSpeed * Time.deltaTime);
                    }
                }
            }
        }
    }
    

    private void MoveCharacter()
    {
        // COMMENTED OUT FOR SPRINT IMPLEMENTATION
        // float deltaX = Input.GetAxis("Horizontal") * moveSpeed;
        // float deltaZ = Input.GetAxis("Vertical") * moveSpeed;
        float deltaX = Input.GetAxis("Horizontal");
        float deltaZ = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(deltaX, 0, deltaZ);
        // COMMENTED OUT FOR SPRING IMPLEMENTATION
        // movement = Vector3.ClampMagnitude(movement, moveSpeed);

        if (movement != Vector3.zero && !Input.GetKey(KeyCode.LeftShift))
        {
            isSprintingForward = false;
            // Walk
            Walk();
            if (!m_AudioSource.isPlaying)
            {
                m_AudioSource.Play();
            }
        }
        else if (movement != Vector3.zero && Input.GetKey(KeyCode.LeftShift))
        {
            // Run
            if (playerStats.UseStamina(10*Time.deltaTime))
            {
                if (Input.GetKey(KeyCode.W))
                    isSprintingForward = true;
                else
                {
                    isSprintingForward = false;
                }
                Run();
            }
            else
            {
                isSprintingForward = false;
                Walk();
            }
        }
        else if (movement == Vector3.zero)
        {
            isSprintingForward = false;
            Idle();
            m_AudioSource.Stop();
        }
        
        if (Input.GetKeyDown(KeyCode.Mouse1) && Time.time > nextRoll && movement != Vector3.zero)
        {
            nextRoll = Time.time + rollRate;
            rotateCameraEnabled = false;
            StartCoroutine(Roll());
            rotateCameraEnabled = true;
        }

        movement *= moveSpeed;
        movement.y = gravity;
        movement *= Time.deltaTime;
        movement = transform.TransformDirection(movement);

        charController.Move(movement);
    }

    private void Idle()
    {
        moveSpeed = 0f;
        animator.SetFloat("Speed", 0f, 0.2f, Time.deltaTime);
    }

    private void Walk()
    {
        moveSpeed = walkSpeed;
        animator.speed = 1.33f;
        if (Input.GetKey(KeyCode.W))
        {
            animator.SetFloat("Speed", 0.5f, 0.2f, Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            animator.SetFloat("Speed", 1f, 0.2f, Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.A))
        {
            animator.SetFloat("Speed", 1.5f, 0.2f, Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            animator.SetFloat("Speed", 2f, 0.2f, Time.deltaTime);
        }
    }
    
    private void Run()
    {
        moveSpeed = runSpeed;
        animator.speed = 1.2f;
        animator.SetFloat("Speed", 2.5f, 0.2f, Time.deltaTime);
    }

    private IEnumerator Roll()
    {
        isRolling = true;
        animator.speed = 1f;
        animator.SetTrigger("Roll");
        if (Input.GetKey(KeyCode.W))
        {
            animator.SetFloat("RollType", 0);
            AddImpact(transform.forward, rollForce);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            animator.SetFloat("RollType", 1.5f);
            AddImpact((transform.forward * -1), rollForce);
        }
        else if (Input.GetKey(KeyCode.A))
        {
            animator.SetFloat("RollType", 1);
            AddImpact((transform.right * -1), rollForce);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            animator.SetFloat("RollType", 0.5f);
            AddImpact(transform.right, rollForce);
        }
        yield return new WaitForSeconds(0.78f);
        isRolling = false;
    }

    private void RotateCharacter()
    {
        transform.Rotate(0, Input.GetAxis("Mouse X") * sensitivityHor, 0);
    }

    private void RotateCamera()
    {
        rotationVert -= Input.GetAxis("Mouse Y") * sensitivityVert;
        rotationVert = Mathf.Clamp(rotationVert, minimumVert, maximumVert);

        headCam.transform.localEulerAngles = new Vector3(
            rotationVert, headCam.transform.localEulerAngles.y, 0
        );
    }
    
    private void AddImpact(Vector3 dir, float force)
    {
        dir.Normalize();
        if (dir.y < 0) 
            dir.y = -dir.y; // reflect down force on the ground
        impact += dir.normalized * force / mass;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        int currentHealth = playerStats.currentHealth;
        int maxHealth = playerStats.maxHealth;
        int currentOvershield = playerStats.currentOvershield;
        int maxOvershield = playerStats.maxOvershield;
        
        if (other.gameObject.CompareTag("Health Potion"))
        {
            if (currentHealth < maxHealth)
            {
                if (currentHealth + potionHealth >= maxHealth)
                {
                    playerStats.AddHealth(maxHealth - currentHealth);
                }
                else
                {
                    playerStats.AddHealth(potionHealth);
                }
                // other.gameObject.SetActive(false);
            }
        }
        if (other.gameObject.CompareTag("Overshield Potion"))
        {
            if (currentOvershield < maxOvershield)
            {
                if (currentOvershield + potionOvershield >= maxOvershield)
                {
                    playerStats.AddOvershield(maxOvershield - currentOvershield);
                }
                else
                {
                    playerStats.AddOvershield(potionOvershield);
                }
                // other.gameObject.SetActive(false);
            }
        }
        Destroy(other);
        other.gameObject.SetActive(false);
    }

    private IEnumerator LookAtPlayerFor(float time)
    {
        yield return new WaitForSeconds(time);
        started = true;
    }
}

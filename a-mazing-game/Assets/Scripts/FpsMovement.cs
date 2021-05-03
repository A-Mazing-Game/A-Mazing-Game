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
    [SerializeField] private Camera headCam;
    [SerializeField] private Camera standardCam;
    [SerializeField] private float walkSpeed;
    [SerializeField] private float runSpeed;
    
    AudioSource m_AudioSource;
    private CharacterController charController;
    private Animator animator;
    private Rigidbody rb;

    public float gravity = -9.8f;
    public float rollForce = 5.0f;
    public float mass;
    public float sensitivityHor = 9.0f;
    public float sensitivityVert = 9.0f;
    public float minimumVert = -45.0f;
    public float maximumVert = 45.0f;

    private Vector3 impact = Vector3.zero;
    private float moveSpeed;
    private float rotationVert;
    private bool rotateCameraEnabled = true;
    private Quaternion cameraRot;
    private float rollRate = 1f;
    private float nextRoll;
    
    private void Start()
    {
        m_AudioSource = GetComponent<AudioSource>();
        charController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (impact.magnitude > 0.2) 
            charController.Move(impact * Time.deltaTime);
        // consumes the impact energy each cycle:
        impact = Vector3.Lerp(impact, Vector3.zero, 5*Time.deltaTime);
        
        MoveCharacter();
        RotateCamera();
        RotateCharacter();
    }
    
    private void LateUpdate()
    {
        Vector3 pos = transform.position;
        Quaternion rot = transform.rotation;
        if (rotateCameraEnabled)
        {
            standardCam.transform.position = pos + transform.TransformDirection(new Vector3(0,1.5f,-1.75f));
            standardCam.transform.rotation = rot;
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
            Run();
        }
        else if (movement == Vector3.zero)
        {
            Idle();
            m_AudioSource.Stop();
        }
        
        if (Input.GetKeyDown(KeyCode.R) && Time.time > nextRoll && movement != Vector3.zero)
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
        standardCam.gameObject.SetActive(true);
        headCam.gameObject.SetActive(false);
        yield return new WaitForSeconds(0.78f);
        headCam.gameObject.SetActive(true);
        standardCam.gameObject.SetActive(false);
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
}

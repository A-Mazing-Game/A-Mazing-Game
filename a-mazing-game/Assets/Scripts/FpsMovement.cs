/*
 * written by Joseph Hocking 2017
 * released under MIT license
 * text of license https://opensource.org/licenses/MIT
 */

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]

// basic WASD-style movement control
public class FpsMovement : MonoBehaviour
{
    [SerializeField] private Camera headCam;
    private float moveSpeed;
    [SerializeField] private float walkSpeed;
    [SerializeField] private float runSpeed;
    private Animator animator;

    // public float speed = 6.0f;
    public float gravity = -9.8f;

    public float sensitivityHor = 9.0f;
    public float sensitivityVert = 9.0f;

    public float minimumVert = -45.0f;
    public float maximumVert = 45.0f;

    private float rotationVert = 0;

    private bool swordOut;
    private float attackRate = 1.5f;
    private float nextAttack;

    AudioSource m_AudioSource;

    private CharacterController charController;

    private void Start()
    {
        m_AudioSource = GetComponent<AudioSource>();
        charController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        MoveCharacter();
        RotateCharacter();
        RotateCamera();
        if (Input.GetKeyDown(KeyCode.Mouse0) && Time.time > nextAttack)
        {
            nextAttack = Time.time + attackRate;
            StartCoroutine(Slash());
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
        animator.SetFloat("Speed", 2.5f, 0.2f, Time.deltaTime);
    }

    private IEnumerator Slash()
    {
        animator.SetLayerWeight(animator.GetLayerIndex("Attack Layer"), 1);
        animator.SetTrigger("Slash");

        yield return new WaitForSeconds(1.5f);
        animator.SetLayerWeight(animator.GetLayerIndex("Attack Layer"), 0);
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
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class FpsMovement : MonoBehaviour
{
    #region Public Members
    // public Transform player;
    public Inventory inventory;
    public HUD Hud;
    public InventoryItemBase mCurrentItem;
    public Camera headCam;
    public GameObject Hand;
    
    public float walkSpeed;
    public float runSpeed;
    public float gravity = -9.8f;
    public float rollForce = 5.0f;
    public float mass;
    public float sensitivityHor = 9.0f;
    public float sensitivityVert = 9.0f;
    public float minimumVert = -45.0f;
    public float maximumVert = 45.0f;
    public bool isSprintingForward;
    public bool isDead;
    #endregion
    
    #region Private Members
    private CharacterController charController;
    private Animator animator;
    private PlayerStats playerStats;
    // private Rigidbody rb;
    private PlayerCombat combat;
    private MazeConstructor maze;
    AudioSource m_AudioSource;
    private InteractableItemBase mInteractItem;
    private RaycastHit camHit;
    private Vector3 impact = Vector3.zero;
    private Vector3 rollPos;
    private Vector3 fpsPos;
    private Vector3 relativePos;
    private Quaternion cameraRot;
    private float distanceOffset;
    private float distance = 0.5f;
    private float moveSpeed;
    private float rotationVert;
    private bool rotateCameraEnabled = true;
    private float rollRate = 1f;
    private float nextRoll;
    private bool isRolling;
    private int potionHealth = 20;
    private int potionOvershield = 10;
    private float zoomSpeed = 10f;
    private float startZoomSpeed = 2f;
    private bool started;
    private bool camAtPlayer;
    private bool mCanTakeDamage = true;

    #endregion

    private void Start()
    {
        sensitivityHor = PlayerPrefs.GetFloat("sensitivity", 4f);
        sensitivityVert = PlayerPrefs.GetFloat("sensitivity", 4f);
        m_AudioSource = GetComponent<AudioSource>();
        charController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        playerStats = GetComponent<PlayerStats>();
        maze = GetComponent<MazeConstructor>();
        combat = GetComponent<PlayerCombat>();
        fpsPos = headCam.transform.localPosition;
        rollPos = new Vector3(0f, 1.4f, -2f);
        isDead = GetComponent<PlayerCombat>().isDead;
        inventory.ItemUsed += Inventory_ItemUsed;
        inventory.ItemRemoved += Inventory_ItemRemoved;
}

    private void Update()
    {
        // Prevent the camera from clipping through walls
        float clipOffset = 0.2f;
        Vector3 playerCenter = new Vector3 (0, 0.7f, 0);
        if (Physics.Linecast(transform.position + playerCenter, (transform.position + playerCenter) + (transform.localRotation * fpsPos), out camHit))
        {
            Vector3 moveTo = new Vector3 (fpsPos.x, fpsPos.y,-Mathf.Abs(transform.position.z - camHit.point.z) + clipOffset);
            headCam.transform.localPosition =
                Vector3.Lerp(headCam.transform.localPosition, moveTo, 10 * Time.deltaTime);
        }
        else 
            headCam.transform.localPosition =
                Vector3.Lerp(headCam.transform.localPosition, fpsPos, 10 * Time.deltaTime);
       
        // Handle player interaction with items
        if (mInteractItem != null && Input.GetKeyDown(KeyCode.F))
        {
            InteractWithItem();
        }
        
        // Apply roll force
        if (impact.magnitude > 0.2)
            charController.Move(impact * Time.deltaTime);
        // consumes the impact energy each cycle:
        impact = Vector3.Lerp(impact, Vector3.zero, 5 * Time.deltaTime);

        // Control the character
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
                }
            }
        }

        // Determine settings for currently equipped weapon
        if (IsArmed)
        {
            animator.SetBool("isArmed", true);
            if (CarriesItem("Sword Epic"))
            {
                animator.SetBool("greatSword", true);
                animator.SetBool("katana", false);
                combat.attackRange = 1.0f;
                combat.attackRate = 1.5f;
            }
            else if (CarriesItem("Katana"))
            {
                animator.SetBool("greatSword", false);
                animator.SetBool("katana", true);
                combat.attackRange = 0.75f;
                combat.attackRate = 1f;
            }
        }
        else
        {
            animator.SetBool("isArmed", false);
            animator.SetBool("greatSword", false);
            animator.SetBool("katana", false);
        }
    }

    void FixedUpdate()
    {
        if (!isDead)
        {
            // Drop item
            if (mCurrentItem != null && Input.GetKeyDown(KeyCode.R))
            {
                DropCurrentItem();
            }
        }
    }

    private void MoveCharacter()
    {
        float deltaX = Input.GetAxis("Horizontal");
        float deltaZ = Input.GetAxis("Vertical");
        Vector3 movement = new Vector3(deltaX, 0, deltaZ);

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
    
    // Following code taken from Jayanam on YouTube
    #region Inventory

    private void Inventory_ItemRemoved(object sender, InventoryEventArgs e)
    {
        InventoryItemBase item = e.Item;

        GameObject goItem = (item as MonoBehaviour).gameObject;
        goItem.SetActive(true);
        goItem.transform.parent = null;

        if (item == mCurrentItem)
            mCurrentItem = null;
    }

    public void SetItemActive(InventoryItemBase item, bool active)
    {
        GameObject currentItem = (item as MonoBehaviour).gameObject;
        currentItem.SetActive(active);
        currentItem.transform.parent = active ? Hand.transform : null;
    }

    private void Inventory_ItemUsed(object sender, InventoryEventArgs e)
    {
        if (e.Item.ItemType != EItemType.Consumable)
        {
            // If the player carries an item, un-use it (remove from player's hand)
            if (mCurrentItem != null)
            {
                SetItemActive(mCurrentItem, false);
            }

            InventoryItemBase item = e.Item;

            // Use item (put it to hand of the player)
            SetItemActive(item, true);

            mCurrentItem = e.Item;
        }
    }
    
    public void DropCurrentItem()
    {
        mCanTakeDamage = false;
        
        GameObject goItem = (mCurrentItem as MonoBehaviour).gameObject;

        inventory.RemoveItem(mCurrentItem);

        goItem.transform.position = transform.position;
        goItem.transform.rotation = Quaternion.Euler(0f,0f,90f);
        BoxCollider bc = goItem.AddComponent<BoxCollider>();
        bc.isTrigger = true;
    }

    #endregion
    
    public bool CarriesItem(string itemName)
    {
        if (mCurrentItem == null)
            return false;

        return (mCurrentItem.Name == itemName);
    }

    public InventoryItemBase GetCurrentItem()
    {
        return mCurrentItem;
    }

    public bool IsArmed
    {
        get
        {
            if (mCurrentItem == null)
                return false;

            return mCurrentItem.ItemType == EItemType.Weapon;
        }
    }
    
    public void InteractWithItem()
    {
        if (mInteractItem != null)
        {
            mInteractItem.OnInteract();

            if (mInteractItem is InventoryItemBase)
            {
                InventoryItemBase inventoryItem = mInteractItem as InventoryItemBase;
                inventory.AddItem(inventoryItem);
                inventoryItem.OnPickup();

                if (inventoryItem.UseItemAfterPickup)
                {
                    inventory.UseItem(inventoryItem);
                }
                Hud.CloseMessagePanel();
                mInteractItem = null;
            }
        }
    }

    private void TryInteraction(Collider other)
    {
        InteractableItemBase item = other.GetComponent<InteractableItemBase>();

        if (item != null)
        {
            if (item.CanInteract(other))
            {
                mInteractItem = item;
                Hud.OpenMessagePanel(mInteractItem);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        TryInteraction(other);
    }

    private void OnTriggerStay(Collider other)
    {
        TryInteraction(other);
    }

    private void OnTriggerExit(Collider other)
    {
        InteractableItemBase item = other.GetComponent<InteractableItemBase>();
        if (item != null)
        {
            Hud.CloseMessagePanel();
            mInteractItem = null;
        }
    }
}


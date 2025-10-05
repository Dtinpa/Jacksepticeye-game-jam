using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public PlayerInput playerInput { get; private set; }
    private Rigidbody2D rigidbody;
    private bool inputEnabled = true;

    [SerializeField] private float speed = 5;
    [SerializeField] private float interactRadius = 3.0f;

    private void Awake()
    {
        EventManager.current.EnableInput += EnableInput;
    }

    private void OnDestroy()
    {
        EventManager.current.EnableInput -= EnableInput;
    }

    // Start is called before the first frame update
    void Start()
    {
        playerInput = this.GetComponent<PlayerInput>();
        rigidbody = this.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (inputEnabled)
        {
            Movement();

            // If the player presses the interact button, check to see if they are within range of an NPC
            if (playerInput.actions["Interact"].WasPressedThisFrame())
            {
                CheckIfInRangeOfNPC();
            }
        }
    }

    private void Movement()
    {
        Vector2 input = playerInput.actions["Move"].ReadValue<Vector2>();
        rigidbody.velocity = new Vector2(input.x * speed, 0);
    }

    private void CheckIfInRangeOfNPC()
    {
        Vector2 origin = transform.position;

        // Define the direction of the circle cast (e.g., forward)
        Vector2 direction = transform.right; // Or any other direction like Vector2.up, Vector2.down, etc.

        // Perform the CircleCast
        LayerMask layerMask = LayerMask.GetMask("NPC");
        RaycastHit2D hit = Physics2D.CircleCast(origin, interactRadius, direction, 0.0f, layerMask);

        // Check if anything was hit
        if (hit.collider != null)
        {
            NPCController npc = hit.collider.GetComponent<NPCController>();
            
            // if we can get an NPCController component and we are in range, then tell the Evenetmanager to initiate the Dialogue UI
            if (npc.isInRange)
            {
                EventManager.current.OnActivateDialogue(npc, this.GetComponent<PlayerController>());
                inputEnabled = false;
            } else
            {
                EventManager.current.OnToggleCart();
                inputEnabled = false;
            }
        } else
        {
            EventManager.current.OnToggleCart();
            inputEnabled = false;
        }
    }

    private void EnableInput()
    {
        inputEnabled = true;
    }
}

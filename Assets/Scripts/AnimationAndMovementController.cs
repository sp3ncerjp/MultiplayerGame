using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;

public class AnimationAndMovementController : NetworkBehaviour
{
    PlayerInput playerInput;                        //Component Variables
    CharacterController characterController;
    Animator animator;

    public Sword sword;                             //Sword script object

    Vector2 currentMovementInput;                   //Various tracking variables
    Vector3 currentMovement;
    bool isMovementPressed;
    bool isAttackPressed = false;
    float rotationFactorPerFrame = 10.0f;
    [SerializeField] float playerSpeed = 5.0f;
    [SerializeField] float positionRange = 5.0f;
    public int playerLife = 3;
    public float damageInvunTime = 2.5f;
    float lastHit = -99.0f;

    private void Awake()
    {
        playerInput = new PlayerInput();                                    //Initialize the components
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();


        //Player input callbacks
        playerInput.Player.Move.started += onMovementInput;                 
        playerInput.Player.Move.canceled += onMovementInput;
        playerInput.Player.Move.performed += onMovementInput;

        playerInput.Player.Attack.started += onAttackInput;
        playerInput.Player.Attack.canceled += onAttackInput;
        playerInput.Player.Attack.performed += onAttackInput;
    }

    public override void OnNetworkSpawn()                                   //Override the existing on network spawn function from netcode and have it use our positioning
    {
        UpdatePositionServerRpc();
    }

    void onMovementInput(InputAction.CallbackContext context)               //When movement is performed it is tracked and set accordingly
    {
        currentMovementInput = context.ReadValue<Vector2>();
        currentMovement.x = currentMovementInput.x;
        currentMovement.z = currentMovementInput.y;
        isMovementPressed = currentMovementInput.x != 0 || currentMovementInput.y != 0;     //Set bool for the animation
    }

    void onAttackInput(InputAction.CallbackContext context)                 //While the attack button is pressed set the bool to true for the animation
    {
        isAttackPressed = context.ReadValueAsButton();
    }

    void handleRotation()                                                   //Function to limit the players rotation, make it not snap
    {
        Vector3 positionToLookAt;

        positionToLookAt.x = currentMovement.x;
        positionToLookAt.y = 0.0f;
        positionToLookAt.z = currentMovement.z;

        Quaternion currentRotation = transform.rotation;

        if (isMovementPressed)                  //As long as the player is moving
        {
            Quaternion targetRotation = Quaternion.LookRotation(positionToLookAt);      //Rotate to the position we are looking at by a factor of the time and a rotation limiter
            transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, rotationFactorPerFrame * Time.deltaTime);    //Use slerp to get a quarternion of the desired rotation
        }
    }
    void handleAnimation()
    {
        bool isWalking = animator.GetBool("isWalking");                     //Booleans set to the animator's parameters
        bool isAttacking = animator.GetBool("isAttacking");

        if (isMovementPressed && !isWalking)                                //If you are trying to walk but arent set walk animation to true
        {
            animator.SetBool("isWalking", true);
        }
        else if (!isMovementPressed && isWalking)                           //If you are not trying to walk but you are set the walk animation to false
        {
            animator.SetBool("isWalking", false);
        }

        if (isAttackPressed && !isAttacking)                                //If you are trying to attack but arent set attack animation to true and let the sword script know
        {
            animator.SetBool("isAttacking", true);
            sword.StartAttack();
        }
        else if (!isAttackPressed && isAttacking)                           //If you are not trying to attack but are set attack animation to false and let the sword script know
        {
            animator.SetBool("isAttacking", false);
            sword.EndAttack();
        }
    }

    void handleGravity()                                                    //Simple function to check if the player is on the ground and if not move them down at the rate of gravity
    {
        if (characterController.isGrounded)
        {
            float groundedGravity = -0.5f;
            currentMovement.y = groundedGravity;
        }
        else
        {
            float gravity = -9.8f;
            currentMovement.y += gravity;
        }
    }

    public void takeDamage()                                                //Function for the sword to call and damage the player, not working
    {
        if (Time.time > lastHit + damageInvunTime)                          //Check if the player took damage in the last few seconds or whatever you set it to
        {
            playerLife -= 1;
            if (playerLife <= 0)                                            //If the player's life total reaches 0 from this, set the death animation parameter to true
            {
                animator.SetBool("isDead", true);
            }
            lastHit = Time.time;
        }
    }

    public int getLifeTotal()                                               //Function for the lifeSettings to call and get playerLife, not working
    {
        return playerLife;
    }

    void Start()
    {
        if (IsOwner)                                                        //Have just the host player spawn in the default position
        {
            transform.position = new Vector3(-10, 1, -10);
            transform.rotation = new Quaternion(0, 180, 0, 0);
        }
        handleGravity();                                                    //Make sure they are on the ground
    }

    void Update()
    {
        if (!IsOwner) return;                                               //Update the current players rotation, animation, and movement.
        handleRotation();
        handleAnimation();
        characterController.Move(currentMovement * Time.deltaTime * playerSpeed);
    }

    void OnEnable()                             //Player input subscriptions
    {
        playerInput.Player.Enable();
        playerInput.Player.Move.Enable();
        playerInput.Player.Attack.Enable();
    }

    void OnDisable()                            //Player input unsubscriptors
    {
        playerInput.Player.Disable();
        playerInput.Player.Move.Disable();
        playerInput.Player.Attack.Disable();
    }

    [ServerRpc(RequireOwnership = false)]
    private void UpdatePositionServerRpc()                      //For the clients send an RPC to the server updating their position, also spawn them within a set distance of the default location
    {
        transform.position = new Vector3(-10 + Random.Range(positionRange, -positionRange), 1, -10 + Random.Range(positionRange, -positionRange));
        transform.rotation = new Quaternion(0, 180, 0, 0);
    }
}


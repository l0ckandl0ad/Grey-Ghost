using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GamePlayInput : MonoBehaviour
{
    private Player player;
    private Transform playerTransform;

    private PlayerInput playerInput;
    private Rigidbody2D rigidBody;
    public float Speed { get => parent.Speed; }

    public Vector3 MovementVector { get; private set; } = new Vector3();
    private Vector2 inputVector;

    private IMapEntity parent;
    
    private PauseMenu pauseMenu;
    private MapNavigation mapNavigation;

    private Detection detectionAbility;
    
    private AttackAbility attackAbility;
    public bool IsWaitingForAttackInput { get; private set; }
    private Vector3 target;

    private void Awake()
    {
        player = FindObjectOfType<Player>();
        playerInput = GetComponent<PlayerInput>();

        playerTransform = player.transform;
        parent = player.GetComponent<IMapEntity>();
        rigidBody = player.GetComponent<Rigidbody2D>();
        detectionAbility = player.GetComponent<Detection>();
        attackAbility = player.GetComponent<AttackAbility>();
    }

    private void Start()
    {
        pauseMenu = FindObjectOfType<PauseMenu>();
        mapNavigation = FindObjectOfType<MapNavigation>();
    }

    private void OnPlayerMovement(InputValue value)
    {
        inputVector = value.Get<Vector2>();
        MovementVector = inputVector;
    }

    private void OnFarSearch()
    {
        if (IsPlayerDestroyed()) return;

        detectionAbility.FarSearch();
    }

    private void OnInitiateAttack()
    {
        if (IsPlayerDestroyed()) return;

        if (IsWaitingForAttackInput) return;

        StartCoroutine(TryAttackingOnNextClick());
    }

    private void OnOpenPauseMenu()
    {
        pauseMenu.OpenPauseMenu(playerInput);
    }

    private void OnClosePauseMenu()
    {
        pauseMenu.Resume(true);
    }

    private void OnNavigateMap(InputValue value)
    {
        mapNavigation.ZoomOut(value);
    }
    private void OnMapNavigationMovement(InputValue value)
    {
        mapNavigation.MapNavigationInput(value);
    }
    private void OnCloseMapNavigation()
    {
        mapNavigation.Resume();
    }
    private void OnMapNavigationSpeedBoost(InputValue value)
    {
        mapNavigation.MapNavigationSpeedBoost(value);
    }

    private void Update()
    {
        if (IsPlayerDestroyed()) return;

        playerTransform.Translate(MovementVector * Speed * Time.deltaTime); // speed needs to be used in Update because it changes more often than input
    }


    private IEnumerator TryAttackingOnNextClick()
    {
        IsWaitingForAttackInput = true;

        while (IsWaitingForAttackInput)
        {
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                target = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());

                attackAbility.InitiateAttack(target);

                IsWaitingForAttackInput = false;

                yield break; //stop the coroutine!
            }
            else if (Mouse.current.rightButton.wasPressedThisFrame)
            {
                IsWaitingForAttackInput = false;

                yield break; //stop the coroutine!
            }

            yield return null;
        }
    }

    private bool IsPlayerDestroyed()
    {
        if (playerTransform == null || playerTransform.Equals(null))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

public class MapNavigation : MonoBehaviour
{
    private PlayerInput playerInput;
    private Player player;
    private CinemachineVirtualCamera virtualCamera;

    private EmptyTarget emptyTarget;
    private Transform emptyTargetTransform;
    private Vector3 movementVector = new Vector3();
    private Vector2 inputVector;

    private float speed = 20f; // how fast the map can be moved
    private float speedBoost = 2f; // boost speed by this amount if boost key (shift) is pressed
    private float speedBoostValue = 1f;

    private float normalOrthoSize;
    private float mapOrthoSize = 10f;

    private string gameplayActionMapName = "Main";
    private string mapNavigationActionMapName = "MapNavigation";

    private bool isZoomedOut = false;

    private void Start()
    {
        playerInput = FindObjectOfType<PlayerInput>();
        player = FindObjectOfType<Player>();
        emptyTarget = FindObjectOfType<EmptyTarget>();
        emptyTargetTransform = emptyTarget.transform;
        virtualCamera = GetComponent<CinemachineVirtualCamera>();
        normalOrthoSize = virtualCamera.m_Lens.OrthographicSize;
    }

    public void ZoomOut(InputValue value)
    {
        ZoomedOutEffects();
    }

    private void ZoomedOutEffects()
    {
        isZoomedOut = true;

        virtualCamera.m_Lens.OrthographicSize = mapOrthoSize;
        virtualCamera.Follow = emptyTargetTransform;

        emptyTarget.CenterOnPlayer();

        Time.timeScale = 0f;

        playerInput.SwitchCurrentActionMap(mapNavigationActionMapName);
        
    }

    private void RestoreGameplayState()
    {
        isZoomedOut = false;

        virtualCamera.m_Lens.OrthographicSize = normalOrthoSize;
      
        Time.timeScale = 1f;

        playerInput.SwitchCurrentActionMap(gameplayActionMapName);

        if (player == null || player.Equals(null)) return;
        virtualCamera.Follow = player.transform;

    }

    public void Resume()
    {
        RestoreGameplayState();
    }

    public void MapNavigationInput(InputValue value)
    {
        inputVector = value.Get<Vector2>();
        movementVector = inputVector;
    }

    public void MapNavigationSpeedBoost(InputValue value)
    {
        if (value.isPressed)
        {
            speedBoostValue = speedBoost;
        }
        else
        {
            speedBoostValue = 1f;
        }
    }

    private void Update()
    {
        if (isZoomedOut)
        {
            emptyTargetTransform.position += (movementVector * speed * speedBoostValue * Time.unscaledDeltaTime);
        }
    }

}

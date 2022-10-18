using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class AttackRangeDisplay : MonoBehaviour
{
    [SerializeField]
    private Image image;
    private GameObject imageObject;
    [SerializeField]
    private Color colorMedium;
    [SerializeField]
    private Color colorFar;
    [SerializeField]
    private Color tooFar;

    private float maxAttackRange;
    private float currentRange = 0f;

    private GamePlayInput playerInput;
    private AttackAbility attackAbility;

    private void Awake()
    {
        attackAbility = GetComponent<AttackAbility>();
        playerInput = FindObjectOfType<GamePlayInput>();

        imageObject = image.gameObject;
        imageObject.SetActive(false);

        maxAttackRange = attackAbility.MaxAttackRange;
    }

    private void CalculateRange()
    {
        
        currentRange = Vector2.Distance(gameObject.transform.position, Camera.main.ScreenToWorldPoint(Pointer.current.position.ReadValue()));
    }

    private void ChangeUI()
    {
        if (currentRange <= maxAttackRange / 2)
        {
            image.color = colorMedium;
        }

        if (currentRange <= maxAttackRange && currentRange > maxAttackRange / 2)
        {
            image.color = colorFar;
        }

        if (currentRange > maxAttackRange)
        {
            image.color = tooFar;
        }

        image.fillAmount = currentRange / maxAttackRange;
    }

    private void Update()
    {
        if (!playerInput.IsWaitingForAttackInput)
        {
            imageObject.SetActive(false);
            return;
        }
        else
        {
            imageObject.SetActive(true);
            CalculateRange();
            ChangeUI();
        }
    }

}

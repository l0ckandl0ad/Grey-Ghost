using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpsIcon : MonoBehaviour
{
    private Player player;
    [SerializeField]
    private GameObject opsIcon;

    private void Start()
    {
        player = GetComponent<Player>();
        opsIcon.SetActive(false);
    }

    private void Update()
    {
        ToggleOpsIcon();
    }

    private void ToggleOpsIcon()
    {
        if (player.IsBusyDoingOps)
        {
            opsIcon.SetActive(true);
        }
        else
        {
            opsIcon.SetActive(false);
        }
    }
}

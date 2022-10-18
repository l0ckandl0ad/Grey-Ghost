using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWIP : Carrier
{
    public PlayerWIP()
    {
        IFF = IFF.BLUFOR;

        UnitType = UnitType.NAVAL;
        slowDownTimer = new WaitForSeconds(2.5f); // how long will the carrier slow down for when launching/landing planes

        Speed = 1;
        maxSpeed = 1;

        Health = 100;
        MaxHealth = 100;
        VPCost = 200;
        VPPerDamage = 2;

        MaxAttackers = 2;
        attackersAvailable = 2;
        MaxAmmo = 8;
        ammo = 8;
    }
}

using UnityEngine;

public class IconDirectionForPlayer : IconDirectionForUnitBase
{
    protected GamePlayInput gamePlayInput;

    protected override void CacheReferences()
    {
        base.CacheReferences();
        gamePlayInput = FindObjectOfType<GamePlayInput>();
    }

    protected override Vector3 CheckMovementVector()
    {
        return gamePlayInput.MovementVector;
    }

}

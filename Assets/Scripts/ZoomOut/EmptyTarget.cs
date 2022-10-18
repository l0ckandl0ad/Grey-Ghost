using UnityEngine;
/// <summary>
/// Needed for navigation.
/// </summary>
public class EmptyTarget : MonoBehaviour
{
    private Player player;

    private void Start()
    {
        player = FindObjectOfType<Player>();
    }

    public void CenterOnPlayer()
    {
        if (player == null || player.Equals(null))
        {
            transform.position = Vector3.zero;
        }
        else
        {
            transform.position = player.transform.position;
        }
    }


}

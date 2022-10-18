using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// This class allows to display UI that indicates when player's avatar is spotted by the enemy.
/// </summary>
public class DetectedIcon : MonoBehaviour
{
    private FogOfWar fogOfWar;
    private bool isCoroutineRunning;
    private WaitForSeconds detectionDecay = new WaitForSeconds(2f); // how long will the entity stay visible upon being detected

    [SerializeField]
    private GameObject detectedIcon;

    private void Awake()
    {
        fogOfWar = GetComponent<FogOfWar>();
        detectedIcon.SetActive(false);
    }


    private void Update()
    {
        if (fogOfWar.IsDetectedByEnemy && !isCoroutineRunning)
        {
            StartCoroutine(ShowDetectedIcon());
        }
    }

    private IEnumerator ShowDetectedIcon()
    {
        isCoroutineRunning = true;
        detectedIcon.SetActive(true);

        yield return detectionDecay;
        
        detectedIcon.SetActive(false);
        
        isCoroutineRunning = false;
    }
}

using UnityEngine;
using TMPro;
using System.Collections;

public class FloatingText : MonoBehaviour
{
    private TextMeshPro text;
    /// <summary>
    /// How long will this object exist after being instantiated.
    /// </summary>
    private WaitForSeconds expiration = new WaitForSeconds(1.5f);
    /// <summary>
    /// Vector of movement for each frame.
    /// </summary>
    private Vector3 movementVector;
    private float movementSpeed = 100f;

    private bool isInitialized = false;

    private void Awake()
    {
        text = GetComponentInChildren<TextMeshPro>();
    }

    public void Initialize(string text, Color color)
    {
        this.text.color = color;
        this.text.SetText(text);
        StartCoroutine(SelfDestruct());
        isInitialized = true;
    }

    private void Start()
    {
        RandomStartingPosition();
        RandomMovementVector();
    }

    private void RandomStartingPosition()
    {
        Vector3 offset = new Vector3(0, 0, 0);
        offset.x = Random.Range(-0.2f, 0.2f);
        offset.y = Random.Range(-0.2f, 0.2f);
        transform.position += offset;
    }
    private void RandomMovementVector()
    {
        movementVector = new Vector3(0f, 0.0002f, 0f);
        movementVector.x = Random.Range(-0.0005f, 0.0005f);
    }

    private void Move()
    {
        if (!isInitialized) return;

        transform.Translate(movementVector * Time.deltaTime * movementSpeed);
    }
    private void Update()
    {
        Move();
    }

    private IEnumerator SelfDestruct()
    {
        yield return expiration;
        Destroy(gameObject);
    }

}

using UnityEngine;

public class Rotate_Z : MonoBehaviour
{
    [Header("Rotation Speed Range")]
    [Tooltip("The minimum possible rotation speed in degrees per second.")]
    public float minRotationSpeed = 50f;

    [Tooltip("The maximum possible rotation speed in degrees per second.")]
    public float maxRotationSpeed = 200f;

    
    private float actualRotationSpeed;

    void Start()
    {
        
        actualRotationSpeed = Random.Range(minRotationSpeed, maxRotationSpeed);

        
        if (Random.value > 0.5f)
        {
            actualRotationSpeed *= -1f;
        }
    }

    void Update()
    {
       
        transform.Rotate(0f, 0f, actualRotationSpeed * Time.deltaTime);
    }
}
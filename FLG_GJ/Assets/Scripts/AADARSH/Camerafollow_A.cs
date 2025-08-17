using UnityEngine;

public class Camerafollow_A : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Transform playerTransform;
    //[SerializeField]float smoothSpeed = 0;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void LateUpdate() {
        Vector3 targetPos = new Vector3(playerTransform.position.x,transform.position.y,transform.position.z);
        
        //Vector3 smoothedPosition = Vector3.Lerp(transform.position, targetPos, smoothSpeed);
        transform.position = targetPos;
    }
}

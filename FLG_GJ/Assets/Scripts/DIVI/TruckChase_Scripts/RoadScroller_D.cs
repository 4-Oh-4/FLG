using UnityEngine;

namespace TruckChase
{
    public class RoadScroller_D : MonoBehaviour
    {
        [SerializeField] private float scrollSpeed = 8f;
        private float roadHeight;

        void Start()
        {
            roadHeight = GetComponent<SpriteRenderer>().bounds.size.y;
        }

        void Update()
        {
            transform.position += Vector3.down * scrollSpeed * Time.deltaTime;
            if (transform.position.y < -roadHeight)
            {
                transform.position += Vector3.up * roadHeight * 2f;
            }
        }
    }
}
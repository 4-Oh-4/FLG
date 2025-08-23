using UnityEngine;

namespace TruckChase
{
    public class RoadScroller : MonoBehaviour
    {
        [SerializeField] private float scrollSpeed = 8f;
        private float roadHeight;

        void Start()
        {
            // Gets the vertical size of the road sprite.
            roadHeight = GetComponent<SpriteRenderer>().bounds.size.y;
        }

        void Update()
        {
            // Move the road segment downwards.
            transform.position += Vector3.down * scrollSpeed * Time.deltaTime;

            // If the road has moved completely off the bottom of the screen...
            if (transform.position.y < -roadHeight)
            {
                // ...teleport it back to the top by moving it up by two full road heights.
                transform.position += Vector3.up * roadHeight * 2f;
            }
        }
    }
}
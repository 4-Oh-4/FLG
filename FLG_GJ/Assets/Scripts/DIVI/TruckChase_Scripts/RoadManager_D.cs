using UnityEngine;

namespace TruckChase
{
    public class RoadManager_D : MonoBehaviour
    {
        [SerializeField] private float scrollSpeed = 8f;
        [SerializeField] private Transform[] roadSegments; 
        [SerializeField] private float pixelOverlap = 0.01f; 

        private float roadHeight;

        private void Start()
        {
            if (roadSegments.Length > 0)
            {
                // Height of one road sprite
                roadHeight = roadSegments[0].GetComponent<SpriteRenderer>().bounds.size.y;
            }
        }

        private void Update()
        {
            foreach (Transform segment in roadSegments)
            {
                // Move road downward
                segment.position += Vector3.down * scrollSpeed * Time.deltaTime;

                // If segment went fully below screen
                if (segment.position.y < -roadHeight)
                {
                    // Find the highest Y among other road segments
                    float highestY = float.MinValue;
                    foreach (Transform seg in roadSegments)
                    {
                        if (seg != segment && seg.position.y > highestY)
                            highestY = seg.position.y;
                    }

                    // Snap the road exactly above, with slight overlap to prevent seams
                    segment.position = new Vector3(
                        segment.position.x,
                        highestY + roadHeight - pixelOverlap,
                        segment.position.z
                    );
                }
            }
        }
    }
}

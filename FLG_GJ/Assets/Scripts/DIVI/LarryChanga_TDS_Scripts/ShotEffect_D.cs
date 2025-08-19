using UnityEngine;

namespace TopDownShooter
{
    // This script requires a LineRenderer component to be attached.
    [RequireComponent(typeof(LineRenderer))]
    public class ShotEffect_D : MonoBehaviour
    {
        private LineRenderer lineRenderer;

        private void Awake()
        {
            // Get the LineRenderer component attached to this object.
            lineRenderer = GetComponent<LineRenderer>();
        }

        /// <summary>
        /// Sets the start and end points of the line.
        /// </summary>
        public void SetPoints(Vector3 start, Vector3 end)
        {
            lineRenderer.SetPosition(0, start);
            lineRenderer.SetPosition(1, end);
        }

        private void Start()
        {
            // Destroy this effect object after a very short time.
            Destroy(gameObject, 0.07f);
        }
    }
}
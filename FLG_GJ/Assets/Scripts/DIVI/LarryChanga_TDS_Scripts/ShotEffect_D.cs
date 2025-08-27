using UnityEngine;

namespace TopDownShooter
{
    [RequireComponent(typeof(LineRenderer))]
    public class ShotEffect_D : MonoBehaviour
    {
        private LineRenderer lineRenderer;

        private void Awake()
        {
            lineRenderer = GetComponent<LineRenderer>();
        }

        // OnEnable is called every time a pooled object is activated.
        private void OnEnable()
        {
            // Schedule this object to deactivate itself after a very short time.
            Invoke(nameof(Deactivate), 0.07f);
        }

        public void SetPoints(Vector3 start, Vector3 end)
        {
            lineRenderer.SetPosition(0, start);
            lineRenderer.SetPosition(1, end);
        }

        // AMENDED: This now deactivates the object instead of destroying it.
        void Deactivate()
        {
            gameObject.SetActive(false);
        }
    }
}
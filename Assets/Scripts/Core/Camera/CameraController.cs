using UnityEngine;

namespace EcoVillage.Core.Camera
{
    public class CameraController : MonoBehaviour
    {
        [Header("Target Tracking")]
        [Tooltip("The target GameObject the camera will follow (e.g. the player).")]
        [SerializeField] private Transform target;

        [Header("2D Follow Offset")]
        [Tooltip("Offset distance from the target in 2D space.")]
        [SerializeField] private Vector2 offset = Vector2.zero;

        [Header("Smooth Options")]
        [Tooltip("Damping factor for camera follow movement delay (higher = slower follow).")]
        [Range(0f, 1f)]
        [SerializeField] private float smoothTime = 0.2f;

        [Header("Camera Bounds")]
        [Tooltip("Enable to clamp the camera within map boundaries.")]
        [SerializeField] private bool useBounds = false;

        [Tooltip("Assign a Collider2D (e.g., BoxCollider2D) that defines the map boundaries.")]
        [SerializeField] private Collider2D boundsCollider;

        [Tooltip("Alternatively, set custom min and max bounds manually if no collider is used.")]
        [SerializeField] private Vector2 minBounds;
        [SerializeField] private Vector2 maxBounds;

        private UnityEngine.Camera cam;
        private Vector2 currentVelocity = Vector2.zero;
        private float originalZ;

        private void Start()
        {
            cam = GetComponent<UnityEngine.Camera>();

            // Keep the original Z position of the camera (crucial in 2D)
            originalZ = transform.position.z;

            // Automatically attempt to find the player if target is not manually assigned
            if (target == null)
            {
                GameObject player = GameObject.FindWithTag("Player");
                if (player != null)
                {
                    target = player.transform;
                }
            }

            // If a collider is provided, calculate min/max bounds from it
            if (useBounds && boundsCollider != null)
            {
                minBounds = boundsCollider.bounds.min;
                maxBounds = boundsCollider.bounds.max;
            }
        }

        private void LateUpdate()
        {
            if (target == null) return;

            // Calculate the desired 2D position
            Vector2 target2DPosition = (Vector2)target.position + offset;

            // Smoothly move the camera on X and Y axes
            Vector2 smoothed2DPosition = Vector2.SmoothDamp(transform.position, target2DPosition, ref currentVelocity, smoothTime);

            // Clamp camera position within bounds if enabled
            if (useBounds && cam != null)
            {
                float camHalfHeight = cam.orthographicSize;
                float camHalfWidth = cam.orthographicSize * cam.aspect;

                float clampedX = Mathf.Clamp(smoothed2DPosition.x, minBounds.x + camHalfWidth, maxBounds.x - camHalfWidth);
                float clampedY = Mathf.Clamp(smoothed2DPosition.y, minBounds.y + camHalfHeight, maxBounds.y - camHalfHeight);

                smoothed2DPosition = new Vector2(clampedX, clampedY);
            }

            // Reconstruct the 3D position keeping the original Z coordinate
            transform.position = new Vector3(smoothed2DPosition.x, smoothed2DPosition.y, originalZ);
        }

        /// <summary>
        /// Updates the target dynamically if needed.
        /// </summary>
        public void SetTarget(Transform newTarget)
        {
            target = newTarget;
        }
    }
}

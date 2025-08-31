using System.Collections;
using UnityEngine;

/// <summary>
/// Defines the type of animator parameter to use for an animation.
/// </summary>
public enum AnimationParameterType { Trigger, Bool }

/// <summary>
/// This script pans the camera between two target GameObjects at the start of the scene.
/// It is suitable for both 3D and 2D games. For 2D, it can also animate the camera's orthographic size (zoom).
/// It can also trigger animations on the targets during the sequence.
/// </summary>
public class CameraPanController : MonoBehaviour {
    [Header("Targets")]
    [Tooltip("The first GameObject the camera will focus on. The camera will only use its X and Y coordinates.")]
    public Transform target1;

    [Tooltip("The second GameObject the camera will pan to. The camera will only use its X and Y coordinates.")]
    public Transform target2;

    [Header("Camera Settings")]
    [Tooltip("The transform of the camera to be moved. If not set, it will default to the main camera.")]
    public Transform cameraTransform;

    [Header("Animation Settings")]
    [Tooltip("The time in seconds it takes for the camera to move from target 1 to target 2.")]
    public float panDuration = 3.0f;

    [Tooltip("The delay in seconds before the panning animation starts.")]
    public float startDelay = 1.0f;

    [Header("2D Zoom Settings")]
    [Tooltip("Check this to animate the camera's orthographic size (zoom) during the pan.")]
    public bool animateZoom = false;

    [Tooltip("The starting orthographic size of the camera. Only used if Animate Zoom is checked.")]
    public float startOrthographicSize = 5f;

    [Tooltip("The ending orthographic size of the camera. Only used if Animate Zoom is checked.")]
    public float endOrthographicSize = 5f;

    [Header("Target 1 Animation")]
    [Tooltip("Animator for the first target. If not assigned, the script will try to find it on the target.")]
    public Animator target1Animator;
    [Tooltip("The type of parameter to use for Target 1's animation.")]
    public AnimationParameterType target1AnimationType = AnimationParameterType.Trigger;
    [Tooltip("The name of the animation boolean or trigger for Target 1 (e.g., 'isShooting' or 'Shoot').")]
    public string target1AnimationName = "Shoot";
    // ADDED: New field for the state name
    [Tooltip("The name of the animation STATE to wait for. Only used if Type is Bool. Must match the name in the Animator Controller.")]
    public string target1AnimationStateName = "Shooting_Animation";
    [Tooltip("Delay in seconds from the start of the pan before playing Target 1's animation.")]
    public float target1AnimationDelay = 1.5f;

    [Header("Target 2 Animation")]
    [Tooltip("Animator for the second target. If not assigned, the script will try to find it on the target.")]
    public Animator target2Animator;
    [Tooltip("The type of parameter to use for Target 2's animation.")]
    public AnimationParameterType target2AnimationType = AnimationParameterType.Trigger;
    [Tooltip("The name of the animation boolean or trigger for Target 2 (e.g., 'isHit' or 'GetHit').")]
    public string target2AnimationName = "GetHit";
    // ADDED: New field for the state name
    [Tooltip("The name of the animation STATE to wait for. Only used if Type is Bool. Must match the name in the Animator Controller.")]
    public string target2AnimationStateName = "GetHit_Animation";
    [Tooltip("Delay in seconds from the start of the pan before playing Target 2's animation.")]
    public float target2AnimationDelay = 2.0f;

    // REMOVED: The old bool duration fields are no longer necessary.
    // public float target1BoolDuration = 1.0f;
    // public float target2BoolDuration = 1.0f;

    [SerializeField] bool target1anim = false;
    [SerializeField] bool target2anim = false;
    // Private reference to the camera component for zoom functionality
    private Camera cam;

    /// <summary>
    /// This method is called before the first frame update.
    /// </summary>
    void Start() {
        // If the camera transform is not assigned in the inspector, find the main camera.
        if (cameraTransform == null) {
            cameraTransform = Camera.main.transform;
            if (cameraTransform == null) {
                Debug.LogError("Main Camera not found. Please assign a camera to the Camera Transform field.");
                return; // Stop execution if no camera is found
            }
        }

        // Get the camera component for zoom functionality
        cam = cameraTransform.GetComponent<Camera>();

        // If zoom is enabled, ensure the camera is orthographic.
        if (animateZoom) {
            if (cam == null) {
                Debug.LogError("Cannot animate zoom because no Camera component was found on the camera object.", cameraTransform);
                animateZoom = false; // Disable zoom if there's no camera component
            } else if (!cam.orthographic) {
                Debug.LogWarning("'Animate Zoom' is enabled, but the camera is not Orthographic. Zoom animation will be disabled.", cam);
                animateZoom = false; // Disable zoom if camera is not orthographic
            }
        }

        // Check if both targets have been assigned in the inspector.
        if (target1 == null || target2 == null) {
            Debug.LogError("One or both targets are not assigned. Please assign both Target 1 and Target 2 in the inspector.");
            return; // Stop execution if targets are missing
        }

        // If animators aren't assigned, try to find them on the targets automatically.
        if (target1Animator == null) target1Animator = target1.GetComponent<Animator>();
        if (target2Animator == null) target2Animator = target2.GetComponent<Animator>();


        // Set the initial position and rotation, preserving the camera's original Z-axis.
        cameraTransform.position = new Vector3(target1.position.x, target1.position.y, cameraTransform.position.z);
        cameraTransform.rotation = target1.rotation;
        if (animateZoom && cam != null) {
            cam.orthographicSize = startOrthographicSize;
        }

        // Start the coroutine that handles the panning animation.
        StartCoroutine(PanCamera());
    }

    /// <summary>
    /// A coroutine that handles the smooth panning of the camera from target1 to target2.
    /// </summary>
    /// <returns>An IEnumerator for the coroutine.</returns>
    private IEnumerator PanCamera() {
        // Wait for the specified start delay before beginning the pan.
        yield return new WaitForSeconds(startDelay);

        // Start the animation coroutine at the same time as the pan
        StartCoroutine(TriggerAnimations());

        float elapsedTime = 0f;
        Vector3 startingPos = cameraTransform.position;
        Quaternion startingRot = cameraTransform.rotation;

        // Loop until the elapsed time is greater than the pan duration.
        while (elapsedTime < panDuration) {
            // Calculate the interpolation factor (from 0 to 1).
            float t = elapsedTime / panDuration;

            // Smoothstep provides a nice ease-in and ease-out effect for motion.
            t = t * t * (3f - 2f * t);

            // Interpolate the camera's X and Y position, but keep the Z-axis constant.
            Vector3 targetPosition = new Vector3(target2.position.x, target2.position.y, startingPos.z);
            cameraTransform.position = Vector3.Lerp(startingPos, targetPosition, t);

            // Interpolate the camera's rotation.
            cameraTransform.rotation = Quaternion.Slerp(startingRot, target2.rotation, t);

            // If zooming is enabled, also interpolate the orthographic size.
            if (animateZoom && cam != null) {
                cam.orthographicSize = Mathf.Lerp(startOrthographicSize, endOrthographicSize, t);
            }

            // Increment the elapsed time by the time since the last frame.
            elapsedTime += Time.deltaTime;

            // Wait for the next frame before continuing the loop.
            yield return null;
        }

        // After the loop, snap the camera exactly to the final target's state
        // to ensure it ends up in the perfect spot, maintaining the Z-axis.
        cameraTransform.position = new Vector3(target2.position.x, target2.position.y, startingPos.z);
        cameraTransform.rotation = target2.rotation;
        if (animateZoom && cam != null) {
            cam.orthographicSize = endOrthographicSize;
        }
    }

    /// <summary>
    /// A coroutine that triggers the animations on the targets with specified delays relative to the pan start.
    /// </summary>
    private IEnumerator TriggerAnimations() {
        // Determine which animation is scheduled to happen first.
        bool target1First = target1AnimationDelay <= target2AnimationDelay;

        if (target1First) {
            // Wait until it's time for the first animation.
            if (target1AnimationDelay > 0)
                yield return new WaitForSeconds(target1AnimationDelay);
            // MODIFIED: Pass the state name to the PlayAnimation method
            PlayAnimation(target1Animator, target1AnimationType, target1AnimationName, target1AnimationStateName);

            // Now, wait the remaining time until the second animation.
            float waitTime = target2AnimationDelay - target1AnimationDelay;
            if (waitTime > 0)
                yield return new WaitForSeconds(waitTime);
            // MODIFIED: Pass the state name to the PlayAnimation method
            PlayAnimation(target2Animator, target2AnimationType, target2AnimationName, target2AnimationStateName);
        } else // Target 2's animation is scheduled to play first.
          {
            // Wait until it's time for the second animation.
            if (target2AnimationDelay > 0)
                yield return new WaitForSeconds(target2AnimationDelay);
            // MODIFIED: Pass the state name to the PlayAnimation method
            PlayAnimation(target2Animator, target2AnimationType, target2AnimationName, target2AnimationStateName);

            // Now, wait the remaining time until the first animation.
            float waitTime = target1AnimationDelay - target2AnimationDelay;
            if (waitTime > 0)
                yield return new WaitForSeconds(waitTime);
            // MODIFIED: Pass the state name to the PlayAnimation method
            PlayAnimation(target1Animator, target1AnimationType, target1AnimationName, target1AnimationStateName);
        }
    }

    /// <summary>
    /// A helper method to play an animation on a target animator.
    /// </summary>
    // MODIFIED: Method signature changed to accept the state name
    private void PlayAnimation(Animator animator, AnimationParameterType type, string paramName, string stateName) {
        if (animator != null && !string.IsNullOrEmpty(paramName)) {
            if (type == AnimationParameterType.Bool) {
                // MODIFIED: Pass the state name to the coroutine
                StartCoroutine(HandleBoolAnimation(animator, paramName, stateName));
            } else {
                animator.SetTrigger(paramName);
            }
        }
    }

    /// <summary>
    /// Handles setting a boolean parameter to true, waiting for the animation to finish, then setting it back to false.
    /// </summary>
    // MODIFIED: Complete rewrite of this coroutine.
    private IEnumerator HandleBoolAnimation(Animator animator, string boolName, string stateName) {
        if (animator == null || string.IsNullOrEmpty(stateName)) yield break;

        // Set the bool to true to trigger the animation
        animator.SetBool(boolName, true);

        // Wait a frame to ensure the animator has transitioned to the new state
        yield return null;

        // Wait until the animator is in the specified state and the animation has finished playing
        // We check normalizedTime >= 1 which means one full playthrough of the animation clip.
        // We also check that we are still in the correct state in case another transition interrupted it.
        while (animator.GetCurrentAnimatorStateInfo(0).IsName(stateName) &&
               animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f) {
            // Wait for the next frame
            yield return null;
        }

        // Now that the animation is done, set the bool back to false
        animator.SetBool(boolName, false);
    }

    private void Update() {
        if (target1Animator != null && !string.IsNullOrEmpty(target1AnimationName))
            target1anim = target1Animator.GetBool(target1AnimationName);

        if (target2Animator != null && !string.IsNullOrEmpty(target2AnimationName))
            target2anim = target2Animator.GetBool(target2AnimationName);
    }
}
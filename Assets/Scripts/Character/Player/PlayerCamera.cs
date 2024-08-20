using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public static PlayerCamera instance;
    public PlayerManager player;
    public Camera cameraObject;
    [SerializeField] Transform cameraPivotTransform;

    // Change these to tweak camera performance
    [Header("Camera Settings")]
    [SerializeField] private float cameraSmoothSpeed = 10; // The bigger this number, the longer for the camera to reach its position during movement
    [SerializeField] float leftAndRightRotationSpeed = 220;
    [SerializeField] float upAndDownRotationSpeed = 220;
    [SerializeField] float minimumPivot = -30; // lowest point you are able to look down
    [SerializeField] float maximumPivot = 60; // highest point you are able to look up
    [SerializeField] float cameraCollisionRadius = 0.2f;
    [SerializeField] LayerMask collideWithLayers;

    [Header("Camera Values")]
    private Vector3 cameraVelocity;
    private Vector3 cameraObjectPosition; // used for camera collisions (moves the camera object to this position up colliding)
    [SerializeField] float leftAndRightLookAngle;
    [SerializeField] float upAndDownLookAngle;
    private float cameraZPosition; // value used for camera collisions
    private float targetCameraZPosition;// value used for camera collisions

    [Header("Lock-On")]
    [SerializeField] float lockOnRadius = 20;
    [SerializeField] float minimumViewableAngle = -50;
    [SerializeField] float maximumViewableAngle = 50;
    [SerializeField] float lockOnTargetFollowSpeed = 0.2f;
    [SerializeField] float setCameraHeightSpeed = 0.01f;
    [SerializeField] float unlockedCameraHeight = 1.65f;
    [SerializeField] float lockedCameraHeight = 2.0f;
    private Coroutine cameraLockOnHeightCoroutine;
    private List<CharacterManager> availableTargets = new List<CharacterManager>();
    public CharacterManager nearestLockOnTarget;
    public CharacterManager leftLockOnTarget;
    public CharacterManager rightLockOnTarget;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        cameraZPosition = cameraObject.transform.localPosition.z;
    }

    public void HandleAllCameraActions()
    {
        if (player != null)
        {
            HandleFollowTarget();
            HandleRotation();
            HandleCollisions();
        }

    }

    private void HandleFollowTarget()
    {
        Vector3 targetCameraPosition = Vector3.SmoothDamp(transform.position, player.transform.position, ref cameraVelocity,
            cameraSmoothSpeed * Time.deltaTime);
        transform.position = targetCameraPosition;
    }

    private void HandleRotation()
    {
        // if locked on, force rotation towards target
        if (player.playerNetworkManager.isLockedOn.Value)
        {
            // rotate gameobject
            Vector3 rotationDirection = player.playerCombatManager.currentTarget.characterCombatManager.lockOnTransform.position - transform.position;
            rotationDirection.Normalize();
            rotationDirection.y = 0;

            Quaternion targetRotation = Quaternion.LookRotation(rotationDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, lockOnTargetFollowSpeed);

            // rotate pivot object
            rotationDirection = player.playerCombatManager.currentTarget.characterCombatManager.lockOnTransform.position - cameraPivotTransform.position;
            rotationDirection.Normalize();

            targetRotation = Quaternion.LookRotation(rotationDirection);
            cameraPivotTransform.transform.rotation = Quaternion.Slerp(cameraPivotTransform.rotation, targetRotation, lockOnTargetFollowSpeed);

            // save rotation value for smooth unlock
            leftAndRightLookAngle = transform.eulerAngles.y;
            upAndDownLookAngle = transform.eulerAngles.x;
        }
        else
        {
            // Rotate left and right based on horizontal movement on the right joystick
            leftAndRightLookAngle += (PlayerInputManager.instance.cameraHorizontalInput * leftAndRightRotationSpeed) * Time.deltaTime;
            // Rotate up and down based on horizontal movement on the right joystick
            upAndDownLookAngle -= (PlayerInputManager.instance.cameraVerticalInput * upAndDownRotationSpeed) * Time.deltaTime;
            // Clamp the up and down look angle between a min and max value
            upAndDownLookAngle = Mathf.Clamp(upAndDownLookAngle, minimumPivot, maximumPivot);

            Vector3 cameraRotation = Vector3.zero;
            Quaternion targetRotation;

            // Left and right
            cameraRotation.y = leftAndRightLookAngle;
            targetRotation = Quaternion.Euler(cameraRotation);
            transform.rotation = targetRotation;

            // Up and down
            cameraRotation = Vector3.zero;
            cameraRotation.x = upAndDownLookAngle;
            targetRotation = Quaternion.Euler(cameraRotation);
            cameraPivotTransform.localRotation = targetRotation;
        }
    }

    private void HandleCollisions()
    {
        targetCameraZPosition = cameraZPosition;
        
        RaycastHit hit;
        // Direction for collision check
        Vector3 direction = cameraObject.transform.position - cameraPivotTransform.position;
        direction.Normalize();

        // We check if there is an object in front of our desired direction
        if (Physics.SphereCast(cameraPivotTransform.position, cameraCollisionRadius, direction, out hit,
            Mathf.Abs(targetCameraZPosition), collideWithLayers))
        {
            // If there is, we get our distance from it
            float distanceFromHitObject = Vector3.Distance(cameraPivotTransform.position, hit.point);
            // We then equate out target z position to the following
            targetCameraZPosition = -(distanceFromHitObject - cameraCollisionRadius);
        }

        // If our target position is less than our collision radius, we subtract our collision radius (making it snap back)
        if (Mathf.Abs(targetCameraZPosition) < cameraCollisionRadius)
        {
            targetCameraZPosition = -cameraCollisionRadius;
        }

        // We then apply our final position using a lerp over a time of 0.2f
        cameraObjectPosition.z = Mathf.Lerp(cameraObject.transform.localPosition.z, targetCameraZPosition, 0.2f);
        cameraObject.transform.localPosition = cameraObjectPosition;
    }

    public void HandleLocatingLockOnTargets()
    {
        float shortestDistance = Mathf.Infinity; // to determine closest target
        float shortestDistanceOfRightTarget = Mathf.Infinity; // to determine closest target on one axis to the right
        float shortestDistanceOfLeftTarget = -Mathf.Infinity; // to determine closest target on one axis to the left

        Collider[] colliders = Physics.OverlapSphere(player.transform.position, lockOnRadius,
            WorldUtilityManager.instance.GetCharacterLayers());

        for (int i = 0; i < colliders.Length; i++)
        {
            CharacterManager lockOnTarget = colliders[i].GetComponent<CharacterManager>();

            if (lockOnTarget != null)
            {
                Vector3 lockOnTargetDirection = lockOnTarget.transform.position - player.transform.position;
                float distanceFromTarget = Vector3.Distance(player.transform.position, lockOnTargetDirection);
                float viewableAngle = Vector3.Angle(lockOnTargetDirection, cameraObject.transform.forward);

                // if target is dead, check next potential target
                if (lockOnTarget.isDead.Value)
                {
                    continue;
                }

                // if target is ourself, check next
                if (lockOnTarget.transform.root == player.transform.root)
                {
                    continue;
                }

                // if target is outside field of view
                if (viewableAngle < maximumViewableAngle && viewableAngle > minimumViewableAngle)
                {
                    RaycastHit hit;

                    // if target is blocked by environment layer
                    if (Physics.Linecast(player.playerCombatManager.lockOnTransform.position,
                        lockOnTarget.characterCombatManager.lockOnTransform.position, out hit,
                        WorldUtilityManager.instance.GetEnvironmentLayers()))
                    {
                        continue;
                    }
                    else
                    {
                        availableTargets.Add(lockOnTarget);
                    }
                }
            }
        }

        // sort to know which one we lock onto first
        for (int k = 0; k < availableTargets.Count; k++)
        {
            if (availableTargets[k] != null)
            {
                float distanceFromTarget = Vector3.Distance(player.transform.position, availableTargets[k].transform.position);

                if (distanceFromTarget < shortestDistance)
                {
                    shortestDistance = distanceFromTarget;
                    nearestLockOnTarget = availableTargets[k];
                }

                if (player.playerNetworkManager.isLockedOn.Value)
                {
                    Vector3 relativeEnemyPosition = player.transform.InverseTransformPoint(availableTargets[k].transform.position);
                    var distanceFromLeftTarget = relativeEnemyPosition.x;
                    var distanceFromRightTarget = relativeEnemyPosition.x;

                    if (availableTargets[k] == player.playerCombatManager.currentTarget)
                    {
                        continue;
                    }

                    // check left side for targets
                    if (relativeEnemyPosition.x <= 0 && distanceFromLeftTarget > shortestDistanceOfLeftTarget)
                    {
                        shortestDistanceOfLeftTarget = distanceFromLeftTarget;
                        leftLockOnTarget = availableTargets[k];
                    } // check right side for targets
                    else if (relativeEnemyPosition.x >= 0 && distanceFromRightTarget < shortestDistanceOfRightTarget)
                    {
                        shortestDistanceOfRightTarget = distanceFromRightTarget;
                        rightLockOnTarget = availableTargets[k];
                    }
                }
            }
            else
            {
                ClearLockedOnTargets();
                player.playerNetworkManager.isLockedOn.Value = false;
            }
        }
    }

    public void SetLockCameraHeight()
    {
        if (cameraLockOnHeightCoroutine != null)
        {
            StopCoroutine(cameraLockOnHeightCoroutine);
        }

        cameraLockOnHeightCoroutine = StartCoroutine(SetCameraHeight());
    }

    public void ClearLockedOnTargets()
    {
        nearestLockOnTarget = null;
        leftLockOnTarget = null;
        rightLockOnTarget = null;
        availableTargets.Clear();
    }

    public IEnumerator WaitThenFindNewTarget()
    {
        while (player.isPerformingAction)
        {
            yield return null;
        }

        ClearLockedOnTargets();
        HandleLocatingLockOnTargets();

        if (nearestLockOnTarget != null)
        {
            player.playerCombatManager.SetTarget(nearestLockOnTarget);
            player.playerNetworkManager.isLockedOn.Value = true;
        }

        yield return null;
    }

    private IEnumerator SetCameraHeight()
    {
        float duration = 1;
        float timer = 0;

        Vector3 velocity = Vector3.zero;
        Vector3 newLockedCameraHeight = new Vector3(cameraPivotTransform.transform.localPosition.x, lockedCameraHeight);
        Vector3 newUnlockedCameraHeight = new Vector3(cameraPivotTransform.transform.localPosition.x, unlockedCameraHeight);

        while (timer < duration)
        {
            timer += Time.deltaTime;

            if (player != null)
            {
                if (player.playerCombatManager.currentTarget != null)
                {
                    cameraPivotTransform.transform.localPosition = Vector3.SmoothDamp(cameraPivotTransform.transform.localPosition, newLockedCameraHeight, ref velocity, setCameraHeightSpeed);
                    cameraPivotTransform.transform.localRotation = Quaternion.Slerp(cameraPivotTransform.transform.localRotation, Quaternion.Euler(0, 0, 0), lockOnTargetFollowSpeed);
                }
                else
                {
                    cameraPivotTransform.transform.localPosition = Vector3.SmoothDamp(cameraPivotTransform.transform.localPosition, newUnlockedCameraHeight, ref velocity, setCameraHeightSpeed);
                    //cameraPivotTransform.transform.localRotation = Quaternion.Slerp(cameraPivotTransform.transform.localRotation, Quaternion.Euler(0, 0, 0), lockOnTargetFollowSpeed);
                }
            }

            yield return null;
        }

        if (player != null)
        {
            if (player.playerCombatManager.currentTarget != null)
            {
                cameraPivotTransform.transform.localPosition = newLockedCameraHeight;
                cameraPivotTransform.transform.localRotation = Quaternion.Euler(0, 0, 0);
            }
            else
            {
                cameraPivotTransform.transform.localPosition = newUnlockedCameraHeight;
            }
        }

        yield return null;
    }
}

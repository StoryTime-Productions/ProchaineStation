using UnityEngine;

public class ItemGrabber : MonoBehaviour
{
    public float interactionRange = 3f; // Maximum range for interacting with items
    public float interactionRadius = 2.2f; // Maximum radius to interact with items around the player
    public Transform itemHoldPosition; // Reference to the position where the item will be held
    public Transform inspectionPosition; // Reference to the position for inspecting the held item

    public float swaySpeed = 1.5f; // Speed of item swaying
    public float swayAmount = 0.02f; // Amount of swaying motion
    public float dropForwardOffset = 1f; // Offset for dropping the item forward
    public float throwLeftOffset = 0.2f; // Offset for throwing the item to the left
    public float throwUpOffset = 0.1f; // Offset for throwing the item upward
    public float throwForce = 10f; // Force applied to the thrown item

    private Rigidbody heldItem; // Reference to the currently held item
    private bool isInspecting = false; // Flag to indicate if the player is in inspection mode
    public PlayerControllerScript playerController; // Reference to the player controller script handling the mouse look

    void Update()
    {
        // Check for "E" key press to pick up or throw the item
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (heldItem == null)
            {
                Collider[] colliders = Physics.OverlapSphere(transform.position, interactionRadius);
                foreach (Collider collider in colliders)
                {
                    if (Vector3.Distance(transform.position, collider.transform.position) <= interactionRadius)
                    {
                        Rigidbody itemRigidbody = collider.GetComponent<Rigidbody>();
                        if (itemRigidbody != null)
                        {
                            heldItem = itemRigidbody;
                            PickUpItem(itemRigidbody);
                            break;
                        }
                    }
                }
            }
            else
            {
                // Throw the held item
                ThrowItem();
                ExitInspectMode();
                heldItem = null; // Reset heldItem reference after throwing
            }
        }

        // Check for "I" key press to toggle inspection mode
        if (Input.GetKeyDown(KeyCode.I) && heldItem != null)
        {
            isInspecting = !isInspecting; // Toggle inspection mode
            if (isInspecting)
            {
                EnterInspectMode();
            }
            else
            {
                ExitInspectMode();
            }
        }

        // Apply swaying motion to the held item or handle inspection
        if (heldItem != null)
        {
            if (!isInspecting)
            {
                // Apply swaying motion
                float sway = Mathf.Sin(Time.time * swaySpeed) * swayAmount;
                heldItem.transform.localPosition = new Vector3(0f, sway, 0f); // Adjust as per required axis
            }
            else
            {
                InspectItem();
            }
        }
    }

    private void PickUpItem(Rigidbody itemRigidbody)
    {
        itemRigidbody.isKinematic = true; // Set the item to kinematic so it doesn't fall
        itemRigidbody.detectCollisions = false; // Disable physics interactions (optional)
        itemRigidbody.transform.SetParent(itemHoldPosition);

        // Set the local position of the item relative to the itemHoldPosition
        itemRigidbody.transform.localPosition = new Vector3(0f, 0f, dropForwardOffset);
        itemRigidbody.transform.localRotation = Quaternion.identity; // Reset rotation
    }

    private void ThrowItem()
    {
        heldItem.transform.SetParent(null); // Remove the parent
        heldItem.isKinematic = false; // Enable physics interactions
        heldItem.detectCollisions = true; // Enable collision detection

        // Calculate the throw direction based on the camera's forward direction
        Vector3 throwDirection = transform.forward + transform.up * throwUpOffset - transform.right * throwLeftOffset;

        // Apply force to throw the item
        heldItem.AddForce(throwDirection * throwForce, ForceMode.Impulse);
    }

    private void EnterInspectMode()
    {
        playerController.canMove = false; // Disable the PlayerController script to stop mouse look

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void ExitInspectMode()
    {
        playerController.canMove = true; // Enable the PlayerController script to resume mouse look

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        isInspecting = false;
    }

    private void InspectItem()
    {
        if (inspectionPosition != null && heldItem != null)
        {
            // Get the pivot point of the heldItem from its collider (center of its bounds)
            Collider itemCollider = heldItem.GetComponent<Collider>();
            if (itemCollider != null)
            {
                Vector3 pivot = itemCollider.bounds.center;

                // Set the pivot as the inspection position
                heldItem.transform.position = inspectionPosition.position;

                // Rotate around the pivot point
                float rotationX = Input.GetAxis("Mouse X") * 3f;
                float rotationY = Input.GetAxis("Mouse Y") * 3f;
                float rotationZ = Input.GetAxis("Mouse ScrollWheel") * 3f;

                Vector3 eulerRotation = new Vector3(rotationY, -rotationX, -rotationZ);
                Quaternion deltaRotation = Quaternion.Euler(eulerRotation);
                Quaternion newRotation = deltaRotation * heldItem.transform.rotation;

                heldItem.transform.rotation = newRotation;
                heldItem.transform.RotateAround(pivot, Vector3.forward, rotationZ);
            }
            else
            {
                Debug.LogWarning("Collider component not found on the held item.");
                isInspecting = false;
            }
        }
        else
        {
            Debug.LogWarning("Inspection position or held item is not set.");
            isInspecting = false;
        }
    }

}

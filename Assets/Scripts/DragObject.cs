using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Disk))]
public class Drag3D : MonoBehaviour
{
    private Vector3 offset;
    private float zCoord;
    private Camera mainCamera;

    private bool isDragging = false;
    private Vector3 startPosition;

    private Peg hoveredPeg;
    private Disk disk;

    private void Awake()
    {
        mainCamera = Camera.main;
        disk = GetComponent<Disk>();

        if (disk == null)
        {
            Debug.LogError("Disk component missing on " + gameObject.name);
        }
    }

    private void Update()
    {
        if (Mouse.current == null) return;

        // --- Start dragging ---
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.transform == transform)
                {
                    startPosition = transform.position;

                    zCoord = mainCamera.WorldToScreenPoint(transform.position).z;
                    offset = transform.position - GetMouseWorldPosition();
                    isDragging = true;
                }
            }
        }

        // --- While dragging ---
        if (isDragging)
        {
            transform.position = GetMouseWorldPosition() + offset;
        }

        // --- Release dragging ---
        if (Mouse.current.leftButton.wasReleasedThisFrame && isDragging)
        {
            isDragging = false;

            if (hoveredPeg != null)
            {
                // Remove from old peg
                if (disk.currentPeg != null)
                {
                    disk.currentPeg.RemoveDisk(transform);
                }

                // Compute actual disk height using Renderer bounds
                Renderer rend = GetComponent<Renderer>();
                float height = rend.bounds.size.y;

                // Stack on top of existing disks
                Vector3 snapPos = hoveredPeg.snapPoint.position;
                snapPos.y += height * hoveredPeg.DiskCount + height / 2f;

                // Apply final position
                transform.position = snapPos;

                // Update peg and disk state
                hoveredPeg.AddDisk(transform);
                disk.currentPeg = hoveredPeg;
                GameManager.Instance.RegisterMove(disk.weight);

            }
            else
            {
                // No peg → return to start
                transform.position = startPosition;
            }
        }
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePos = Mouse.current.position.ReadValue();
        mousePos.z = zCoord;
        return mainCamera.ScreenToWorldPoint(mousePos);
    }

    private void OnTriggerEnter(Collider other)
    {
        Peg peg = other.GetComponent<Peg>();
        if (peg != null)
        {
            hoveredPeg = peg;
            Debug.Log("Entered peg: " + peg.name);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Peg peg = other.GetComponent<Peg>();
        if (peg != null && peg == hoveredPeg)
        {
            hoveredPeg = null;
            Debug.Log("Exited peg: " + peg.name);
        }
    }
}

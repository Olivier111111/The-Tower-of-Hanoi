using System.Collections.Generic;
using UnityEngine;

public class Peg : MonoBehaviour
{
    [Header("Base point where disks stack")]
    public Transform snapPoint;

    // Internal list of disks on this peg
    private List<Transform> disks = new List<Transform>();

    // Public read-only property to get the current number of disks
    public int DiskCount => disks.Count;

    // Add a disk to this peg
    public void AddDisk(Transform disk)
    {
        if (!disks.Contains(disk))
        {
            disks.Add(disk);
        }
    }

    // Remove a disk from this peg
    public void RemoveDisk(Transform disk)
    {
        if (disks.Contains(disk))
        {
            disks.Remove(disk);
        }
    }

    // Optional helper: get the position for the next disk on this peg
    public Vector3 GetNextDiskPosition(float diskHeight)
    {
        Vector3 pos = snapPoint.position;
        pos.y += diskHeight * disks.Count + diskHeight / 2f;
        return pos;
    }
}

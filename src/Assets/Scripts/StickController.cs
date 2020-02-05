using SpriteGlow;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickController : MonoBehaviour {
    
    Stack<DiskController> _disks = new Stack<DiskController>();

    // glow effect component
    SpriteGlowEffect glowing;

    void Start()
    {
        glowing = GetComponent<SpriteGlowEffect>();

        glowing.enabled = false;
    }

    /// <summary>
    /// Initialize the stick with a list of disks
    /// </summary>
    /// <param name="disks">The list of disks to pile in the stick. The disks will be automatically sorted by size</param>
    public void Initialize(DiskController[] disks)
    {
        // Sort disk from the bigger to the smaller 
        List<DiskController> ordered = new List<DiskController>(disks);
        ordered.Sort(new Comparison<DiskController>((x, y) => x.Size.CompareTo(y.Size)*-1));
        foreach (var disk in ordered)
        {
            AddDisk(disk);
        }
    }

    /// <summary>
    /// Return the number of disks now stacked on this stick
    /// </summary>
    /// <returns>The number of disks in the stick</returns>
    public int DiskCount()
    {
        return _disks.Count;
    }

    /// <summary>
    /// Check if a disk can be added to the current stick
    /// </summary>
    /// <param name="disk">The disk that should be dropped in the stick</param>
    /// <returns>True if the disk can be dropped, false otherwise</returns>
    public bool CanAddDisk(DiskController disk)
    {
        // No disk -> allow dropping
        if (_disks.Count == 0)
        {
            return true;
        }

        // Allow dropping only if the Disk on top has a size bigger than the one is being dropped
        if (_disks.Peek().Size >= disk.Size)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Pile the disk to the stick
    /// </summary>
    /// <param name="disk">The disk to pile</param>
    public void AddDisk(DiskController disk)
    {
        if (CanAddDisk(disk))
        {
            disk.currentStick = this;

            _disks.Push(disk);
        }
    }

    /// <summary>
    /// Check whcih disk is at the top of the stick
    /// </summary>
    /// <returns>The disk at the top</returns>
    public DiskController GetTopDisk()
    {
        // No disk -> return null
        if (_disks.Count == 0)
        {
            return null;
        }
        else
        {
            return _disks.Peek();
        }
    }

    /// <summary>
    /// Remove the top disk from the stick
    /// </summary>
    /// <returns>The removed disk</returns>
    public DiskController RemoveTopDisk()
    {
        // No disk -> return null
        if (_disks.Count == 0)
        {
            return null;
        }
        else
        {
            DiskController disk = _disks.Pop();
            disk.currentStick = null;
            return disk;
        }
    }

    /// <summary>
    /// Set the disks on this stick transparent with a fade-out effect
    /// </summary>
    /// <param name="trans">If true it will fade-out in a transparency, il will go back to opaque otherwise</param>
    /// <param name="dragged">Set as the disk to not set transparent. For example the disk being dragged on the stick should not be transparent</param>
    public void SetDisksTransparent(bool trans, DiskController dragged)
    {
        // Stop any fade in action
        StopAllCoroutines();

        // Set glowing of the stick        
        glowing.enabled = trans;

        // Iterate through all disks on the disk
        foreach (var disk in _disks)
        {
            if (dragged == null || dragged.name != disk.name)
            {
                // Iterate through all childer of the disk (like the back sprite)
                foreach (var render in disk.GetComponentsInChildren<SpriteRenderer>())
                {
                    if (trans)
                    {
                        // Use coroutine to not interrupt the frame rate
                        StartCoroutine(FadeOut(render));
                    }
                    else
                    {
                        render.color = new Color(1, 1, 1, 1);
                    }
                }
            }
        }
    }
    
    // Change the alpha value of the sprite 
    IEnumerator FadeOut(SpriteRenderer rend)
    {
        for (float i = 1f; i >= 0.4f; i -= 0.1f)
        {
            Color c = rend.color;
            c.a = i;
            rend.color = c;

            yield return new WaitForSeconds(0.1f);
        }
    }
}

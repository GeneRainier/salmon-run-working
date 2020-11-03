using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class InputFunctions
{
    /// <summary>
    /// Returns true if mouse moves, false otherwise
    /// </summary>
    public static bool CheckForMouseMvt()
    {
        if (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// Gets the mouses position relative to the origin, as a point in Unity space
    /// </summary>
    /// <param name="origin"></param>
    /// <returns></returns>
    public static Vector3 GetMouseVector(GameObject origin)
    {
        Vector3 target = Input.mousePosition;

        Vector3 object_pos = Camera.main.WorldToScreenPoint(origin.transform.position);

        target.x = target.x - object_pos.x;
        target.y = target.y - object_pos.y;
        target.z = target.z - object_pos.z;

        return target;
    }
    
    public static bool IsMoving()
    {
        if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
        {
            return true;
        }
        return false;
    }

    public static void ToggleCursor(bool cursorOn)
    {
        Cursor.visible = cursorOn;
        if (cursorOn)
        {
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}

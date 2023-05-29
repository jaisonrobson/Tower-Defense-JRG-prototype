using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFocusController : MonoBehaviour
{
    private void OnMouseDown()
    {
        if (CameraController.instanceExists)
            CameraController.instance.followTransform = transform;
    }
}

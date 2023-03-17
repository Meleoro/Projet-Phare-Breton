using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationZoneFlute : MonoBehaviour
{
    void Update()
    {
        transform.rotation = ReferenceManager.Instance.cameraRotationReference.transform.rotation;
    }
}

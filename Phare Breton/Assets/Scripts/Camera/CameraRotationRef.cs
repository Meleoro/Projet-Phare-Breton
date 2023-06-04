using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotationRef : MonoBehaviour
{
    public Vector3 wantedRotation;
    public Vector3 currentRotation;
    
    void Update()
    {
        transform.rotation = Quaternion.Euler(currentRotation.x, currentRotation.y, currentRotation.z);

        //AdapteRoation();
    }


    public void AdapteRoation()
    {
        currentRotation = Vector3.Lerp(currentRotation, wantedRotation, Time.deltaTime);
    }
}

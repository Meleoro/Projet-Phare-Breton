using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationCanvas : MonoBehaviour
{

    private void Update()
    {
        ActualiseRotation();
    }



    public void ActualiseRotation()
    {
        Vector3 directionCamera = transform.position - ReferenceManager.Instance.cameraReference.transform.position;

        transform.rotation = Quaternion.LookRotation(-directionCamera) * Quaternion.Euler(0, 180, 0);
    }
}

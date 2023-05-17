using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planche : ObjetInteractible
{
    private void FixedUpdate()
    {
        if(isMoved)
            RotatePlanche();
    }

    public void RotatePlanche()
    {
        Quaternion newRot = Quaternion.Lerp(transform.rotation, new Quaternion(0, transform.rotation.y, 0, transform.rotation.w), Time.deltaTime * 3);
        
        transform.rotation = newRot;
    }
}

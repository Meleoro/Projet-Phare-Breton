using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planche : ObjetInteractible
{
    public ParticleSystem VFXDeplacement;

    private void FixedUpdate()
    {
        if(isMoved)
            RotatePlanche();
        
        if(isMagneted)
            MagnetEffect();
    }

    public void RotatePlanche()
    {
        Quaternion newRot = Quaternion.Lerp(transform.rotation, new Quaternion(0, transform.rotation.y, 0, transform.rotation.w), Time.fixedDeltaTime * 3);
        
        transform.rotation = newRot;
    }
}

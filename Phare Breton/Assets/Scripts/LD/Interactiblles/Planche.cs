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

    private void OnCollisionEnter(Collision collision)
    {
        if (!rb.isKinematic && !collision.gameObject.CompareTag("Player") && !collision.collider.isTrigger && !isMoved && !isMagneted && !cantPlaySound)
        {
            if (!didSound)
            {
                didSound = true;
                
                AudioManager.instance.PlaySoundOneShot(5, 1, 0, GetComponent<AudioSource>());
            }
        }
    }

    public void RotatePlanche()
    {
        Quaternion newRot = Quaternion.Lerp(transform.rotation, new Quaternion(0, transform.rotation.y, 0, transform.rotation.w), Time.fixedDeltaTime * 3);
        
        transform.rotation = newRot;
    }
}

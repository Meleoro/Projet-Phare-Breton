using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Boite : ObjetInteractible
{
    public ParticleSystem VFXDeplacement;

    public bool isElectrified;
    public ParticleSystem VFXelectricity;


    public override void StopVFX()
    {
        DesactivateStase();

        VFXDeplacement.Stop();
        VFXelectricity.Stop();
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (!rb.isKinematic && !collision.gameObject.CompareTag("Player") && !collision.collider.isTrigger && !isMoved && !cantPlaySound)
        {
            if (!didSound)
            {
                didSound = true;
                
                AudioManager.instance.PlaySoundOneShot(4, 1, 0, GetComponent<AudioSource>());
            }
        }
    }



    // VERIFIE SI UN PANNEAU EST RELIE
    public override void VerifyLinkedObject()
    {
        if (!isInStase)
        {
            isElectrified = false;
        }

        for (int k = 0; k < linkedObject.Count; k++)
        {
            PanneauElectrique currentPanneau;

            if (linkedObject[k].TryGetComponent(out currentPanneau))
            {
                isElectrified = true;
            }


            Boite currentBoite;
            if (linkedObject[k].TryGetComponent(out currentBoite))
            {
                if(currentBoite.isElectrified)
                    
                    isElectrified = true;
            }
        }


        if (isElectrified)
        {
            VFXelectricity.Play();
        }
        else
        {
            VFXelectricity.Stop();
        }
    }
}

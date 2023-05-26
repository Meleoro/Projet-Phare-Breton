using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Boite : ObjetInteractible
{
    public ParticleSystem VFXDeplacement;

    public bool isElectrified;
    public ParticleSystem VFXelectricity;
    
    
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

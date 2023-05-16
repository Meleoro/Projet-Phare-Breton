using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Boite : ObjetInteractible
{
    public ParticleSystem VFXDeplacement;

    public bool isElectrified;
    
    
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

            if (linkedObject[k].TryGetComponent<PanneauElectrique>(out currentPanneau))
            {
                isElectrified = true;
            }


            Boite currentBoite;
            if (linkedObject[k].TryGetComponent<Boite>(out currentBoite))
            {
                if(currentBoite.isElectrified)
                    
                    isElectrified = true;
            }
        }
    }
}

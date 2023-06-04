using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanneauElectrique : ObjetInteractible
{
    [Header("PanneauElectrique")] 
    public ParticleSystem etincelles;
    public ParticleSystem energie;
    private bool isUsed;
    
    
    public override void VerifyLinkedObject()
    {
        if (!isInStase)
        {
            isUsed = false;
        }

        for (int k = 0; k < linkedObject.Count; k++)
        {
            Boite currentBoite;
            if (linkedObject[k].TryGetComponent(out currentBoite))
            {
                isUsed = true;
            }

            Ampoule currentAmpoule;
            if (linkedObject[k].TryGetComponent(out currentAmpoule))
            {
                isUsed = true;
            }
        }


        if (isUsed)
        {
            energie.Play();
            
            etincelles.Stop();
        }
        else
        {
            etincelles.Play();
            
            energie.Stop();
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ampoule : ObjetInteractible
{
    [Header("Ampoule")]
    [SerializeField] private bool ampouleActive;
    [SerializeField] private Light lightComponent;
    [SerializeField] private SphereCollider lightArea;

    private bool doOnce = false;


    void Update()
    {
        if (ampouleActive)
        {
            ActivateAmpoule();
        }

        else
        {
            lightComponent.enabled = false;
            lightArea.enabled = false;

            if (doOnce)
            {
                doOnce = false;
                AudioManager.instance.PlaySoundOneShot(8, 1, 0, ReferenceManager.Instance.characterReference.playerAudioSource);
            }
        }
    }

    // COMPORTEMENT DE L'OBJET SI IL EST UNE AMPOULE
    private void ActivateAmpoule()
    {
        lightComponent.enabled = true;
        lightArea.enabled = true;

        if (!doOnce)
        {
            doOnce = true;
            AudioManager.instance.PlaySoundOneShot(7, 1, 0, ReferenceManager.Instance.characterReference.playerAudioSource);
        }
    }


    // VERIFIE SI UN PANNEAU EST RELIE
    public override void VerifyLinkedObject()
    {
        if (isInStase)
        {
            ampouleActive = false;
        }

        for (int k = 0; k < linkedObject.Count; k++)
        {
            PanneauElectrique currentPanneau;

            if (linkedObject[k].TryGetComponent<PanneauElectrique>(out currentPanneau))
            {
                ampouleActive = true;
            }


            Boite currentBoite;
            if (linkedObject[k].TryGetComponent<Boite>(out currentBoite))
            {
                if(currentBoite.isElectrified)
                    ampouleActive = true;
            }
        }
    }
    
    
    
    private void OnTriggerStay(Collider other)
    {
        /*if (other.CompareTag("Interactible") && !other.isTrigger)
        {
            other.GetComponent<ObjetInteractible>().isLighted = true;
        }

        if (other.CompareTag("Player") && !other.isTrigger)
        {
            ReferenceManager.Instance.characterReference.isInLightSource = true;
        }*/

        if (other.CompareTag("ZoneSombre"))
        {
            if(ampouleActive)
                other.GetComponent<ColliderZoneSombre>().DesactivateZoneSombre();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Interactible") && !other.isTrigger)
        {
            other.GetComponent<ObjetInteractible>().isLighted = false;
        }
        
        if (other.CompareTag("Player") && !other.isTrigger)
        {
            ReferenceManager.Instance.characterReference.isInLightSource = false;
        }
    }
}

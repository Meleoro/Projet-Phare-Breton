using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ampoule : ObjetInteractible
{
    [Header("Ampoule")]
    [SerializeField] private bool ampouleActive;
    [SerializeField] private Light lightComponent;
    [SerializeField] private SphereCollider lightArea;


    void Update()
    {
        if (ampouleActive)
        {
            ActivateAmpoule();
        }
    }


    // COMPORTEMENT DE L'OBJET SI IL EST UNE AMPOULE
    private void ActivateAmpoule()
    {
        lightComponent.enabled = true;
        lightArea.enabled = true;
    }


    // VERIFIE SI UN PANNEAU EST RELIE
    private void VerifyLinkedObject()
    {
        ampouleActive = false;

        for (int k = 0; k < linkedObject.Count; k++)
        {
            PanneauElectrique currentPanneau;

            if (linkedObject[k].TryGetComponent<PanneauElectrique>(out currentPanneau))
            {
                ampouleActive = true;
            }
        }
    }
}

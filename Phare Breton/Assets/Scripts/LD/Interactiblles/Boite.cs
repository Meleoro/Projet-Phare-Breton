using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Boite : ObjetInteractible
{
    public ParticleSystem VFXDeplacement;

    private void Start()
    {
        VFXDeplacement.Stop(true);
    }
}

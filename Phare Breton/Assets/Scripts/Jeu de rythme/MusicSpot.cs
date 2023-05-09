using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicSpot : MonoBehaviour
{
    [Range(1, 3)] public int melodyIndex;

    [SerializeField] private Transform cameraPos;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player") && !other.isTrigger)
        {
            ReferenceManager.Instance.characterReference.canPlayMusic = true;
            ReferenceManager.Instance.characterReference.currentMelodyIndex = melodyIndex;

            ReferenceManager.Instance.cameraReference.posCameraRythme = cameraPos;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && !other.isTrigger)
        {
            ReferenceManager.Instance.characterReference.canPlayMusic = false;
            ReferenceManager.Instance.characterReference.currentMelodyIndex = melodyIndex;
            
            ReferenceManager.Instance.cameraReference.posCameraRythme = null;
        }
    }
}

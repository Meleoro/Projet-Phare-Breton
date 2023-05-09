using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicSpot : MonoBehaviour
{
    [Range(1, 3)] public int melodyIndex;

    [SerializeField] private Transform cameraPos;
    [SerializeField] private Transform cameraPos2;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player") && !other.isTrigger)
        {
            ReferenceManager.Instance.characterReference.canPlayMusic = true;
            ReferenceManager.Instance.characterReference.currentMelodyIndex = melodyIndex;

            ReferenceManager.Instance.cameraReference.posCameraRythme = cameraPos;
            ReferenceManager.Instance.cameraReference.posCameraRythme2 = cameraPos2;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && !other.isTrigger)
        {
            ReferenceManager.Instance.characterReference.canPlayMusic = false;
            ReferenceManager.Instance.characterReference.currentMelodyIndex = melodyIndex;
            
            ReferenceManager.Instance.cameraReference.posCameraRythme = null;
            ReferenceManager.Instance.cameraReference.posCameraRythme2 = null;
        }
    }
}

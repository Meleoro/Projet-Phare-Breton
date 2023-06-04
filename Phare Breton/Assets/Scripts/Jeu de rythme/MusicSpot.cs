using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MusicSpot : MonoBehaviour
{
    [Range(1, 3)] public int melodyIndex;

    [SerializeField] private Transform cameraPos;
    [SerializeField] private Transform cameraPos2;

    public List<GameObject> tags = new List<GameObject>();


    private void Start()
    {
        for (int i = 0; i < tags.Count; i++)
        {
            tags[i].GetComponent<MeshRenderer>().material.DOFade(0, 0);
        }
    }



    private void OnTriggerEnter(Collider other)
    {
        if (!ReferenceManager.Instance.characterReference.notesScript.wonMelodies[melodyIndex - 1])
        {
            if(other.CompareTag("Player") && !other.isTrigger)
            {
                ReferenceManager.Instance.characterReference.canPlayMusic = true;
                ReferenceManager.Instance.characterReference.currentMelodyIndex = melodyIndex;

                ReferenceManager.Instance.cameraReference.posCameraRythme = cameraPos;
                ReferenceManager.Instance.cameraReference.posCameraRythme2 = cameraPos2;

                ReferenceManager.Instance.characterReference.tagToActivate = tags;
            }
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
            
            ReferenceManager.Instance.characterReference.tagToActivate.Clear();
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Note : ObjetInteractible
{
    [Header("Note")]
    [Min(1)] public int partitionNumber;   // Quelle partition
    [Min(1)] public int posInPartitionNumber;   // Quelle place dans cette partition
    [HideInInspector] public bool isNote;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ReferenceManager.Instance.characterReference.nearNoteObject = gameObject;
            ReferenceManager.Instance.characterReference.nearNotePartitionNumber = partitionNumber;
            ReferenceManager.Instance.characterReference.nearNoteNumber = posInPartitionNumber;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ReferenceManager.Instance.characterReference.nearNotePartitionNumber = 0;
            ReferenceManager.Instance.characterReference.nearNoteNumber = 0;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Note : ObjetInteractible
{
    [Header("Note")]
    [Range(1, 3)] public int partitionNumber;   // Quelle partition
    [Range(1, 3)]  public int posInPartitionNumber;   // Quelle place dans cette partition

    public bool isInBiblio;

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && !other.isTrigger)
        {
            ReferenceManager.Instance.characterReference.nearNoteObject = gameObject;
            ReferenceManager.Instance.characterReference.nearNotePartitionNumber = partitionNumber;
            ReferenceManager.Instance.characterReference.nearNoteNumber = posInPartitionNumber;
            ReferenceManager.Instance.characterReference.isInBiblio = isInBiblio;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && !other.isTrigger)
        {
            ReferenceManager.Instance.characterReference.nearNotePartitionNumber = 0;
            ReferenceManager.Instance.characterReference.nearNoteNumber = 0;
            ReferenceManager.Instance.characterReference.isInBiblio = false;
        }
    }
}

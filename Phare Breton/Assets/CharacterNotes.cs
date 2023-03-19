using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterNotes : MonoBehaviour
{
    public List<int> notesPerMelody;
    
    [HideInInspector] public List<bool> collectedNotes1;

    public void AddNote(int partition, int notePos)
    {
        if (partition == 1)
        {
            collectedNotes1[notePos - 1] = true;
        }
    }
}

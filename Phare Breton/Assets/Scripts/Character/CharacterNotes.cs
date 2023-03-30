using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterNotes : MonoBehaviour
{
    [HideInInspector] public List<bool> collectedNotes1;
    [HideInInspector] public List<bool> collectedNotes2;

    private void Start()
    {
    }

    public void Initialize()
    {
        for (int k = 0; k < ReferenceManager.Instance.noteManagerReference.Melodies[0].nbrNotes; k++)
        {
            collectedNotes1.Add(false);
        }
        
        for (int j = 0; j <  ReferenceManager.Instance.noteManagerReference.Melodies[1].nbrNotes; j++)
        {
            collectedNotes2.Add(false);
        }
    }

    public void AddNote(int partition, int notePos)
    {
        if (partition == 1)
        {
            collectedNotes1[notePos - 1] = true;
        }
        else if (partition == 2)
        {
            collectedNotes2[notePos - 1] = true;
        }
    }


    public void Play()
    {
        
    }
}

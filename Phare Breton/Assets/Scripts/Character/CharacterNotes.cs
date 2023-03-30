using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterNotes : MonoBehaviour
{
    public List<CollectedNotes> collectedNotes;

    private List<GameObject> bandes;


    private void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
        for (int k = 0; k < 3; k++)
        {
            for (int j = 0; j < ReferenceManager.Instance.noteManagerReference.Melodies[k].nbrNotes; j++)
            {
                collectedNotes[k].currentCollectedNotes.Add(false);
            }
        }
    }

    public void AddNote(int partition, int notePos)
    {
        collectedNotes[partition - 1].currentCollectedNotes[notePos - 1] = true;
    }


    public void Play(int melodyIndex)
    {
        if (!collectedNotes[melodyIndex].currentCollectedNotes.Contains(false))
        {
            // On récupère chaque bande de cette mélodie
            bandes.Clear();

            for(int k = 0; k < ReferenceManager.Instance.noteManagerReference.Melodies[melodyIndex - 1].bandes.Count; k++)
            {
                bandes.Add(ReferenceManager.Instance.noteManagerReference.Melodies[melodyIndex - 1].bandes[k]);
            }


            // On commence le mini jeu
            bandes[0].GetComponent<BandeJeuDeRythme>().LaunchGame();
        }
    }
}



[Serializable]
public class CollectedNotes
{
    public List<bool> currentCollectedNotes = new List<bool>();
}

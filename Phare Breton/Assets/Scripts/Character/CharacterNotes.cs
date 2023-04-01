using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterNotes : MonoBehaviour
{
    private CharaManager mainSript;

    [HideInInspector] public List<CollectedNotes> collectedNotes = new List<CollectedNotes>();

    private List<GameObject> bandes = new List<GameObject>();
    private int currentBande;


    private void Start()
    {
        Initialize();

        mainSript = GetComponent<CharaManager>();
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


    public void StartPlay(int melodyIndex)
    {
        if (collectedNotes[melodyIndex].currentCollectedNotes.Contains(false))
        {
            // On récupère chaque bande de cette mélodie
            bandes.Clear();

            for(int k = 0; k < ReferenceManager.Instance.noteManagerReference.Melodies[melodyIndex - 1].bandes.Count; k++)
            {
                bandes.Add(ReferenceManager.Instance.noteManagerReference.Melodies[melodyIndex - 1].bandes[k]);
            }


            // On commence le mini jeu
            GameObject newBande = Instantiate(bandes[0]);
            newBande.GetComponent<BandeJeuDeRythme>().LaunchGame();

            currentBande = 1;

            mainSript.noControl = true;
        }
    }

    public void NextBande()
    {
        if(currentBande < bandes.Count)
        {
            GameObject newBandeRef = bandes[currentBande];

            GameObject newBande = Instantiate(newBandeRef);
            newBande.GetComponent<BandeJeuDeRythme>().LaunchGame();

            currentBande += 1;
        }

        else
        {
            mainSript.noControl = false;

            for (int i = 0; i < bandes.Count; i++)
            {
                bandes[i].SetActive(false);
            }

            bandes.Clear();
        }
    }
}



[Serializable]
public class CollectedNotes
{
    public List<bool> currentCollectedNotes = new List<bool>();
}

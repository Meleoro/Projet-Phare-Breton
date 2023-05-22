using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterNotes : MonoBehaviour
{
    private CharaManager mainSript;

    public List<CollectedNotes> collectedNotes = new List<CollectedNotes>();

    private List<GameObject> bandes = new List<GameObject>();
    private List<GameObject> bandesObjects = new List<GameObject>();
    private int currentBande;
    private int currentMelody;

    [Header("References")] 
    [SerializeField] private ParticleSystem VFXReussite;
    [SerializeField] private ParticleSystem VFXEchec;


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
        currentMelody = melodyIndex;

        if (!collectedNotes[melodyIndex - 1].currentCollectedNotes.Contains(false))
        {
            ReferenceManager.Instance.cameraReference.StartMoveCameraRythme();
            
            // On recupere chaque bande de cette melodie
            bandes.Clear();

            for(int k = 0; k < ReferenceManager.Instance.noteManagerReference.Melodies[melodyIndex - 1].bandes.Count; k++)
            {
                bandes.Add(ReferenceManager.Instance.noteManagerReference.Melodies[melodyIndex - 1].bandes[k]);
            }


            // On commence le mini jeu
            GameObject newBande = Instantiate(bandes[0]);
            newBande.GetComponentInChildren<BandeJeuDeRythme>().LaunchGame();

            bandesObjects.Add(newBande);

            currentBande = 1;

            mainSript.noControl = true;
        }
    }

    public void NextBande()
    {
        if(currentBande < bandes.Count)
        {
            GameObject newBande = Instantiate(bandes[currentBande]);
            newBande.GetComponentInChildren<BandeJeuDeRythme>().LaunchGame();

            bandesObjects.Add(newBande);

            currentBande += 1;
        }

        else
        {
            mainSript.noControl = false;
            
            ReferenceManager.Instance.cameraReference.StopMoveCameraRythme();
            DoVFXReussite();
    
            UnlockPower();

            for (int i = 0; i < bandes.Count; i++)
            {
                Destroy(bandesObjects[i]);
            }

            bandes.Clear();
            bandesObjects.Clear();
        }
    }


    public void UnlockPower()
    {
        if(currentMelody == 1)
        {
            mainSript.canMoveObjects = true;
        }
        else if(currentMelody == 2)
        {
            mainSript.canCable = true;
        }
        else
        {
            mainSript.canStase = true;
        }
    }


    public void DoVFXReussite()
    {
        ParticleSystem newParticle = Instantiate(VFXReussite, transform.position, Quaternion.Euler(-90, 0, 0));
        
        Destroy(newParticle.gameObject, 10f);
    }

    public void DoVFXEchec()
    {
        ParticleSystem newParticle = Instantiate(VFXEchec, transform.position, Quaternion.Euler(-90, 0, 0));
        
        Destroy(newParticle.gameObject, 10f);
    }
}



[Serializable]
public class CollectedNotes
{
    public List<bool> currentCollectedNotes = new List<bool>();
}

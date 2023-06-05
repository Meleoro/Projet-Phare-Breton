using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class CharacterNotes : MonoBehaviour
{
    private CharaManager mainSript;

    public List<CollectedNotes> collectedNotes = new List<CollectedNotes>();

    private List<GameObject> bandes = new List<GameObject>();
    private List<GameObject> bandesObjects = new List<GameObject>();
    public List<bool> wonMelodies = new List<bool>();
    private int currentBande;
    private int currentMelody;

    [Header("References")] 
    [SerializeField] private ParticleSystem VFXReussite;
    [SerializeField] private ParticleSystem VFXEchec;
    [SerializeField] private UINotes UIScript;


    private void Start()
    {
        //Initialize();

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

    public void GiveAllNotes()
    {
        for (int k = 0; k < 3; k++)
        {
            for (int j = 0; j < ReferenceManager.Instance.noteManagerReference.Melodies[k].nbrNotes; j++)
            {
                collectedNotes[k].currentCollectedNotes.Add(true);
            }
        }
    }

    public void AddNote(int partition, int notePos)
    {
        collectedNotes[partition - 1].currentCollectedNotes[notePos - 1] = true;

        StartCoroutine(UIScript.GainNote(notePos));
    }


    public void StartPlay(int melodyIndex)
    {
        if (!ReferenceManager.Instance.cameraReference.goToSave)
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
                newBande.GetComponentInChildren<BandeJeuDeRythme>().LaunchGame(currentMelody - 1);

                bandesObjects.Add(newBande);

                currentBande = 1;

                mainSript.noControl = true;
            }

            else
            {
                StartCoroutine(ReferenceManager.Instance.characterReference.SayNo());
                StartCoroutine(UIScript.NoNotes());
            }
        }
    }

    public void NextBande()
    {
        if(currentBande < bandes.Count)
        {
            GameObject newBande = Instantiate(bandes[currentBande]);
            newBande.GetComponentInChildren<BandeJeuDeRythme>().LaunchGame(currentMelody - 1);

            bandesObjects.Add(newBande);

            currentBande += 1;
        }

        else
        {
            AudioManager.instance.FadeOutAudioSource(0.2f, 0.5f, 0,
                ReferenceManager.Instance.characterReference.musicAudioSource);

            ReferenceManager.Instance.cameraReference.StopMoveCameraRythme();
            DoVFXReussite();
            StartCoroutine(UIScript.PutEverythingGray(false));
    
            StartCoroutine(UnlockPower());

            wonMelodies[currentMelody - 1] = true;
            mainSript.canPlayMusic = false;

            for (int i = 0; i < bandes.Count; i++)
            {
                Destroy(bandesObjects[i]);
            }

            bandes.Clear();
            bandesObjects.Clear();

            if (mainSript.tagToActivate.Count != 0)
            {
                StartCoroutine(ActualiseTags(4, 4));
            }
        }
    }


    private IEnumerator ActualiseTags(float timer, float original)
    {
        if (timer < 0)
        {
            mainSript.tagToActivate.Clear();
            yield break;
        }
        
        for(int i = 0; i < mainSript.tagToActivate.Count; i++)
        {
            mainSript.tagToActivate[i].GetComponent<MeshRenderer>().material.SetFloat("_alpha", Mathf.Lerp(1, 0, timer / original));
        }
        
        yield return new WaitForEndOfFrame();

        timer -= Time.deltaTime;
        StartCoroutine(ActualiseTags(timer, original));
    }
    

    public void StopPlay()
    {
        AudioManager.instance.FadeOutAudioSource(0.2f, 0.5f, 0,
            ReferenceManager.Instance.characterReference.playerAudioSource);
        
        mainSript.noControl = false;
            
        ReferenceManager.Instance.cameraReference.StopMoveCameraRythme();

        for (int i = 0; i < bandes.Count; i++)
        {
            Destroy(bandesObjects[i]);
        }

        bandes.Clear();
        bandesObjects.Clear();

        StartCoroutine(UINotes.Instance.LoseNote(1, true));
    }


    public IEnumerator UnlockPower()
    {
        AudioManager.instance.PlaySoundOneShot(10, 1, 0, mainSript.playerAudioSource);
        
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

        yield return new WaitForSeconds(6);

        mainSript.noControl = false;
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

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using DG.Tweening;
using UnityEngine.Rendering;


public class BandeJeuDeRythme : MonoBehaviour
{
    [Header("Infos Node Selectionnee")]
    [HideInInspector] public MusicNode currentNode;
    [HideInInspector] public bool isOnX;
    [HideInInspector] public bool isOnY;
    [HideInInspector] public bool isOnZ;
    
    [Header("Parametres deroulement")]
    public float dureePreparation;
    public float speedMoveNode;
    public float duration;
    public List<Node> nodes = new List<Node>();
    
    [Header("References")] 
    public GameObject nodeObjectX;
    public GameObject nodeObjectY;
    public GameObject nodeObjectZ;
    public RectTransform posSpawnLeft;
    public RectTransform posSpawnRight;
    public RectTransform barreAvancement;
    public RectTransform fond;
    public RectTransform arc;
    public List<RectTransform> waypointsLeft = new List<RectTransform>();
    public List<RectTransform> waypointsRight = new List<RectTransform>();

    [Header("ReferencesVFX")] 
    public Canvas canvaVFX;
    public ParticleSystem VFXNoteDestroy;

    [Header("Inputs")]
    private bool pressX;
    private bool pressY;
    private bool pressZ;
    
    [Header("Autre")]
    private float timer;
    private bool gameStarted;
    private bool startMoveBarre;
    [HideInInspector] public List<MusicNode> nodesCreated = new List<MusicNode>();
    [HideInInspector] public bool stop;
    [HideInInspector] public bool isFinishing;
    public int erasedNotes;
    private bool usingBarre;
    private int currentHealth;
    public int melodyIndex;
    public float delaybande1;
    private bool doOnce;


    private void Awake()
    {
        canvaVFX.worldCamera = ReferenceManager.Instance._cameraUI;
    }


    private void Start()
    {
        stop = false;
    }


    private void Update()
    {
        if (!stop)
        {
            if (gameStarted)
            {
                UpdateTimer();
            }

            if (pressX || pressY || pressZ)
            {
                if (currentNode != null)
                {
                    bool isRight = VerifyNote();

                    currentNode.EraseNode(isRight);

                    nodesCreated.Remove(currentNode);

                    Destroy(currentNode);
                    Destroy(currentNode.gameObject);

                    erasedNotes += 1;

                    if (isRight)
                    {
                        StartCoroutine(FeelDestroyNode());
                    }
                    else
                    {
                        RestartGame();
                    }
                }
                
                if (!usingBarre)
                {
                    AudioManager.instance.PlaySoundOneShot(1, 0, 0, ReferenceManager.Instance.characterReference.playerAudioSource);
                    
                    StartCoroutine(FeelBarre());
                }
            }
            
        }
    }



    public void LaunchGame(int index)
    {
        timer = 0;
        erasedNotes = 0;
        currentHealth = 3;
            
        ReferenceManager.Instance.cameraReference.durationRythme = duration;

        UINotes.Instance.StartGame();

        StartCoroutine(StartGameFeel(index));
    }

    public IEnumerator StartGameFeel(int index)
    {
        Image imageBarre =  barreAvancement.GetComponent<Image>();
        Image imageFond = fond.GetComponent<Image>();
        Image imageArc = arc.GetComponent<Image>();

        imageBarre.DOFade(0, 0);
        imageFond.DOFade(0, 0);
        imageArc.DOFade(0, 0);

        barreAvancement.DOMoveY(barreAvancement.position.y - 200, 0);
        fond.DOMoveY(fond.position.y - 200, 0);
        arc.DOMoveY(arc.position.y - 200, 0);

        yield return new WaitForSeconds(0.01f);
        
        imageBarre.DOFade(1, 1.5f).SetEase(Ease.Linear);
        imageFond.DOFade(0.4f, 1.5f).SetEase(Ease.Linear);
        imageArc.DOFade(1f, 1.5f).SetEase(Ease.Linear);

        barreAvancement.DOMoveY(barreAvancement.position.y + 200, 1.3f).SetEase(Ease.OutCubic);
        fond.DOMoveY(fond.position.y + 200, 1.3f).SetEase(Ease.OutCubic);
        arc.DOMoveY(arc.position.y + 200, 1.3f).SetEase(Ease.OutCubic);

        yield return new WaitForSeconds(1.6f);

        AudioManager.instance.PlaySoundContinuous(index, 2, 0, ReferenceManager.Instance.characterReference.musicAudioSource);
        
        gameStarted = true;
    }

    IEnumerator FeelBarre()
    {
        Vector3 originalScale = barreAvancement.localScale;
        float duration = 0.07f;
        
        usingBarre = true;
        barreAvancement.DOScale(barreAvancement.localScale * 1.2f, duration * 0.7f);
        barreAvancement.DOMoveY(barreAvancement.position.y + 16, duration * 0.7f);

        yield return new WaitForSeconds(duration * 0.7f);

        barreAvancement.DOScale(originalScale, duration);
        barreAvancement.DOMoveY(barreAvancement.position.y - 16, duration * 0.7f);

        yield return new WaitForSeconds(duration * 0.2f);
        
        pressX = false;
        pressY = false;
        pressZ = false;
        
        yield return new WaitForSeconds(duration * 0.8f);
        
        usingBarre = false;
    }
    

    IEnumerator FeelDestroyNode()
    {
        float duration = 0.23f;
        Volume volume = ReferenceManager.Instance.cameraReference.rythmeVolume;

        DOTween.To(() => volume.weight, x => volume.weight = x, 0.7f, duration * 0.5f);

        yield return new WaitForSeconds(duration * 0.5f);
            
        DOTween.To(() => volume.weight, x => volume.weight = x, 0, duration);
    }
    
    IEnumerator LoseEffects()
    {
        float duration = 0.23f;
        
        ReferenceManager.Instance.characterReference.notesScript.DoVFXEchec();
        ReferenceManager.Instance.cameraReference.DoCameraShake(0.4f, 0.2f);

        yield return new WaitForSeconds(duration);
        
        ReferenceManager.Instance.characterReference.notesScript.StopPlay();
    }


    public void RestartGame()
    {
        currentHealth -= 1;

        if(currentHealth > 0)
        {
            AudioManager.instance.PlaySoundOneShot(1, 1, 0, ReferenceManager.Instance.characterReference.playerAudioSource);

            StartCoroutine(UINotes.Instance.LoseNote(currentHealth + 1, false));
        }

        else
        {
            timer = 0f;
            erasedNotes = 0;

            stop = true;

            StartCoroutine(LoseEffects());

            for (int i = 0; i < nodes.Count; i++)
            {
                nodes[i].isSpawned = false;
            }

            for (int i = nodesCreated.Count - 1; i >= 0; i--)
            {
                Destroy(nodesCreated[i].gameObject);
            }

            nodesCreated.Clear();

            isOnX = false;
            isOnY = false;
            isOnZ = false;
            currentNode = null;

            //ReferenceManager.Instance.cameraReference.RestartMoveCameraRythme();
        }
    }


    public IEnumerator EndGame()
    {
        isFinishing = true;

        yield return new WaitForSeconds(0.5f);

        stop = true;
        
        ReferenceManager.Instance.characterReference.notesScript.NextBande();
    }


    public void UpdateTimer()
    {
        timer += Time.deltaTime;

        if ((timer > duration || erasedNotes >= nodes.Count) && !isFinishing)
        {
            StartCoroutine(EndGame());
        }

        for (int i = 0; i < nodes.Count; i++)
        {
            if (!nodes[i].isSpawned)
            {
                if (melodyIndex == 1)
                {
                    if (timer > nodes[i].spawnTiming + delaybande1)
                    {
                        MusicNode newNode = null;
                    
                        switch (nodes[i].nodeType)
                        {
                            case Node.InputNeeded.x :
                                newNode = Instantiate(nodeObjectX, transform.position, Quaternion.identity, transform).GetComponent<MusicNode>();
                                break;
                        
                            case Node.InputNeeded.y :
                                newNode = Instantiate(nodeObjectY, transform.position, Quaternion.identity, transform).GetComponent<MusicNode>();
                                break;
                        
                            case Node.InputNeeded.z :
                                newNode = Instantiate(nodeObjectZ, transform.position, Quaternion.identity, transform).GetComponent<MusicNode>();
                                break;
                        }

                        switch (nodes[i].spawnPos)
                        {
                            case Node.SpawnPos.left :
                                newNode.InitialiseNode(nodes[i].nodeType, nodes[i].spawnPos, this, waypointsLeft);
                                break;
                        
                            case Node.SpawnPos.right :
                                newNode.InitialiseNode(nodes[i].nodeType, nodes[i].spawnPos, this, waypointsRight);
                                break;
                        }
                    
                        nodes[i].isSpawned = true;
                        nodesCreated.Add(newNode);
                    }
                }

                else
                {
                    if (timer > nodes[i].spawnTiming + 0.05f)
                    {
                        MusicNode newNode = null;
                    
                        switch (nodes[i].nodeType)
                        {
                            case Node.InputNeeded.x :
                                newNode = Instantiate(nodeObjectX, transform.position, Quaternion.identity, transform).GetComponent<MusicNode>();
                                break;
                        
                            case Node.InputNeeded.y :
                                newNode = Instantiate(nodeObjectY, transform.position, Quaternion.identity, transform).GetComponent<MusicNode>();
                                break;
                        
                            case Node.InputNeeded.z :
                                newNode = Instantiate(nodeObjectZ, transform.position, Quaternion.identity, transform).GetComponent<MusicNode>();
                                break;
                        }

                        switch (nodes[i].spawnPos)
                        {
                            case Node.SpawnPos.left :
                                newNode.InitialiseNode(nodes[i].nodeType, nodes[i].spawnPos, this, waypointsLeft);
                                break;
                        
                            case Node.SpawnPos.right :
                                newNode.InitialiseNode(nodes[i].nodeType, nodes[i].spawnPos, this, waypointsRight);
                                break;
                        }
                    
                        nodes[i].isSpawned = true;
                        nodesCreated.Add(newNode);
                    }
                }
            }
        }

        for (int i = 0; i < nodesCreated.Count; i++)
        {
            nodesCreated[i].MoveNode(speedMoveNode);
        }
    }


    public void PlayVFXDestroy()
    {
        VFXNoteDestroy.Play();
    }


    public bool VerifyNote()
    {
        if(pressX && isOnX)
        {
            return true;
        }

        else if(pressY && isOnY)
        {
            return true;
        }

        else if(pressZ && isOnZ)
        {
            return true;
        }

        else
        {
            return false;
        }
    }



    public void OnX(InputAction.CallbackContext context)
    {
        if (context.started)
            pressX = true;

        if (context.canceled)
            pressX = false;
    }

    public void OnY(InputAction.CallbackContext context)
    {
        if (context.started)
            pressY = true;

        if (context.canceled)
            pressY = false;
    }

    public void OnZ(InputAction.CallbackContext context)
    {
        if (context.started)
            pressZ = true;

        if (context.canceled)
            pressZ = false;
    }
}


[Serializable]
public class Node
{
    public enum InputNeeded
    {
        x,
        y,
        z
    }
    
    public enum SpawnPos
    {
        left,
        right
    }

    public InputNeeded nodeType;
    public SpawnPos spawnPos;
    
    public float spawnTiming;

    [HideInInspector] public bool isSpawned = false;
    [HideInInspector] public bool isDestroyed = false;
}

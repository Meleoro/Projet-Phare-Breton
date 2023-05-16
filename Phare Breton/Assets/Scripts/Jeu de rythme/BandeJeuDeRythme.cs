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
    [HideInInspector] public int erasedNotes;
    private bool usingBarre;


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
                if (currentNode != null && !usingBarre)
                {
                    bool isRight = VerifyNote();

                    if (isRight)
                    {
                        currentNode.EraseNode();

                        nodesCreated.Remove(currentNode);
                        
                        Destroy(currentNode);
                        Destroy(currentNode.gameObject);

                        erasedNotes += 1;

                        StartCoroutine(FeelDestroyNode());
                    }
                    else
                    {
                        RestartGame();
                    }
                }
                
                pressX = false;
                pressY = false;
                pressZ = false;
                
                if (!usingBarre)
                {
                    StartCoroutine(FeelBarre());
                }
            }
            
        }
    }



    public void LaunchGame()
    {
        timer = 0;
        erasedNotes = 0;
            
        ReferenceManager.Instance.cameraReference.durationRythme = duration;

        StartCoroutine(StartGameFeel());
    }

    public IEnumerator StartGameFeel()
    {
        Image imageBarre =  barreAvancement.GetComponent<Image>();
        Image image = GetComponent<Image>();

        imageBarre.DOFade(0, 0);
        image.DOFade(0, 0);

        barreAvancement.DOMoveY(barreAvancement.position.y - 200, 0);
        fond.DOMoveY(fond.position.y - 200, 0);
        
        yield return new WaitForSeconds(0.01f);
        
        imageBarre.DOFade(1, 1.5f).SetEase(Ease.Linear);
        image.DOFade(0.9f, 1.5f).SetEase(Ease.Linear);
        
        barreAvancement.DOMoveY(barreAvancement.position.y + 200, 1.3f).SetEase(Ease.OutCubic);
        fond.DOMoveY(fond.position.y + 200, 1.3f).SetEase(Ease.OutCubic);
        
        yield return new WaitForSeconds(1.6f);

        gameStarted = true;
    }

    IEnumerator FeelBarre()
    {
        Vector3 originalScale = barreAvancement.localScale;
        float duration = 0.08f;
        
        usingBarre = true;
        barreAvancement.DOScale(barreAvancement.localScale * 1.2f, duration * 0.7f);
        
        yield return new WaitForSeconds(duration * 0.7f);
        
        barreAvancement.DOScale(originalScale, duration);
        
        yield return new WaitForSeconds(duration);
        
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
        
        ReferenceManager.Instance.cameraReference.DoCameraShake(0.4f, 0.2f);

        yield return new WaitForSeconds(duration);
    }


    public void RestartGame()
    {
        timer = -1.5f;
        erasedNotes = 0;

        StartCoroutine(LoseEffects());

        for(int i = 0; i < nodes.Count; i++)
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

        ReferenceManager.Instance.cameraReference.RestartMoveCameraRythme();
    }


    public void EndGame()
    {
        stop = true;

        ReferenceManager.Instance.characterReference.notesScript.NextBande();
    }


    public void UpdateTimer()
    {
        timer += Time.deltaTime;

        if (timer > duration)
        {
            EndGame();
        }

        for (int i = 0; i < nodes.Count; i++)
        {
            if (!nodes[i].isSpawned)
            {
                if (timer > nodes[i].spawnTiming)
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

                    newNode.InitialiseNode(nodes[i].nodeType, nodes[i].spawnPos, this);

                     nodes[i].isSpawned = true;
                     nodesCreated.Add(newNode);
                }
            }
        }

        for (int i = 0; i < nodesCreated.Count; i++)
        {
            nodesCreated[i].MoveNode(speedMoveNode);
        }
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

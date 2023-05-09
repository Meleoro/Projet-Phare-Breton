using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using DG.Tweening;


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
    public List<Node> nodes = new List<Node>();


    [Header("References")] 
    public GameObject nodeObject;
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
    [HideInInspector] public List<MusicNode> nodesErased = new List<MusicNode>();
    [HideInInspector] public List<MusicNode> nodesCreated = new List<MusicNode>();
    [HideInInspector] public bool stop;


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

                if (timer > dureePreparation && !startMoveBarre)
                {
                    startMoveBarre = true;
                }
            }

            if (pressX || pressY || pressZ)
            {
                if (currentNode != null)
                {
                    bool isRight = VerifyNote();

                    if (isRight)
                    {
                        currentNode.EraseNode();

                        nodesCreated.Remove(currentNode);
                        
                        Destroy(currentNode);
                        Destroy(currentNode.gameObject);
                    }
                }
                
                pressX = false;
                pressY = false;
                pressZ = false;
            }
        }
    }



    public void LaunchGame()
    {
        timer = 0;

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
        
        imageBarre.DOFade(1, 1.8f);
        image.DOFade(0.9f, 1.8f);
        
        barreAvancement.DOMoveY(barreAvancement.position.y + 200, 1.8f);
        fond.DOMoveY(fond.position.y + 200, 1.8f);
        
        yield return new WaitForSeconds(2);

        gameStarted = true;
    }


    public void RestartGame()
    {
        timer = 0;

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
    }


    public void EndGame()
    {
        stop = true;

        ReferenceManager.Instance.characterReference.notesScript.NextBande();

        GetComponent<RectTransform>().DOMoveY(GetComponent<RectTransform>().position.y - 50, 0.5f);
    }


    public void UpdateTimer()
    {
        timer += Time.deltaTime;

        for (int i = 0; i < nodes.Count; i++)
        {
            if (!nodes[i].isSpawned)
            {
                if (timer > nodes[i].spawnTiming)
                {
                     MusicNode newNode = Instantiate(nodeObject, transform.position, Quaternion.identity, transform).GetComponent<MusicNode>();
                     
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

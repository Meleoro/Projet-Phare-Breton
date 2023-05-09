using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class MusicNode : MonoBehaviour
{
    public BandeJeuDeRythme currentBande;

    [HideInInspector] public bool isX;
    [HideInInspector] public bool isY;
    [HideInInspector] public bool isZ;

    [HideInInspector] public bool erased;

    [Header("Movement")] 
    public AnimationCurve moveX;
    public AnimationCurve moveY;
    [HideInInspector] public Vector3 originPos;
    private float timer;
    private bool goLeft;

    [Header("References")]
    private Image image;

    private RectTransform rectTransform;


    private void Start()
    {
        image = GetComponent<Image>();
        rectTransform = GetComponent<RectTransform>();
    }


    public void InitialiseNode(Node.InputNeeded inputNeeded, Node.SpawnPos spawnPos, BandeJeuDeRythme bande)
    {
        rectTransform = GetComponent<RectTransform>();
        currentBande = bande;
        
        switch (inputNeeded)
        {
            case Node.InputNeeded.x :
                isX = true;
                break;
            
            case Node.InputNeeded.y :
                isY = true;
                break;
            
            case Node.InputNeeded.z :
                isZ = true;
                break;
        }
        
        switch (spawnPos)
        {
            case Node.SpawnPos.left :
                originPos = bande.posSpawnLeft.position;
                goLeft = false;
                break;
            
            case Node.SpawnPos.right :
                originPos = bande.posSpawnRight.position;
                goLeft = true;
                break;
        }
    }
    

    public void MoveNode(float speed)
    {
        timer += Time.deltaTime * speed;

        if(goLeft) 
            rectTransform.position = originPos + (new Vector3(-moveX.Evaluate(timer), moveY.Evaluate(timer), 0) * 225);
        
        else
            rectTransform.position = originPos + (new Vector3(moveX.Evaluate(timer), moveY.Evaluate(timer), 0) * 225);
    }
    

    public void EraseNode()
    {
        image.enabled = false;
        GetComponentInChildren<TextMeshProUGUI>().enabled = false;
        erased = true;
    }

    public void ReappearNode()
    {
        image.enabled = true;
        GetComponentInChildren<TextMeshProUGUI>().enabled = true;
        erased = false;
    }



    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("MusicBarre"))
        {
            currentBande.currentNode = this;

            if (isX)
                currentBande.isOnX = true;

            else if (isY)
                currentBande.isOnY = true;

            else
                currentBande.isOnZ = true;
        }
    }


    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("MusicBarre"))
        {
            currentBande.currentNode = null;

            if (isX)
                currentBande.isOnX = false;

            else if (isY)
                currentBande.isOnY = false;

            else
                currentBande.isOnZ = false;


            // Si le joueur n'a pas appuye 
            if (!erased)
            {
                currentBande.RestartGame();
            }
        }
    }
}

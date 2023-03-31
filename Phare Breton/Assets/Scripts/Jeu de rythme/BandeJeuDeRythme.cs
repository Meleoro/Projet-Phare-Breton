using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;


public class BandeJeuDeRythme : MonoBehaviour
{
    [Header("Infos Node Selectionnée")]
    [HideInInspector] public MusicNode currentNode;
    [HideInInspector] public bool isOnGreen;
    [HideInInspector] public bool isOnYellow;
    [HideInInspector] public bool isOnBlue;


    [Header("Parametres déroulement")]
    public float dureePreparation;
    public float speedMoveBarre;


    [Header("References")]
    public RectTransform barreAvancement;

    [Header("Inputs")]
    private bool pressX;
    private bool pressY;
    private bool pressZ;


    [Header("Autre")]
    private float timer;
    private bool gameStarted;
    private bool startMoveBarre;
    public List<MusicNode> nodesErased = new List<MusicNode>();
    private Vector3 originBarre;


    private void Start()
    {
        originBarre = barreAvancement.position;
    }


    private void Update()
    {
        if (gameStarted)
        {
            timer += Time.deltaTime;

            if(timer > dureePreparation && !startMoveBarre)
            {
                startMoveBarre = true;
            }
        }

        if (startMoveBarre)
        {
            barreAvancement.localPosition += Vector3.right * speedMoveBarre * Time.deltaTime;
        }

        if (pressX || pressY || pressZ)
        {
            if(currentNode != null)
            {
                bool isRight = VerifyNote();

                if (isRight)
                {
                    currentNode.EraseNode();
                    nodesErased.Add(currentNode);
                }

                else
                {
                    RestartGame();
                }

                pressX = false;
                pressY = false;
                pressZ = false;
            }

            else
            {
                RestartGame();
            }
        }
    }



    public void LaunchGame()
    {
        timer = 0;
        gameStarted = true;
    }


    public void RestartGame()
    {
        timer = 0;
        startMoveBarre = false;

        barreAvancement.position = originBarre;

        for(int i = 0; i < nodesErased.Count; i++)
        {
            nodesErased[i].ReappearNode();
        }

        nodesErased.Clear();

        isOnBlue = false;
        isOnGreen = false;
        isOnYellow = false;
        currentNode = null;
    }


    public bool VerifyNote()
    {
        if(pressX && isOnGreen)
        {
            return true;
        }

        else if(pressY && isOnYellow)
        {
            return true;
        }

        else if(pressZ && isOnBlue)
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

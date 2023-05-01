using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

public class PauseManager : MonoBehaviour
{
    [SerializeField] private RectTransform pauseObject;
    private bool pauseOpen;
    private int currentButton = 1;
    private bool isMoving;

    [Header("Références")] 
    [SerializeField] private List<TextMeshProUGUI> textsButtons = new List<TextMeshProUGUI>();

    [Header("Inputs")] 
    private bool pause;
    private bool interaction;
    private bool up;
    private bool down;


    private void Start()
    {
        QuitPause();
        currentButton = 1;
    }


    private void Update()
    {
        if (pauseOpen)
        {
            if ((up || down) && !isMoving)
            {
                ChangeSelected();

                up = false;
                down = false;
            }
        }
        
        if (pause)
        {
            if (pauseOpen)
            {
                QuitPause();
            }
            else
            {
                OpenPause();
            }

            pause = false;
        }
    }


    public void OpenPause()
    {
        pauseObject.gameObject.SetActive(true);
        ReferenceManager.Instance.characterReference.noControl = true;

        pauseOpen = true;
    }

    public void QuitPause()
    {
        pauseObject.gameObject.SetActive(false);
        ReferenceManager.Instance.characterReference.noControl = false;
        
        pauseOpen = false;
    }

    
    public void ChangeSelected()
    {
        if (up && currentButton > 1)
        {
            currentButton -= 1;
            
            GoUp();
        }
        
        else if (down && currentButton < textsButtons.Count)
        {
            currentButton += 1;
            
            GoDown();
        }
    }

    public void GoUp()
    {
        pauseObject.DOMoveY(pauseObject.position.y - 125, 0.5f).OnComplete((() => isMoving = false));

        isMoving = true;
    }

    public void GoDown()
    {
        pauseObject.DOMoveY(pauseObject.position.y + 125, 0.5f).OnComplete((() => isMoving = false));
        
        isMoving = true;
    }
    
    
    // ----------------------------------------------------------------------
    // INPUTS

    public void OnPause(InputAction.CallbackContext context)
    {
        if (context.started)
            pause = true;

        if (context.canceled)
            pause = false;
    }
    
    public void OnInteraction(InputAction.CallbackContext context)
    {
        if (context.started)
            interaction = true;

        if (context.canceled)
            interaction = false;
    }
    
    public void OnUp(InputAction.CallbackContext context)
    {
        if (context.started)
            up = true;

        if (context.canceled)
            up = false;
    }
    
    public void OnDown(InputAction.CallbackContext context)
    {
        if (context.started)
            down = true;

        if (context.canceled)
            down = false;
    }
}

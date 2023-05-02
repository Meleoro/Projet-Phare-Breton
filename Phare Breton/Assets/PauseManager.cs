using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    [SerializeField] private RectTransform pauseObject;
    private bool pauseOpen;
    private int currentButton = 1;
    private bool isMoving;

    [Header("Références")] 
    [SerializeField] private List<TextMeshProUGUI> textsButtons = new List<TextMeshProUGUI>();
    [SerializeField] private List<Image> notesImages = new List<Image>();

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

            if (interaction && !isMoving)
            {
                UseButton();
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
        
        textsButtons[currentButton - 1].DOFade(1, 0.5f).OnComplete((() => isMoving = false));
        textsButtons[currentButton - 1].transform.DOScale(new Vector3( 1.2f, 1.2f, 1.2f), 0.5f);
        textsButtons[currentButton - 1].transform.DOMoveX(textsButtons[currentButton - 1].rectTransform.position.x + 30, 0.5f);
        
        isMoving = true;
    }

    public void QuitPause()
    {
        pauseObject.gameObject.SetActive(false);
        ReferenceManager.Instance.characterReference.noControl = false;
        
        pauseOpen = false;
    }



    public void UseButton()
    {
        if (currentButton == 1)
        {
            QuitPause();
        }
        
        else if (currentButton == 2)
        {
            
        }

        else
        {
            SceneManager.LoadScene("MainMenu");
        }
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

        textsButtons[currentButton].DOFade(0.5f, 0.5f);
        textsButtons[currentButton].transform.DOScale(new Vector3( 1f, 1f, 1f), 0.5f);
        textsButtons[currentButton].transform.DOMoveX(textsButtons[currentButton].rectTransform.position.x - 30, 0.5f);
        
        textsButtons[currentButton - 1].DOFade(1, 0.5f);
        textsButtons[currentButton - 1].transform.DOScale(new Vector3( 1.2f, 1.2f, 1.2f), 0.5f);
        textsButtons[currentButton - 1].transform.DOMoveX(textsButtons[currentButton - 1].rectTransform.position.x + 30, 0.5f);
    }

    public void GoDown()
    {
        pauseObject.DOMoveY(pauseObject.position.y + 125, 0.5f).OnComplete((() => isMoving = false));
        
        isMoving = true;
        
        textsButtons[currentButton - 2].DOFade(0.5f, 0.5f);
        textsButtons[currentButton - 2].transform.DOScale(new Vector3( 1f, 1f, 1f), 0.5f);
        textsButtons[currentButton - 2].transform.DOMoveX(textsButtons[currentButton - 2].rectTransform.position.x - 30, 0.5f);
        
        textsButtons[currentButton - 1].DOFade(1, 0.5f);
        textsButtons[currentButton - 1].transform.DOScale(new Vector3( 1.2f, 1.2f, 1.2f), 0.5f);
        textsButtons[currentButton - 1].transform.DOMoveX(textsButtons[currentButton - 1].rectTransform.position.x + 30, 0.5f);
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

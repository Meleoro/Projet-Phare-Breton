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

    public float screenSizeMultiplicator;

    private float saveX;
    private float screenSize;

    public bool noControl;

    [Header("Références")] 
    [SerializeField] private List<TextMeshProUGUI> textsButtons = new List<TextMeshProUGUI>();
    [SerializeField] private List<Image> notesImages = new List<Image>();
    [SerializeField] private List<Image> bandesImages = new List<Image>();
    [SerializeField] private OptionsManager optionsScript;
    [SerializeField] private CheatsManager cheatScript;

    [Header("Inputs")] 
    private bool pause;
    private bool interaction;
    private bool up;
    private bool down;


    private void Start()
    {
        screenSize = ReferenceManager.Instance.cameraReference._camera.pixelWidth;

        saveX = textsButtons[currentButton - 1].rectTransform.position.x;

        QuitPause();
        currentButton = 1;
    }


    private void Update()
    {
        screenSize = ReferenceManager.Instance.cameraReference._camera.pixelWidth;

        if (!noControl)
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
                else if (!ReferenceManager.Instance.cameraReference.moveCameraRythme)
                {
                    OpenPause();
                }

                pause = false;
            }
        }
    }


    public void OpenPause()
    {
        pauseObject.gameObject.SetActive(true);
        ReferenceManager.Instance.characterReference.noControl = true;

        pauseOpen = true;
        
        textsButtons[currentButton - 1].DOFade(1, 0.5f).OnComplete((() => isMoving = false));
        textsButtons[currentButton - 1].transform.DOScale(new Vector3(1.3f, 1.3f, 1.3f), 0.3f);
        textsButtons[currentButton - 1].transform.DOMoveX(textsButtons[currentButton - 1].rectTransform.position.x + screenSize * screenSizeMultiplicator, 0.3f);

        notesImages[currentButton - 1].DOFade(1, 0.3f);
        notesImages[currentButton - 1].transform.DOScale(new Vector3(1.4f, 1.4f, 1.4f), 0.3f);

        isMoving = true;
    }

    public void QuitPause()
    {
        DOTween.KillAll();

        textsButtons[currentButton - 1].DOFade(0.5f, 0);
        textsButtons[currentButton - 1].transform.DOScale(new Vector3(1f, 1f, 1f), 0);
        textsButtons[currentButton - 1].transform.DOMoveX(saveX, 0);

        notesImages[currentButton - 1].DOFade(1, 0);
        notesImages[currentButton - 1].transform.DOScale(new Vector3(1, 1, 1), 0);

        pauseObject.gameObject.SetActive(false);
        ReferenceManager.Instance.characterReference.noControl = false;
        
        pauseOpen = false;
    }



    public void UseButton()
    {
        AudioManager.instance.PlaySoundOneShot(0, 5, 0, ReferenceManager.Instance.characterReference.playerAudioSource);
        
        if (currentButton == 1)
        {
            QuitPause();
        }
        
        else if (currentButton == 2)
        {
            StartCoroutine(optionsScript.OpenOptions(0.7f, 0.4f));
        }
        
        else if (currentButton == 3)
        {
            StartCoroutine(cheatScript.OpenCheats(0.7f, 0.4f));
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
            AudioManager.instance.PlaySoundOneShot(1, 5, 0, ReferenceManager.Instance.characterReference.playerAudioSource);
            
            currentButton -= 1;
            
            GoUp();
        }
        
        else if (down && currentButton < textsButtons.Count)
        {
            AudioManager.instance.PlaySoundOneShot(1, 5, 0, ReferenceManager.Instance.characterReference.playerAudioSource);
            
            currentButton += 1;
            
            GoDown();
        }
    }

    public void GoUp()
    {
        float duration = 0.3f;

        isMoving = true;
        saveX = textsButtons[currentButton - 1].rectTransform.position.x;

        // Textes
        textsButtons[currentButton].DOFade(0.4f, duration).OnComplete(() => isMoving = false);
        textsButtons[currentButton].transform.DOScale(new Vector3(1f, 1f, 1f), duration);
        textsButtons[currentButton].transform.DOMoveX(textsButtons[currentButton].rectTransform.position.x - screenSize * screenSizeMultiplicator, duration);

        textsButtons[currentButton - 1].DOFade(1, duration);
        textsButtons[currentButton - 1].transform.DOScale(new Vector3(1.3f, 1.3f, 1.3f), duration);
        textsButtons[currentButton - 1].transform.DOMoveX(textsButtons[currentButton - 1].rectTransform.position.x + screenSize * screenSizeMultiplicator, duration);


        // Images 
        notesImages[currentButton].DOFade(0.4f, duration);
        notesImages[currentButton].transform.DOScale(new Vector3(1f, 1f, 1f), duration);

        notesImages[currentButton - 1].DOFade(1, duration);
        notesImages[currentButton - 1].transform.DOScale(new Vector3(1.4f, 1.4f, 1.4f), duration);


        // Bandes
       /* bandesImages[currentButton].DOFade(0f, duration);

        bandesImages[currentButton - 1].DOFade(1, duration);*/
    }

    public void GoDown()
    {
        float duration = 0.3f;

        isMoving = true;
        saveX = textsButtons[currentButton - 1].rectTransform.position.x;

        // Textes
        textsButtons[currentButton - 2].DOFade(0.4f, duration).OnComplete(() => isMoving = false);
        textsButtons[currentButton - 2].transform.DOScale(new Vector3(1f, 1f, 1f), duration);
        textsButtons[currentButton - 2].transform.DOMoveX(textsButtons[currentButton - 2].rectTransform.position.x - screenSize * screenSizeMultiplicator, duration);

        textsButtons[currentButton - 1].DOFade(1, duration);
        textsButtons[currentButton - 1].transform.DOScale(new Vector3(1.3f, 1.3f, 1.3f), duration);
        textsButtons[currentButton - 1].transform.DOMoveX(textsButtons[currentButton - 1].rectTransform.position.x + screenSize * screenSizeMultiplicator, duration);


        // Images 
        notesImages[currentButton - 2].DOFade(0.4f, duration);
        notesImages[currentButton - 2].transform.DOScale(new Vector3(1f, 1f, 1f), duration);

        notesImages[currentButton - 1].DOFade(1, duration);
        notesImages[currentButton - 1].transform.DOScale(new Vector3(1.4f, 1.4f, 1.4f), duration);


        // Bandes
        /*bandesImages[currentButton - 2].DOFade(0f, duration);

        bandesImages[currentButton - 1].DOFade(1, duration);*/
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

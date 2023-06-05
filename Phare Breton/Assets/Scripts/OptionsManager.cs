using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class OptionsManager : MonoBehaviour
{
    [Header("Références")] 
    [SerializeField] private GameObject optionsObject;
    [SerializeField] private TextMeshProUGUI optionsMainText;
    [SerializeField] private List<TextMeshProUGUI> texts = new List<TextMeshProUGUI>();
    [SerializeField] private List<GameObject> sliders = new List<GameObject>();
    [SerializeField] private Image fond;
    [SerializeField] private MenuPrincManager menuPrincipalScript;
    [SerializeField] private PauseManager pauseScript;
    public EventSystem eventSystem;
    
    [Header("Inputs")] 
    private bool pause;
    private bool interaction;
    private bool up;
    private bool down;
    private bool escape;

    [Header("Autres")] 
    public int index = 0;
    public bool isOnMenuPrincipal;
    private bool canUse;
    private bool stopControl;
    
    private void Start()
    {
        eventSystem = GetComponentInChildren<EventSystem>();
        
        StartCoroutine(QuitOptions(0, 0));

        ActualiseSliders();
    }

    private void Update()
    {
        if (canUse)
        {
            if (down || up)
            {
                if (!stopControl)
                {
                    ChangeSelected();

                    down = false;
                    up = false;

                    StartCoroutine(StopControl());
                }
            }

            if (escape)
            {
                StartCoroutine(QuitOptions(1, 0));
            }
        }
    }
    
    
    IEnumerator StopControl()
    {
        stopControl = true;
        
        yield return new WaitForSeconds(0.3f);
        
        stopControl = false;
    }


    public void ChangeSelected()
    {
        if (up)
        {
            if (index - 1 >= 0)
            {
                index -= 1;
                
                GoUp(0.3f, 1);
            }
        }
        else
        {
            if (index + 1 < texts.Count)
            {
                index += 1;
                
                GoDown(0.3f, 1);
            }
        }
    }

    
    public void GoUp(float duration, float value)
    {
        texts[index + 1].DOFade(0.5f, duration);
        
        texts[index].DOFade(value, duration);
        
        
        Image[] sliderImages = sliders[index + 1].GetComponentsInChildren<Image>();

        for (int k = 0; k < sliderImages.Length; k++)
        {
            sliderImages[k].DOFade(0.5f, duration);
        }
        
        sliderImages = sliders[index].GetComponentsInChildren<Image>();

        for (int k = 0; k < sliderImages.Length; k++)
        {
            sliderImages[k].DOFade(value, duration);
        }
    }

    
    public void GoDown(float duration, float value)
    {
        texts[index - 1].DOFade(0.5f, duration);
        
        texts[index].DOFade(value, duration);
        
        
        Image[] sliderImages = sliders[index - 1].GetComponentsInChildren<Image>();

        for (int k = 0; k < sliderImages.Length; k++)
        {
            sliderImages[k].DOFade(0.5f, duration);
        }
        
        sliderImages = sliders[index].GetComponentsInChildren<Image>();

        for (int k = 0; k < sliderImages.Length; k++)
        {
            sliderImages[k].DOFade(value, duration);
        }
    }
    
    
    
    public IEnumerator OpenOptions(float duration, float value)
    {
        index = 0;
        
        eventSystem.enabled = false;
        
        optionsMainText.DOFade(1, duration);

        fond.DOFade(0, 0);
        fond.DOFade(1f, duration);


        for (int i = 0; i < texts.Count; i++)
        {
            texts[i].DOFade(0, 0);
            texts[i].DOFade(value, duration);
        }
        
        for (int i = 0; i < sliders.Count; i++)
        {
            Image[] sliderImages = sliders[i].GetComponentsInChildren<Image>();

            for (int k = 0; k < sliderImages.Length; k++)
            {
                sliderImages[k].DOFade(value, duration);
            }
        }

        optionsObject.SetActive(true);

        if(isOnMenuPrincipal)
            menuPrincipalScript.noControl = true;
        
        else
            pauseScript.noControl = true;
        
        GoUp(duration, 1);

        yield return new WaitForSeconds(duration);

        eventSystem.enabled = true;

        canUse = true;
    }

    public void ActualiseSliders()
    {
        sliders[0].GetComponent<Slider>().value = SaveManager.Instance.volumeMaster;
        sliders[1].GetComponent<Slider>().value = SaveManager.Instance.volumeMusic;
        sliders[2].GetComponent<Slider>().value = SaveManager.Instance.volumeSound;
    }

    public void ActualiseValue1()
    {
        SaveManager.Instance.volumeMaster = sliders[0].GetComponent<Slider>().value;
        
        SaveManager.Instance.SetupVolume();
    }

    public void ActualiseValue2()
    {
        SaveManager.Instance.volumeMusic = sliders[1].GetComponent<Slider>().value;
        
        SaveManager.Instance.SetupVolume();
    }

    public void ActualiseValue3()
    {
        SaveManager.Instance.volumeSound = sliders[2].GetComponent<Slider>().value;
        
        SaveManager.Instance.SetupVolume();
    }


    public IEnumerator QuitOptions(float duration, float value)
    {
        optionsObject.SetActive(true); 
        
        eventSystem.enabled = false;
        
        canUse = false;
        
        optionsMainText.DOFade(value, duration);

        fond.DOFade(0, duration);
        
        for (int i = 0; i < texts.Count; i++)
        {
            texts[i].DOFade(value, duration);
        }
        
        for (int i = 0; i < sliders.Count; i++)
        {
            Image[] sliderImages = sliders[i].GetComponentsInChildren<Image>();

            for (int k = 0; k < sliderImages.Length; k++)
            {
                sliderImages[k].DOFade(value, duration);
            }
        }

        yield return new WaitForSeconds(duration);
        
        eventSystem.enabled = true;
        
        optionsObject.SetActive(false);

        if (isOnMenuPrincipal)
            menuPrincipalScript.noControl = false;

        else
            pauseScript.noControl = false;
    }
    
    
    // ----------------------------------------------------------------------
    // INPUTS

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
    
    public void OnEscape(InputAction.CallbackContext context)
    {
        if (context.started)
            escape = true;

        if (context.canceled)
            escape = false;
    }
}

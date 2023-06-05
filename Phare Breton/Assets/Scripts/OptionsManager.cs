using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
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

    private void Start()
    {
        StartCoroutine(QuitOptions(0, 0));

        ActualiseSliders();
    }

    private void Update()
    {
        if (canUse)
        {
            if (down || up)
            {
                ChangeSelected();

                down = false;
                up = false;
            }

            if (escape)
            {
                StartCoroutine(QuitOptions(1, 0));
            }
        }
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

        canUse = true;
    }

    public void ActualiseSliders()
    {
        sliders[0].GetComponent<Slider>().value = AudioManager.instance.masterVolume;
        sliders[1].GetComponent<Slider>().value = AudioManager.instance.musicVolume;
        sliders[2].GetComponent<Slider>().value = AudioManager.instance.sfxVolume;
    }

    public void ActualiseValue1()
    {
        AudioManager.instance.masterVolume = sliders[0].GetComponent<Slider>().value;
    }

    public void ActualiseValue2()
    {
        AudioManager.instance.musicVolume = sliders[1].GetComponent<Slider>().value;
    }

    public void ActualiseValue3()
    {
        AudioManager.instance.sfxVolume = sliders[2].GetComponent<Slider>().value;
    }


    public IEnumerator QuitOptions(float duration, float value)
    {
        optionsObject.SetActive(true); 
        
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

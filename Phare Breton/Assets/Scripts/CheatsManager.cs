using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class CheatsManager : MonoBehaviour
{
    [Header("Références")] 
    [SerializeField] private GameObject cheatObject;
    [SerializeField] private TextMeshProUGUI cheatMainText;
    [SerializeField] private List<TextMeshProUGUI> texts = new List<TextMeshProUGUI>();
    [SerializeField] private MenuPrincManager menuPrincipalScript;
    [SerializeField] private PauseManager pauseScript;
    public Image fond;
    
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
        StartCoroutine(QuitCheats(0, 0));
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

            if (interaction)
            {
                Use();
                
                interaction = false;
            }

            if (escape)
            {
                StartCoroutine(QuitCheats(1, 0));
            }
        }
    }

    IEnumerator StopControl()
    {
        stopControl = true;
        
        yield return new WaitForSeconds(0.3f);
        
        stopControl = false;
    }

    public void Use()
    {
        if (index == 0)
        {
            SaveManager.Instance.TPZone1();
        }
        
        else if (index == 1)
        {
            SaveManager.Instance.TPZone21();
        }
        
        else if (index == 2)
        {
            SaveManager.Instance.TPZone22();
        }
        
        else if (index == 3)
        {
            SaveManager.Instance.TPZone31();
        }
        
        else if (index == 4)
        {
            SaveManager.Instance.TPZone32();
        }
        
        else if (index == 5)
        {
            ReferenceManager.Instance.characterReference.notesScript.GiveAllNotes();
        }
        
        else if (index == 6)
        {
            ReferenceManager.Instance.characterReference.canCable = true;
            ReferenceManager.Instance.characterReference.canStase = true;
            ReferenceManager.Instance.characterReference.canMoveObjects = true;
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
        texts[index + 1].DOFade(0.4f, duration);
        
        texts[index].DOFade(value, duration);
    }

    
    public void GoDown(float duration, float value)
    {
        texts[index - 1].DOFade(0.4f, duration);
        
        texts[index].DOFade(value, duration);
    }
    
    
    
    public IEnumerator OpenCheats(float duration, float value)
    {
        cheatObject.SetActive(true);

        index = 0;


        fond.DOFade(0, 0);
        fond.DOFade(1, 0);


        cheatMainText.DOFade(1, duration);
        
        for (int i = 0; i < texts.Count; i++)
        {
            texts[i].DOFade(0, 0);
            
            texts[i].DOFade(value, duration);
        }

        if(isOnMenuPrincipal)
            menuPrincipalScript.noControl = true;
        
        else
            pauseScript.noControl = true;
        
        GoUp(duration, 1);

        yield return new WaitForSeconds(duration);

        canUse = true;
    }

    
    public IEnumerator QuitCheats(float duration, float value)
    {
        cheatObject.SetActive(true); 
        
        canUse = false;
        
        fond.DOFade(0, duration);

        cheatMainText.DOFade(value, duration);
        
        for (int i = 0; i < texts.Count; i++)
        {
            texts[i].DOFade(value, duration);
        }

        yield return new WaitForSeconds(duration);

        cheatObject.SetActive(false);

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

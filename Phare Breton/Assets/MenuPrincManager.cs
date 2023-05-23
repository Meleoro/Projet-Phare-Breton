using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class MenuPrincManager : MonoBehaviour
{
    [SerializeField] private RectTransform menuObject;
    private int currentButton = 1;
    private bool isMoving;

    [Header("Références")] 
    [SerializeField] private List<TextMeshProUGUI> textsButtons = new List<TextMeshProUGUI>();
    [SerializeField] private List<Image> notesImages = new List<Image>();
    [SerializeField] private List<Image> bandesImages = new List<Image>();

    [Header("Inputs")] 
    private bool pause;
    private bool interaction;
    private bool up;
    private bool down;


    private void Start()
    {
        OpenMenu();
        currentButton = 1;
    }


    private void Update()
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


    public void OpenMenu()
    {
        float duration = 0.3f;
        
        menuObject.gameObject.SetActive(true);

        textsButtons[currentButton - 1].DOFade(1, duration).OnComplete((() => isMoving = false));
        textsButtons[currentButton - 1].transform.DOScale(new Vector3( 1.2f, 1.2f, 1.2f), duration);
        textsButtons[currentButton - 1].transform.DOMoveX(textsButtons[currentButton - 1].rectTransform.position.x + 30, duration);
        
        notesImages[currentButton - 1].DOFade(1, duration);
        notesImages[currentButton - 1].transform.DOScale(new Vector3( 1.2f, 1.2f, 1.2f), duration);
        notesImages[currentButton - 1].transform.DOMoveX(notesImages[currentButton - 1].rectTransform.position.x + 10, duration);
        
        isMoving = true;
    }



    public void UseButton()
    {
        if (currentButton == 1)
        {
            SceneManager.LoadScene("LevelDesign - BlockMesh");
        }
        
        else if (currentButton == 2)
        {
            
        }

        else
        {
            Application.Quit();
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
        float duration = 0.3f;
        
        menuObject.DOMoveY(menuObject.position.y, duration).OnComplete((() => isMoving = false));

        isMoving = true;

        // Textes
        textsButtons[currentButton].DOFade(0.4f, duration);
        textsButtons[currentButton].transform.DOScale(new Vector3( 1f, 1f, 1f), duration);
        textsButtons[currentButton].transform.DOMoveX(textsButtons[currentButton].rectTransform.position.x - 50, duration);

        textsButtons[currentButton - 1].DOFade(1, duration);
        textsButtons[currentButton - 1].transform.DOScale(new Vector3( 1.3f, 1.3f, 1.3f), duration);
        textsButtons[currentButton - 1].transform.DOMoveX(textsButtons[currentButton - 1].rectTransform.position.x + 50, duration);
        
        
        // Images 
        notesImages[currentButton].DOFade(0.6f, duration);
        notesImages[currentButton].transform.DOScale(new Vector3( 1f, 1f, 1f), duration);
        notesImages[currentButton].transform.DOMoveX(notesImages[currentButton].rectTransform.position.x - 10, duration);

        notesImages[currentButton - 1].DOFade(1, duration);
        notesImages[currentButton - 1].transform.DOScale(new Vector3( 1.2f, 1.2f, 1.2f), duration);
        notesImages[currentButton - 1].transform.DOMoveX(notesImages[currentButton - 1].rectTransform.position.x + 10, duration);
        
        
        // Bandes
        bandesImages[currentButton].DOFade(0f, duration);
        /*notesImages[currentButton].transform.DOScale(new Vector3( 1f, 1f, 1f), duration);
        notesImages[currentButton].transform.DOMoveX(notesImages[currentButton].rectTransform.position.x - 10, duration);*/

        bandesImages[currentButton - 1].DOFade(1, duration);
        /*notesImages[currentButton - 1].transform.DOScale(new Vector3( 1.2f, 1.2f, 1.2f), duration);
        notesImages[currentButton - 1].transform.DOMoveX(notesImages[currentButton - 1].rectTransform.position.x + 10, duration);*/
    }

    public void GoDown()
    {
        float duration = 0.3f;
        
        menuObject.DOMoveY(menuObject.position.y, duration).OnComplete((() => isMoving = false));
        
        isMoving = true;
        
        // Textes
        textsButtons[currentButton - 2].DOFade(0.4f, duration);
        textsButtons[currentButton - 2].transform.DOScale(new Vector3( 1f, 1f, 1f), duration);
        textsButtons[currentButton - 2].transform.DOMoveX(textsButtons[currentButton - 2].rectTransform.position.x - 50, duration);
        
        textsButtons[currentButton - 1].DOFade(1, duration);
        textsButtons[currentButton - 1].transform.DOScale(new Vector3( 1.3f, 1.3f, 1.3f), duration);
        textsButtons[currentButton - 1].transform.DOMoveX(textsButtons[currentButton - 1].rectTransform.position.x + 50, duration);
        
        
        // Images 
        notesImages[currentButton - 2].DOFade(0.6f, duration);
        notesImages[currentButton - 2].transform.DOScale(new Vector3( 1f, 1f, 1f), duration);
        notesImages[currentButton - 2].transform.DOMoveX(notesImages[currentButton - 2].rectTransform.position.x - 10, duration);
        
        notesImages[currentButton - 1].DOFade(1, duration);
        notesImages[currentButton - 1].transform.DOScale(new Vector3( 1.2f, 1.2f, 1.2f), duration);
        notesImages[currentButton - 1].transform.DOMoveX(notesImages[currentButton - 1].rectTransform.position.x + 10, duration);
        
        
        // Bandes
        bandesImages[currentButton - 2].DOFade(0f, duration);
        /*notesImages[currentButton - 2].transform.DOScale(new Vector3( 1f, 1f, 1f), duration);
        notesImages[currentButton - 2].transform.DOMoveX(notesImages[currentButton - 2].rectTransform.position.x - 10, duration);*/
        
        bandesImages[currentButton - 1].DOFade(1, duration);
        /*notesImages[currentButton - 1].transform.DOScale(new Vector3( 1.2f, 1.2f, 1.2f), duration);
        notesImages[currentButton - 1].transform.DOMoveX(notesImages[currentButton - 1].rectTransform.position.x + 10, duration);*/
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
}

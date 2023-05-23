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

    
    [Header("Shake")]
    public float amplitude;
    public float duration;
    private Vector3 wantedPos;
    private Vector2 modificateur;
    private Vector2 modificateur2;

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
        StartCoroutine(FindNewShakePos());
        StartCoroutine(FindNewShakePos2());
        
        currentButton = 1;
    }


    private void Update()
    {
        MoveBande();

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


    public void MoveBande()
    {
        menuObject.position = Vector3.Lerp(menuObject.position, wantedPos + new Vector3(modificateur.x + modificateur2.x, modificateur.y + modificateur2.y, 0), Time.deltaTime);
    }

    private IEnumerator FindNewShakePos()
    {
        modificateur = new Vector2(Random.Range(-amplitude, amplitude), Random.Range(-amplitude, amplitude));
        
        yield return new WaitForSeconds(duration);

        StartCoroutine(FindNewShakePos());
    }
    
    private IEnumerator FindNewShakePos2()
    {
        yield return new WaitForSeconds(duration / 2);
        
        modificateur2 = new Vector2(Random.Range(-amplitude, amplitude), Random.Range(-amplitude, amplitude));
        
        yield return new WaitForSeconds(duration / 2);

        StartCoroutine(FindNewShakePos2());
    }

    public void OpenMenu()
    {
        float duration = 0.3f;
        
        menuObject.gameObject.SetActive(true);

        wantedPos = menuObject.position;

        textsButtons[currentButton - 1].DOFade(1, duration).OnComplete((() => isMoving = false));
        textsButtons[currentButton - 1].transform.DOScale(new Vector3( 1.2f, 1.2f, 1.2f), duration);
        textsButtons[currentButton - 1].transform.DOMoveX(textsButtons[currentButton - 1].rectTransform.position.x + 50, duration);
        
        notesImages[currentButton - 1].DOFade(1, duration);
        notesImages[currentButton - 1].transform.DOScale(new Vector3( 1.4f, 1.4f, 1.4f), duration);

        bandesImages[currentButton - 1].DOFade(1, duration);
        
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

        wantedPos += Vector3.down * 10f;

        isMoving = true;

        // Textes
        textsButtons[currentButton].DOFade(0.4f, duration).OnComplete(() => isMoving = false);
        textsButtons[currentButton].transform.DOScale(new Vector3( 1f, 1f, 1f), duration);
        textsButtons[currentButton].transform.DOMoveX(textsButtons[currentButton].rectTransform.position.x - 50, duration);

        textsButtons[currentButton - 1].DOFade(1, duration);
        textsButtons[currentButton - 1].transform.DOScale(new Vector3( 1.3f, 1.3f, 1.3f), duration);
        textsButtons[currentButton - 1].transform.DOMoveX(textsButtons[currentButton - 1].rectTransform.position.x + 50, duration);
        
        
        // Images 
        notesImages[currentButton].DOFade(0.4f, duration);
        notesImages[currentButton].transform.DOScale(new Vector3( 1f, 1f, 1f), duration);
        //notesImages[currentButton].transform.DOMoveX(notesImages[currentButton].rectTransform.position.x - 10, duration);

        notesImages[currentButton - 1].DOFade(1, duration);
        notesImages[currentButton - 1].transform.DOScale(new Vector3( 1.4f, 1.4f, 1.4f), duration);
        //notesImages[currentButton - 1].transform.DOMoveX(notesImages[currentButton - 1].rectTransform.position.x + 10, duration);
        
        
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
        
        wantedPos -= Vector3.down * 10f;
        
        isMoving = true;
        
        // Textes
        textsButtons[currentButton - 2].DOFade(0.4f, duration).OnComplete(() => isMoving = false);
        textsButtons[currentButton - 2].transform.DOScale(new Vector3( 1f, 1f, 1f), duration);
        textsButtons[currentButton - 2].transform.DOMoveX(textsButtons[currentButton - 2].rectTransform.position.x - 50, duration);
        
        textsButtons[currentButton - 1].DOFade(1, duration);
        textsButtons[currentButton - 1].transform.DOScale(new Vector3( 1.3f, 1.3f, 1.3f), duration);
        textsButtons[currentButton - 1].transform.DOMoveX(textsButtons[currentButton - 1].rectTransform.position.x + 50, duration);
        
        
        // Images 
        notesImages[currentButton - 2].DOFade(0.4f, duration);
        notesImages[currentButton - 2].transform.DOScale(new Vector3( 1f, 1f, 1f), duration);
        //notesImages[currentButton - 2].transform.DOMoveX(notesImages[currentButton - 2].rectTransform.position.x - 10, duration);
        
        notesImages[currentButton - 1].DOFade(1, duration);
        notesImages[currentButton - 1].transform.DOScale(new Vector3( 1.4f, 1.4f, 1.4f), duration);
        //notesImages[currentButton - 1].transform.DOMoveX(notesImages[currentButton - 1].rectTransform.position.x + 10, duration);
        
        
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

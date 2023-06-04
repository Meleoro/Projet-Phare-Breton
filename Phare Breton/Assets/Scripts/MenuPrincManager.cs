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
    [SerializeField] private RectTransform fond;
    [SerializeField] private Camera _camera;
    private int currentButton = 1;
    private bool isMoving;
    public float multiplier1;
    public float multiplier2;
    public float multiplier3;
    private float screenWidth;
    private float screenHeight;
    public bool noControl;


    [Header("Shake")]
    public float amplitude;
    public float amplitudeRot;
    public float duration;
    private Vector3 wantedPos;
    /*private Vector2 modificateur;
    private Vector2 modificateur2;
    private float modificateurRot;
    private float modificateurRot2;*/
    private Vector3 posScroll;
    private Vector3 posModificateur;
    private float timerShake;
    private Vector2 currentWantedPosShake;
    private Vector2 wantedPosShake;
    private Vector3 originalPosFond;


    [Header("Références")] 
    [SerializeField] private List<TextMeshProUGUI> textsButtons = new List<TextMeshProUGUI>();
    [SerializeField] private List<Image> notesImages = new List<Image>();
    [SerializeField] private List<Image> bandesImages = new List<Image>();
    [SerializeField] private OptionsManager optionsManager;
    [SerializeField] private Image fondu;
    public AudioSource currentAudioSource;


    [Header("Fonds")]
    [SerializeField] private List<Image> fonds = new List<Image>();
    private int currentFond;
    private float fondTimer;


    [Header("Inputs")] 
    private bool pause;
    private bool interaction;
    private bool up;
    private bool down;
    private bool escape;


    private void Start()
    {
        screenWidth = _camera.pixelWidth;
        screenHeight = _camera.pixelHeight;

        fondu.DOFade(0, 1.5f);

        originalPosFond = fond.transform.position;


        OpenMenu();

        StartCoroutine(ShakeCoroutine());


        for(int i = 0; i < fonds.Count; i++)
        {
            fonds[i].DOFade(0, 0);
        }

        StartCoroutine(FondCoroutine());
        
        /*StartCoroutine(FindNewShakePos());
        StartCoroutine(FindNewShakePos2());*/
        
        currentButton = 1;
        
        AudioManager.instance.PlaySoundOneShot(3, 2, 0,currentAudioSource);

        SetupAudio();
    }

    private void SetupAudio()
    {
        AudioManager.instance.musicAudioSources.Clear();
        AudioManager.instance.musicAudioSources.Add(currentAudioSource);
    }


    private void Update()
    {
        screenWidth = _camera.pixelWidth;
        screenHeight = _camera.pixelHeight;

        MoveBande();

        if (!noControl)
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
    }


    public void MoveBande()
    {
        posScroll =  Vector3.Lerp(posScroll, wantedPos, Time.deltaTime * 1f);
        //posModificateur = Vector3.Lerp(posModificateur, new Vector3(modificateur.x + modificateur2.x, modificateur.y + modificateur2.y, 0), Time.deltaTime * 0.2f);
        
        menuObject.position = posScroll + posModificateur;
        fond.position = originalPosFond + posModificateur * 2;
        //menuObject.rotation = Quaternion.Euler(0,0, Mathf.Lerp(menuObject.rotation.z, modificateurRot + modificateurRot2, Time.deltaTime * 0.5f));
    }

    private IEnumerator FondCoroutine()
    {
        fondTimer -= Time.deltaTime;

        if(fondTimer <= 0)
        {
            fondTimer = 6;

            fonds[currentFond].DOFade(0, 1);

            int newIndex = currentFond;

            while(newIndex == currentFond)
            {
                newIndex = Random.Range(0, 7);
            }

            currentFond = newIndex;

            fonds[currentFond].DOFade(1, 1);

            fond = fonds[currentFond].rectTransform;
        }


        yield return new WaitForEndOfFrame();

        StartCoroutine(FondCoroutine());
    }

    private IEnumerator ShakeCoroutine()
    {
        timerShake += Time.deltaTime;

        if (timerShake > duration)
        {
            timerShake = 0;
            
            currentWantedPosShake = new Vector2(Random.Range(-amplitude * screenWidth * multiplier3, amplitude * screenWidth * multiplier3), Random.Range(-amplitude * screenHeight * multiplier3, amplitude * screenHeight * multiplier3));
        }
        
        wantedPosShake = Vector2.Lerp(wantedPosShake, currentWantedPosShake, Time.deltaTime);
        posModificateur = Vector3.Lerp(posModificateur, new Vector3(wantedPosShake.x, wantedPosShake.y, 0), Time.deltaTime * 0.2f);
        
        yield return new WaitForSeconds(Time.deltaTime);

        StartCoroutine(ShakeCoroutine());
    }
    
    
    /*private IEnumerator FindNewShakePos()
    {
        modificateur = new Vector2(Random.Range(-amplitude * screenWidth * multiplier3, amplitude * screenWidth * multiplier3), Random.Range(-amplitude * screenHeight * multiplier3, amplitude * screenHeight * multiplier3));
        modificateurRot = Random.Range(-amplitudeRot, amplitudeRot);
        
        yield return new WaitForSeconds(duration);

        StartCoroutine(FindNewShakePos());
    }
    
    private IEnumerator FindNewShakePos2()
    {
        yield return new WaitForSeconds(duration / 2);
        
        modificateur2 = new Vector2(Random.Range(-amplitude * screenWidth * multiplier3, amplitude * screenWidth * multiplier3), Random.Range(-amplitude * screenHeight * multiplier3, amplitude * screenHeight * multiplier3));
        modificateurRot2 = Random.Range(-amplitudeRot, amplitudeRot);
        
        yield return new WaitForSeconds(duration / 2);

        StartCoroutine(FindNewShakePos2());
    }*/

    public void OpenMenu()
    {
        float duration = 0.3f;
        
        menuObject.gameObject.SetActive(true);

        wantedPos = menuObject.position;
        posScroll = wantedPos;

        textsButtons[currentButton - 1].DOFade(1, duration).OnComplete((() => isMoving = false));
        textsButtons[currentButton - 1].transform.DOScale(new Vector3( 1.2f, 1.2f, 1.2f), duration);
        textsButtons[currentButton - 1].transform.DOMoveX(textsButtons[currentButton - 1].rectTransform.position.x + screenWidth * multiplier1, duration);
        
        notesImages[currentButton - 1].DOFade(1, duration);
        notesImages[currentButton - 1].transform.DOScale(new Vector3( 1.4f, 1.4f, 1.4f), duration);

        bandesImages[currentButton - 1].DOFade(1, duration);
        
        isMoving = true;
    }



    public void UseButton()
    {
        AudioManager.instance.PlaySoundOneShot(0, 5, 0, currentAudioSource);
        
        if (currentButton == 1)
        {
            StartCoroutine(CourtineStart());
        }
        
        else if (currentButton == 2)
        {
            StartCoroutine(optionsManager.OpenOptions(1, 0.5f));
        }

        else
        {
            Application.Quit();
        }
    }


    public IEnumerator CourtineStart()
    {
        fondu.DOFade(1, 0.8f);

        yield return new WaitForSeconds(1);

        SceneManager.LoadScene("LevelDesign - BlockMesh");
    }
    



    public void ChangeSelected()
    {
        if (up && currentButton > 1)
        {
            AudioManager.instance.PlaySoundOneShot(1, 5, 0, currentAudioSource);
            
            currentButton -= 1;
            
            GoUp();
        }
        
        else if (down && currentButton < textsButtons.Count)
        {
            AudioManager.instance.PlaySoundOneShot(1, 5, 0, currentAudioSource);
            
            currentButton += 1;
            
            GoDown();
        }
    }

    public void GoUp()
    {
        float duration = 0.3f;

        wantedPos += Vector3.down * (screenHeight * multiplier2);

        isMoving = true;

        // Textes
        textsButtons[currentButton].DOFade(0.4f, duration).OnComplete(() => isMoving = false);
        textsButtons[currentButton].transform.DOScale(new Vector3( 1f, 1f, 1f), duration);
        textsButtons[currentButton].transform.DOMoveX(textsButtons[currentButton].rectTransform.position.x - screenWidth * multiplier1, duration);

        textsButtons[currentButton - 1].DOFade(1, duration);
        textsButtons[currentButton - 1].transform.DOScale(new Vector3( 1.3f, 1.3f, 1.3f), duration);
        textsButtons[currentButton - 1].transform.DOMoveX(textsButtons[currentButton - 1].rectTransform.position.x + screenWidth * multiplier1, duration);
        
        
        // Images 
        notesImages[currentButton].DOFade(0.4f, duration);
        notesImages[currentButton].transform.DOScale(new Vector3( 1f, 1f, 1f), duration);

        notesImages[currentButton - 1].DOFade(1, duration);
        notesImages[currentButton - 1].transform.DOScale(new Vector3( 1.4f, 1.4f, 1.4f), duration);
        
        
        // Bandes
        bandesImages[currentButton].DOFade(0f, duration);

        bandesImages[currentButton - 1].DOFade(1, duration);
    }

    public void GoDown()
    {
        float duration = 0.3f;
        
        wantedPos -= Vector3.down * (screenHeight * multiplier2);
        
        isMoving = true;
        
        // Textes
        textsButtons[currentButton - 2].DOFade(0.4f, duration).OnComplete(() => isMoving = false);
        textsButtons[currentButton - 2].transform.DOScale(new Vector3( 1f, 1f, 1f), duration);
        textsButtons[currentButton - 2].transform.DOMoveX(textsButtons[currentButton - 2].rectTransform.position.x - screenWidth * multiplier1, duration);
        
        textsButtons[currentButton - 1].DOFade(1, duration);
        textsButtons[currentButton - 1].transform.DOScale(new Vector3( 1.3f, 1.3f, 1.3f), duration);
        textsButtons[currentButton - 1].transform.DOMoveX(textsButtons[currentButton - 1].rectTransform.position.x + screenWidth * multiplier1, duration);
        
        
        // Images 
        notesImages[currentButton - 2].DOFade(0.4f, duration);
        notesImages[currentButton - 2].transform.DOScale(new Vector3( 1f, 1f, 1f), duration);
        
        notesImages[currentButton - 1].DOFade(1, duration);
        notesImages[currentButton - 1].transform.DOScale(new Vector3( 1.4f, 1.4f, 1.4f), duration);
        
        
        // Bandes
        bandesImages[currentButton - 2].DOFade(0f, duration);
        
        bandesImages[currentButton - 1].DOFade(1, duration);
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

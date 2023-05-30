using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Rendering;


public class UINotes : MonoBehaviour
{
    public static UINotes Instance;

    [Header("Références")] 
    public Image note1;
    public Image note2;
    public Image note3;
    public Image fond;
    public RectTransform UIObject;
    public Volume damageVolume;

    [Header("Values")]
    public float noFade;
    public float yesFade;
    public float noScale;
    public float yesScale;
    public float modificateurX;
    public float modificateurShake;
    private float damageValue;

    [Header("Other")] 
    public List<bool> activatedNotes = new List<bool>();
    private float screenWidth;


    private void Awake()
    {
        Instance = this;
    }


    private void Start()
    {
        StartCoroutine(PutEverythingGray(true));

        damageValue = 0;
    }

    private void Update()
    {
        screenWidth = ReferenceManager.Instance.cameraReference._camera.pixelWidth;
    }


    public void GoLeft(float duration)
    {
        UIObject.DOMoveX(UIObject.position.x - screenWidth * modificateurX, duration);

        fond.DOFade(0.2f, duration);
    }

    public void GoRight(float duration)
    {
        UIObject.DOMoveX(UIObject.position.x + screenWidth * modificateurX, duration);
        
        fond.DOFade(0f, duration * 0.5f);
    }
    
    

    public IEnumerator GainNote(int index)
    {
        activatedNotes[index - 1] = true;
        
        float duration1 = 0.3f;
        float duration2 = 0.15f;
        float duration3 = 1;
        
        GoLeft(duration3);
        yield return new WaitForSeconds(duration3 + 0.5f);
        
        if (index == 1)
        {
            note1.DOFade(yesFade + 0.1f, duration1);

            note1.rectTransform.DOScale(Vector3.one * (yesScale + 0.2f), duration1);
            
            yield return new WaitForSeconds(duration1);

            note1.DOFade(yesFade, duration2);

            note1.rectTransform.DOScale(Vector3.one * (yesScale), duration2);
        }
        
        else if (index == 2)
        {
            note2.DOFade(yesFade + 0.1f, duration1);

            note2.rectTransform.DOScale(Vector3.one * (yesScale + 0.2f), duration1);
            
            yield return new WaitForSeconds(duration1);

            note2.DOFade(yesFade, duration2);

            note2.rectTransform.DOScale(Vector3.one * (yesScale), duration2);
        }

        else
        {
            note3.DOFade(yesFade + 0.1f, duration1);

            note3.rectTransform.DOScale(Vector3.one * (yesScale + 0.2f), duration1);
            
            yield return new WaitForSeconds(duration1);

            note3.DOFade(yesFade, duration2);

            note3.rectTransform.DOScale(Vector3.one * (yesScale), duration2);
        }
        
        yield return new WaitForSeconds(duration2 + 1);
        
        GoRight(duration3);
    }


    //PARTIE JDR
    public void StartGame()
    {
        GoLeft(1);
    }

    public IEnumerator LoseNote(int index)
    {
        float duration1 = 0.3f;

        DOTween.To(() => damageValue, x => damageValue = x, 1, 0.05f).OnUpdate(() =>
        {
            damageVolume.weight = damageValue;
        });

        if (index == 1)
        {
            note1.DOColor(Color.red, duration1);
            note1.rectTransform.DOShakePosition(duration1, screenWidth * modificateurShake);
        }
        else if(index == 2)
        {
            note2.DOColor(Color.red, duration1);
            note2.rectTransform.DOShakePosition(duration1, screenWidth * modificateurShake);
        }
        else
        {
            note3.DOColor(Color.red, duration1);
            note3.rectTransform.DOShakePosition(duration1, screenWidth * modificateurShake);
        }

        yield return new WaitForSeconds(0.05f);

        DOTween.To(() => damageValue, x => damageValue = x, 0, 0.2f).OnUpdate(() =>
        {
            damageVolume.weight = damageValue;
        });
    }

    public IEnumerator PutEverythingWhite()
    {
        float duration1 = 0.05f;
        float duration2 = 0.3f;
        float duration3 = 1;

        note1.DOColor(Color.white, duration2);
        note2.DOColor(Color.white, duration2);
        note3.DOColor(Color.white, duration2);

        note1.rectTransform.DOScale(Vector3.one * (yesScale + 0.1f), duration1);
        note2.rectTransform.DOScale(Vector3.one * (yesScale + 0.1f), duration1);
        note3.rectTransform.DOScale(Vector3.one * (yesScale + 0.1f), duration1);

        yield return new WaitForSeconds(0.1f);

        note1.rectTransform.DOScale(Vector3.one * (yesScale), duration2);
        note2.rectTransform.DOScale(Vector3.one * (yesScale), duration2);
        note3.rectTransform.DOScale(Vector3.one * (yesScale), duration2);

        GoRight(duration3);
    }


    public IEnumerator NoNotes()
    {
        float duration1 = 1f;
        float duration2 = 0.5f;

        GoLeft(duration1);
        yield return new WaitForSeconds(duration1 + 0.5f);

        for (int i = 0; i < activatedNotes.Count; i++)
        {
            if (!activatedNotes[i])
            {
                if (i == 0)
                {
                    note1.rectTransform.DOShakePosition(duration2, screenWidth * modificateurShake);
                }

                else if (i == 1)
                {
                    note2.rectTransform.DOShakePosition(duration2, screenWidth * modificateurShake);
                }

                else
                {
                    note3.rectTransform.DOShakePosition(duration2, screenWidth * modificateurShake);
                }
            }
        }

        yield return new WaitForSeconds(duration2 + 0.5f);

        GoRight(duration1);
    }

    public IEnumerator PutEverythingGray(bool start)
    {
        float duration1 = 0.05f;
        float duration2 = 0.3f;
        float duration3 = 1;

        for (int i = 0; i < activatedNotes.Count; i++)
        {
            activatedNotes[i] = false;
        }
        
        if (!start)
        {
            GoLeft(duration3);
            yield return new WaitForSeconds(duration3 + 0.5f);
        }
        
        note1.DOFade(yesFade + 0.1f, duration1);
        note2.DOFade(yesFade + 0.1f, duration1);
        note3.DOFade(yesFade + 0.1f, duration1);
        
        note1.rectTransform.DOScale(Vector3.one * (yesScale + 0.1f), duration1);
        note2.rectTransform.DOScale(Vector3.one * (yesScale + 0.1f), duration1);
        note3.rectTransform.DOScale(Vector3.one * (yesScale + 0.1f), duration1);

        yield return new WaitForSeconds(duration1);

        note1.DOFade(noFade, duration2);
        note2.DOFade(noFade, duration2);
        note3.DOFade(noFade, duration2);
        
        note1.rectTransform.DOScale(Vector3.one * (noScale), duration2);
        note2.rectTransform.DOScale(Vector3.one * (noScale), duration2);
        note3.rectTransform.DOScale(Vector3.one * (noScale), duration2);
        
        if (!start)
        {
            yield return new WaitForSeconds(duration2 + 0.5f);
            
            GoRight(duration3);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class Credits : MonoBehaviour
{
    public List<TextMeshProUGUI> texts = new List<TextMeshProUGUI>();
    private int currentIndex;

    public bool stop;
    
    void Start()
    {
        stop = true;
        currentIndex = 0;
        
        for (int i = 0; i < texts.Count; i++)
        {
            texts[i].DOFade(0, 0);
        }
    }
    
    void Update()
    {
        if (!stop)
        {
            float currentHauteur = ReferenceManager.Instance.characterReference.transform.position.y;

            if (texts[currentIndex].transform.position.y < currentHauteur)
            {
                if (currentIndex == 0)
                {
                    texts[currentIndex].DOFade(1, 1);

                    currentIndex = 1;
                }
            
                else if (currentIndex < texts.Count - 1)
                {
                    texts[currentIndex - 1].DOFade(0, 1);
                    texts[currentIndex].DOFade(1, 1);

                    currentIndex += 1;
                }

                if (currentIndex == texts.Count)
                {
                    stop = true;
                }
            }
        }
    }
}

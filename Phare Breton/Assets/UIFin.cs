using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class UIFin : MonoBehaviour
{
    public Image titre;
    public Image triangle;

    public RectTransform pos2;

    public List<TextMeshProUGUI> names = new List<TextMeshProUGUI>();



    private void Start()
    {
        titre.DOFade(0, 0.1f);
        triangle.DOFade(0, 0.1f);
    }



    public IEnumerator Credits()
    {
        titre.DOFade(1, 2);
        triangle.DOFade(1, 2);

        yield return new WaitForSeconds(3);

        triangle.rectTransform.DORotate(new Vector3(0, 0, 180), 2).SetEase(Ease.InOutSine);

        yield return new WaitForSeconds(2.5f);

        titre.rectTransform.DOMoveY(pos2.position.y, 1.5f);
        triangle.rectTransform.DOMoveY(pos2.position.y, 1.5f);

        yield return new WaitForSeconds(1.3f);

        for (int i = 0; i < names.Count; i++)
        {
            names[i].DOFade(1, 2);
        }
        
        yield return new WaitForSeconds(10);
    }
}

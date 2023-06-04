using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UIFin : MonoBehaviour
{
    public Image titre;
    public Image triangle;


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

        yield return new WaitForSeconds(3);
    }
}

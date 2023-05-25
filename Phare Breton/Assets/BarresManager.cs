using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BarresManager : MonoBehaviour
{
    public static BarresManager Instance;
    
    public List<GameObject> barres = new List<GameObject>();


    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        
        else
            Destroy(gameObject);
    }


    public void MoveBarres()
    {
        for (int i = 0; i < barres.Count; i++)
        {
            if (i % 2 == 0)
            {
                barres[i].transform.DOMoveZ( barres[i].transform.position.z + 10, 2).SetEase(Ease.InCubic);
            }

            else
            {
                barres[i].transform.DOMoveZ( barres[i].transform.position.z - 10, 2).SetEase(Ease.InCubic);
            }
        }
    }
}

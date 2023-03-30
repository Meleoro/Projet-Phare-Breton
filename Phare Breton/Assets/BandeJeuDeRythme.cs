using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BandeJeuDeRythme : MonoBehaviour
{
    [Header("Infos Node Selectionnée")]
    public GameObject currentNode;
    public bool isOnGreen;
    public bool isOnYellow;
    public bool isOnBlue;


    [Header("Parametres déroulement")]
    public float dureePreparation;
    public float speedMoveBarre;


    [Header("References")]
    public RectTransform barreAvancement;


    [Header("Autre")]
    private float timer;
    private bool gameStarted;
    private bool startMoveBarre;



    private void Update()
    {
        if (gameStarted)
        {
            timer += Time.deltaTime;

            if(timer > dureePreparation && !startMoveBarre)
            {
                startMoveBarre = true;
            }
        }

        if (startMoveBarre)
        {
            barreAvancement.localPosition += Vector3.right * speedMoveBarre * Time.deltaTime;
        }
    }



    public void LaunchGame()
    {
        timer = 0;
        gameStarted = true;
    }
}

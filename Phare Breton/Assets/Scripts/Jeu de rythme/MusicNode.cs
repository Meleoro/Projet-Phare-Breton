using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class MusicNode : MonoBehaviour
{
    public BandeJeuDeRythme currentBande;

    public bool isGreen;
    public bool isYellow;
    public bool isBlue;

    [HideInInspector] public bool erased;

    [Header("Références")]
    private Image image;


    private void Start()
    {
        image = GetComponent<Image>();
    }


    public void EraseNode()
    {
        image.enabled = false;
        GetComponentInChildren<TextMeshProUGUI>().enabled = false;
        erased = true;
    }

    public void ReappearNode()
    {
        image.enabled = true;
        GetComponentInChildren<TextMeshProUGUI>().enabled = true;
        erased = false;
    }



    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("MusicBarre"))
        {
            currentBande.currentNode = this;

            if (isGreen)
                currentBande.isOnGreen = true;

            else if (isYellow)
                currentBande.isOnYellow = true;

            else
                currentBande.isOnBlue = true;
        }
    }


    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("MusicBarre"))
        {
            currentBande.currentNode = null;

            if (isGreen)
                currentBande.isOnGreen = false;

            else if (isYellow)
                currentBande.isOnYellow = false;

            else
                currentBande.isOnBlue = false;


            // Si le joueur n'a pas appuyé 
            if (!erased)
            {
                currentBande.RestartGame();
            }
        }
    }
}

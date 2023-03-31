using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicNode : MonoBehaviour
{
    public BandeJeuDeRythme currentBande;

    public bool isGreen;
    public bool isYellow;
    public bool isBlue;



    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("MusicBarre"))
        {
            currentBande.currentNode = gameObject;

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
        }
    }
}

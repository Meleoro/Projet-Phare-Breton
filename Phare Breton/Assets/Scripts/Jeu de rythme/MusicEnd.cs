using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicEnd : MonoBehaviour
{
    public BandeJeuDeRythme currentBande;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("MusicBarre"))
        {
            collision.enabled = false;
            GetComponent<Collider2D>().enabled = false;

            currentBande.EndGame();
        }
    }
}

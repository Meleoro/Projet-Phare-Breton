using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteManager : MonoBehaviour
{
    public List<Melody> Melodies;

    [Range(1, 3)] public int currentMelody; 
}




[Serializable]
public class Melody
{
    public int nbrNotes;

    public List<GameObject> bandes;
}

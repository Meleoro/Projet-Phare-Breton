using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;

    public bool goMiddle;

    public string scene1Name;
    public string scene2Name;
    public string scene3Name;
    

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        
        else
            Destroy(gameObject);
        
        DontDestroyOnLoad(transform.gameObject);
    }


    public void TPZone1()
    {
        SceneManager.LoadScene(scene1Name);
    }

    public void TPZone21()
    {
        goMiddle = false;
        
        SceneManager.LoadScene(scene2Name);
    }

    public void TPZone22()
    {
        goMiddle = true;

        SceneManager.LoadScene(scene2Name);
    }

    public void TPZone31()
    {
        goMiddle = false;
        
        SceneManager.LoadScene(scene3Name);
    }

    public void TPZone32()
    {
        goMiddle = true;
        
        SceneManager.LoadScene(scene3Name);
    }
}

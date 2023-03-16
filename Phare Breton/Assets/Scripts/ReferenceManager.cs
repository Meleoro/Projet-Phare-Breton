using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReferenceManager : MonoBehaviour
{
    public static ReferenceManager Instance;

    [Header("References")]
    public GameObject cameraReference;
    public GameObject cameraRotationReference;
    public CharaManager characterReference;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        else
            Destroy(gameObject);
    }
}

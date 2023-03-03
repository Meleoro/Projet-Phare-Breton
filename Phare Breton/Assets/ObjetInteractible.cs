using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjetInteractible : MonoBehaviour
{
    [SerializeField] Material notSelectedMaterial;
    [SerializeField] Material selectedMaterial;

    private MeshRenderer mesh;


    private void Start()
    {
        mesh = GetComponent<MeshRenderer>();
    }


    public void Select()
    {
        mesh.material = selectedMaterial;
    }

    public void Deselect()
    {
        mesh.material = notSelectedMaterial;
    }
}

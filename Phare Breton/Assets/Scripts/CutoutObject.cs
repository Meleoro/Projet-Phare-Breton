using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutoutObject : MonoBehaviour
{
    [SerializeField] private Transform targetObject;
    [SerializeField] private LayerMask wallMask;
    private Camera mainCamera;

    [Header("ShaderTransparence")] 
    public float cutoutSize = 0.1f;
    public float fallOffSize = 0.05f;
    private Material[] materials;
    private List<Material> globalMaterials = new List<Material>();

    
    void Start()
    {
        mainCamera = GetComponent<Camera>();
    }
    
    void Update()
    {
        Vector2 cutoutPos = mainCamera.WorldToViewportPoint(targetObject.position);
        //cutoutPos.y /= (Screen.width / Screen.height);

        Vector3 offset = targetObject.position - transform.position;
        RaycastHit[] hitObjects = Physics.RaycastAll(transform.position, offset, offset.magnitude, wallMask);

        for (int i = 0; i < hitObjects.Length; i++)
        {
            materials = hitObjects[i].transform.GetComponent<Renderer>().materials;

            for (int j = 0; j < materials.Length; j++)
            {
                if(!globalMaterials.Contains(materials[j]))
                    globalMaterials.Add(materials[j]);
            }
        }

        for(int i = 0; i < globalMaterials.Count; i++)
        {
            globalMaterials[i].SetVector("_CutoutPos", cutoutPos);
            globalMaterials[i].SetFloat("_CutoutSize", cutoutSize);
            globalMaterials[i].SetFloat("_FalloffSize", fallOffSize);
        }
    }


    public void ResetAlphas()
    {
        for (int j = 0; j < globalMaterials.Count; j++)
        {
            globalMaterials[j].SetVector("_CutoutPos", Vector2.zero);
            globalMaterials[j].SetFloat("_CutoutSize", 0);
            globalMaterials[j].SetFloat("_FalloffSize", 0);
        }

        globalMaterials.Clear();
    }
}

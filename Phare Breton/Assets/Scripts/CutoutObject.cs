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
        cutoutPos.y /= (Screen.width / Screen.height);

        Vector3 offset = targetObject.position - transform.position;
        RaycastHit[] hitObjects = Physics.RaycastAll(transform.position, offset, offset.magnitude, wallMask);

        for (int j = 0; j < globalMaterials.Count; j++)
        {
            globalMaterials[j].SetVector("_CutoutPos", Vector2.zero);
            globalMaterials[j].SetFloat("_CutoutSize", 0);
            globalMaterials[j].SetFloat("_FalloffSize", 0);
        }

        globalMaterials.Clear();

        for (int i = 0; i < hitObjects.Length; i++)
        {
            materials = hitObjects[i].transform.GetComponent<Renderer>().materials;

            for (int j = 0; j < materials.Length; j++)
            {
                globalMaterials.Add(materials[j]);

                materials[j].SetVector("_CutoutPos", cutoutPos);
                materials[j].SetFloat("_CutoutSize", cutoutSize);
                materials[j].SetFloat("_FalloffSize", fallOffSize);
            }
        }
    }
}

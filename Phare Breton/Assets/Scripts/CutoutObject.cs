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
    public float sphereSize = 5;
    public bool debug;
    private Material[] materials;
    private List<RaycastHit[]> hitObjectsArrays = new List<RaycastHit[]>();
    private List<Material> globalMaterials = new List<Material>();

    
    void Start()
    {
        mainCamera = GetComponent<Camera>();
    }
    
    void Update()
    {
        Vector2 cutoutPos = mainCamera.WorldToViewportPoint(targetObject.position);
        //cutoutPos.y /= (Screen.width / Screen.height);

        Vector3 offset = targetObject.position - transform.position + Vector3.up;
        
        ResetAlphas();

        RaycastHit[] hitObjects = Physics.RaycastAll(transform.position, offset, offset.magnitude, wallMask);
        hitObjectsArrays.Add(hitObjects);
        
        /*hitObjects = Physics.RaycastAll(transform.position + Vector3.right * sphereSize, offset, offset.magnitude, wallMask);
        hitObjectsArrays.Add(hitObjects);
        
        hitObjects = Physics.RaycastAll(transform.position + Vector3.left * sphereSize, offset, offset.magnitude, wallMask);
        hitObjectsArrays.Add(hitObjects);
        
        hitObjects = Physics.RaycastAll(transform.position + Vector3.back * sphereSize, offset, offset.magnitude, wallMask);
        hitObjectsArrays.Add(hitObjects);
        
        hitObjects = Physics.RaycastAll(transform.position + Vector3.forward * sphereSize, offset, offset.magnitude, wallMask);
        hitObjectsArrays.Add(hitObjects);
        
        hitObjects = Physics.RaycastAll(transform.position + new Vector3(0.7f, 0, 0.7f) * sphereSize , offset, offset.magnitude, wallMask);
        hitObjectsArrays.Add(hitObjects);
        
        hitObjects = Physics.RaycastAll(transform.position + new Vector3(0.7f, 0, -0.7f) * sphereSize, offset, offset.magnitude, wallMask);
        hitObjectsArrays.Add(hitObjects);
        
        hitObjects = Physics.RaycastAll(transform.position +  new Vector3(-0.7f, 0, 0.7f) * sphereSize, offset, offset.magnitude, wallMask);
        hitObjectsArrays.Add(hitObjects);
        
        hitObjects = Physics.RaycastAll(transform.position +  new Vector3(-0.7f, 0, -0.7f) * sphereSize, offset, offset.magnitude, wallMask);
        hitObjectsArrays.Add(hitObjects);*/
        
        /*if (debug)
        {
            Debug.DrawLine(transform.position, transform.position + offset);
            
            Debug.DrawLine(transform.position + Vector3.right * sphereSize, transform.position + Vector3.right * sphereSize + offset);
            Debug.DrawLine(transform.position + Vector3.left * sphereSize, transform.position + Vector3.left * sphereSize + offset);
            Debug.DrawLine(transform.position + Vector3.back * sphereSize, transform.position + Vector3.back * sphereSize + offset);
            Debug.DrawLine(transform.position + Vector3.forward * sphereSize, transform.position + Vector3.forward * sphereSize + offset);
            
            Debug.DrawLine(transform.position + new Vector3(0.7f, 0, 0.7f) * sphereSize, transform.position + new Vector3(0.7f, 0, 0.7f) * sphereSize + offset);
            Debug.DrawLine(transform.position + new Vector3(0.7f, 0, -0.7f) * sphereSize, transform.position + new Vector3(0.7f, 0, -0.7f) * sphereSize + offset);
            Debug.DrawLine(transform.position + new Vector3(-0.7f, 0, 0.7f) * sphereSize, transform.position + new Vector3(-0.7f, 0, 0.7f) * sphereSize + offset);
            Debug.DrawLine(transform.position + new Vector3(-0.7f, 0, -0.7f) * sphereSize, transform.position + new Vector3(-0.7f, 0, -0.7f) * sphereSize + offset);
        }*/
        
        for (int k = 0; k < hitObjectsArrays.Count; k++)
        {
            for (int i = 0; i < hitObjectsArrays[k].Length; i++)
            {
                materials = hitObjectsArrays[k][i].transform.GetComponent<Renderer>().materials;

                for (int j = 0; j < materials.Length; j++)
                {
                    globalMaterials.Add(materials[j]);
                }
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

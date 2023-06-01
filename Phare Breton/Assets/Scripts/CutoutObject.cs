using System;
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
    private Material[] materials;
    private List<RaycastHit[]> hitObjectsArrays = new List<RaycastHit[]>();
    public List<GlobalMaterial> globalMaterials = new List<GlobalMaterial>();

    private int index = 0;
    private int index2 = 0;
    private List<GameObject> allHitObjects = new List<GameObject>();

    private List<RaycastHit[]> hitObjectsSides = new List<RaycastHit[]>();
    private RaycastHit[] hitObjects;


    void Start()
    {
        mainCamera = GetComponent<Camera>();
        
        globalMaterials.Add(new GlobalMaterial());

        for (int i = 0; i < 8; i++)
        {
            globalMaterials.Add(new GlobalMaterial());
            
            hitObjectsSides.Add(new RaycastHit[0]);
        }
    }


    
    void Update()
    {
        Vector3 offset = ReferenceManager.Instance.characterReference.movedObjectPosition - transform.position;
        
        Vector2 cutoutPos = mainCamera.WorldToViewportPoint(ReferenceManager.Instance.characterReference.movedObjectPosition);
        //cutoutPos.y /= (Screen.width / Screen.height);
            
        ResetAlphas(0);

        Vector3 pos1 = transform.position - new Vector3(offset.x, 0, offset.z).normalized * 7;
        Vector3 pos2 = ReferenceManager.Instance.characterReference.movedObjectPosition - new Vector3(offset.x, 0, offset.z).normalized * 14;
        
        Debug.DrawLine(pos1 + Vector3.up * 6, pos2 + Vector3.up * 6);
        
        hitObjects = Physics.CapsuleCastAll(pos1 + Vector3.up * 6, pos2 + Vector3.up * 6, 5, offset, 0, wallMask);

        for (int i = 0; i < hitObjects.Length; i++)
        {
            materials = hitObjects[i].transform.GetComponent<Renderer>().materials;

            for (int j = 0; j < materials.Length; j++)
            {
                globalMaterials[0].materials.Add(materials[j]);
            }
        }
        
        for(int i = 0; i < globalMaterials[index].materials.Count; i++)
        {
            globalMaterials[index].materials[i].SetVector("_CutoutPos", cutoutPos);
            globalMaterials[index].materials[i].SetFloat("_CutoutSize", cutoutSize);
            globalMaterials[index].materials[i].SetFloat("_FalloffSize", fallOffSize);
        }
            
    }


    public void ResetAlphas(int index)
    {
        if(index == 0)
            allHitObjects.Clear();
        
        for (int j = 0; j < globalMaterials[index].materials.Count; j++)
        {
            globalMaterials[index].materials[j].SetVector("_CutoutPos", Vector2.zero);
            globalMaterials[index].materials[j].SetFloat("_CutoutSize", 0);
            globalMaterials[index].materials[j].SetFloat("_FalloffSize", 0);
        }

        globalMaterials[index].materials.Clear();
    }
}


[Serializable]
public class GlobalMaterial
{
    public List<Material> materials = new List<Material>();
}

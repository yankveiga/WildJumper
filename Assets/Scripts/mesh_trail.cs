using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Mesh_trail : MonoBehaviour
{
    private player_controller mind;
    public float activeTime = 2f;
    [Header("Mesh Related")]
    
    public float meshRefreshRate = 0.1f;
    public float meshDestroyDelay = 3f;
    public Transform positionToSpawn;
    [Header("Shader Related")]
    public Material mat;
    public string shaderVarRef;
    public float shaderVarRate = 0.1f;
    public float shaderVarRefresh = 0.05f;



    private SkinnedMeshRenderer[] skinnedMeshRenderers;
    private bool isTrailActive;
    private void Start()
    {
        mind = GetComponent<player_controller>();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !isTrailActive) 
        {
            isTrailActive = true;
            StartCoroutine(ActivateTrail(activeTime));
            Debug.Log(mind.media_attention);

        }
         
        if((mind.media_attention >= 80 ) && !isTrailActive)
        {
            meshRefreshRate = 0.1f;
            isTrailActive = true;
            StartCoroutine(ActivateTrail(activeTime));
            

        }
        else if  ((mind.media_attention >= 60) && !isTrailActive)
        {
            meshRefreshRate = 1f;
            isTrailActive = true;
            StartCoroutine(ActivateTrail(activeTime));
        }
    }
    IEnumerator ActivateTrail(float timeActive)
    {
        while (timeActive > 0)
        {
            timeActive -= meshRefreshRate; 

            if (skinnedMeshRenderers == null)
               skinnedMeshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
           
            for(int i = 0; i < skinnedMeshRenderers.Length; i++)
            {
                GameObject gObj = new GameObject();
                gObj.transform.SetPositionAndRotation(positionToSpawn.position,positionToSpawn.rotation);


                MeshRenderer mr = gObj.AddComponent<MeshRenderer>();
                MeshFilter mf =  gObj.AddComponent<MeshFilter>();

                Mesh mesh = new Mesh();
                skinnedMeshRenderers[i].BakeMesh(mesh);

                mf.mesh = mesh;
                mr.material = mat;
                
                StartCoroutine(AnimateMaterialFloat(mr.material, 0 , shaderVarRate ,shaderVarRefresh ));

                Destroy(gObj, meshDestroyDelay);
            }



            yield return new WaitForSeconds(meshRefreshRate);
        }
        isTrailActive = false;
        
  
    }

    IEnumerator AnimateMaterialFloat(Material mat,float goal,float rate,float refreshrate) 
    {
        float valueToanimate = mat.GetFloat(shaderVarRef);

        while (valueToanimate > goal) 
        {
            valueToanimate -= rate;
            mat.SetFloat(shaderVarRef, valueToanimate);
            yield return new WaitForSeconds (refreshrate);

        }
    
    }


}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class zNubVisual : MonoBehaviour {
  private static Mesh m_sphereMesh;
  private static Mesh m_cubeMesh;
  public string m_visualType;

  public void init(Visual _vis) {
    Debug.Log("[zNubVisual:init] Called");

    // set nub name, if valid, otherwise the id
    Mesh mesh = GetComponent<MeshFilter>().mesh;
        GetComponent<MeshFilter>().mesh = m_sphereMesh;
        gameObject.AddComponent<SphereCollider>();
/*    switch (_nubData.prim)
    {
      case "sphere":
        GetComponent<MeshFilter>().mesh = m_sphereMesh;
        gameObject.AddComponent<SphereCollider>();
        break;

      case "cube":
        GetComponent<MeshFilter>().mesh = m_cubeMesh;
        gameObject.AddComponent<BoxCollider>();
        break;

      default:
        Debug.Log("[zNubVisual:init] UNKNOWN mesh type:" + _nubData.prim);
        break;
    }
    */
  }

  void Awake() {
    Debug.Log("[zNubVisual:Awake] Called");
    // setup the sphere mesh if not already done
    if(m_sphereMesh == null) {
      GameObject sphereObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
      m_sphereMesh = sphereObj.GetComponent<MeshFilter>().sharedMesh;
      Destroy(sphereObj);
      Debug.Assert(m_sphereMesh != null, "Unable to get sphere mesh");
    }
    if(m_cubeMesh == null) {
      GameObject cubeObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
      m_cubeMesh = cubeObj.GetComponent<MeshFilter>().sharedMesh;
      Destroy(cubeObj);
      Debug.Assert(m_cubeMesh != null, "Unable to get cube mesh");
    }
  }

  void Start() {
    Debug.Log("[zNubVisual:Start] Called");
    //GetComponent<MeshFilter>().mesh = m_sphereMesh;
  }
}

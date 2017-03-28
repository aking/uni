using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NubInfo : MonoBehaviour {
  private static Mesh m_sphereMesh;
  private static Mesh m_cubeMesh;
  public string m_id;

  public void init(Nub _nubData) {
    Debug.Log("[NubInfo:init] Called");

    gameObject.name = _nubData.name;
    Mesh mesh = GetComponent<MeshFilter>().mesh;
    switch (_nubData.prim)
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
        Debug.Log("[NubInfo:init] UNKNOWN mesh type:" + _nubData.prim);
        break;
    }
  }

  void Awake() {
    Debug.Log("[NubInfo:Awake] Called");
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
    Debug.Log("[NubInfo:Start] Called");
    //GetComponent<MeshFilter>().mesh = m_sphereMesh;
  }
}

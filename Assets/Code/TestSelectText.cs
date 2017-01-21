using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TestSelectText : MonoBehaviour, IPointerDownHandler {

  public Camera m_camera;
  private TextMesh m_text;

	// Use this for initialization
	void Start () {
    m_text = GetComponent<TextMesh>();

    if(m_text != null) {
      print("[TST:Start] FOUND");
    } else {
      print("[TST:Start] NOT FOUND");
    }
	}
	
	// Update is called once per frame
	void Update () {
    if (Input.GetMouseButtonDown(0)){ // if left button pressed...
      Debug.Log("HIT:" + Input.mousePosition);
      Ray ray = m_camera.ScreenPointToRay(Input.mousePosition);
      RaycastHit hit;
      if (Physics.Raycast(ray, out hit)){

        print("clicked:" + hit.triangleIndex + ":" + hit.point + ":" + hit);

      }
      else
      {

    Debug.Log("[TESTSELECTTEXT:Update] NO HIT");
      }
    }
		
    var text = GetComponent<Text>();
    if(text != null) {
      transform.Rotate(Vector3.up * Time.deltaTime*10.0f);
    }
	}

  public void OnPointerDown(PointerEventData _eventData) {
    Debug.Log("[TESTSELECTTEXT:OnPointerDown]");
  }

  void OnDrawGizmos()
  {
    var text = GetComponent<Text>();
    if(text == null)
      return;
    
    var textGen = text.cachedTextGenerator;
    var prevMatrix = Gizmos.matrix;
    Gizmos.matrix = transform.localToWorldMatrix;
    for (int i = 0; i < textGen.characterCount; ++i)
    {
      Vector2 locUpperLeft = new Vector2(textGen.verts[i * 4].position.x, textGen.verts[i * 4].position.y);
      Vector2 locBottomRight = new Vector2(textGen.verts[i * 4 + 2].position.x, textGen.verts[i * 4 + 2].position.y);

      Vector3 mid = (locUpperLeft + locBottomRight) / 2.0f;
      Vector3 size = locBottomRight - locUpperLeft;

      Gizmos.DrawWireCube(mid, size);
    }
    Gizmos.matrix = prevMatrix;
  }
}

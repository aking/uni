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

    /*
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
	*/	
    var text = GetComponent<Text>();
    if(text != null) {
      //transform.Rotate(Vector3.up * Time.deltaTime*10.0f);
    }
	}

  public void OnPointerDown(PointerEventData _eventData) {
    Debug.Log("[TESTSELECTTEXT:OnPointerDown]");
    _eventData.Use();

    int index = getIndexOfClick(_eventData.pressEventCamera.ScreenPointToRay(_eventData.position));
    print("INDEX:" + index);
    if(index != -1)
      Debug.Log("INDEX:" + index + ":" + getWordAtIndex(index));
  }


  //---------------------------------------------------------------------------
  private int getIndexOfClick(Ray _ray) {
    Ray localRay = new Ray(
      transform.InverseTransformPoint(_ray.origin),
      transform.InverseTransformDirection(_ray.direction));

    Vector3 localClickPos = localRay.origin +
      localRay.direction / localRay.direction.z * (transform.localPosition.z - localRay.origin.z);

    Debug.DrawRay(transform.TransformPoint(localClickPos), Vector3.up / 4, Color.green, 2.0f);

    var text = GetComponent<Text>();
    var textGen = text.cachedTextGenerator;
    print("SDF: " + textGen.characterCount);
    for(int i=0; i < textGen.characterCount; ++i) {
      Vector2 locUpperLeft = new Vector2(textGen.verts[i * 4].position.x, textGen.verts[i * 4].position.y);
      Vector2 locBottomRight = new Vector2(textGen.verts[i * 4 + 2].position.x, textGen.verts[i * 4 + 2].position.y);

      print("UL:" + locUpperLeft + "BR:" + locBottomRight + "CP:" + localClickPos);

      if(localClickPos.x >= locUpperLeft.x &&
        localClickPos.x <= locBottomRight.x &&
        localClickPos.y <= locUpperLeft.y &&
        localClickPos.y >= locBottomRight.y) {
        return i;
      }
    }

    return -1;
  }

  //---------------------------------------------------------------------------
  private string getWordAtIndex(int _idx) {
    var text = GetComponent<Text>();
    int startIdx = -1;
    int cursor = _idx;
    while(startIdx == -1) {
      cursor--;
      if(cursor < 0) {
        startIdx = 0;
      } else if(!char.IsLetter(text.text[cursor])) {
        startIdx = cursor+1;
      }
    }

    int lastIdx = -1;
    cursor = _idx;
    while(lastIdx == -1) {
      cursor++;
      if(cursor > text.text.Length-1) {
        lastIdx = text.text.Length;
      } else if(!char.IsLetter(text.text[cursor])) {
        lastIdx = cursor;
      }
    }
    return text.text.Substring(startIdx, lastIdx - startIdx); 
  }

  //---------------------------------------------------------------------------
  void OnDrawGizmos()
  {
    var text = GetComponent<Text>();
    if(text == null)
      return;
    
    var textGen = text.cachedTextGenerator;
    var prevMatrix = Gizmos.matrix;
    Gizmos.matrix = transform.localToWorldMatrix;
    for (int i = 0; i < textGen.characterCount; ++i) {
      Vector2 locUpperLeft = new Vector2(textGen.verts[i * 4].position.x, textGen.verts[i * 4].position.y);
      Vector2 locBottomRight = new Vector2(textGen.verts[i * 4 + 2].position.x, textGen.verts[i * 4 + 2].position.y);

      Vector3 mid = (locUpperLeft + locBottomRight) / 2.0f;
      Vector3 size = locBottomRight - locUpperLeft;

      Gizmos.DrawWireCube(mid, size);
    }
    Gizmos.matrix = prevMatrix;
  }
}

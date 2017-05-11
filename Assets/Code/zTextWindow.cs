using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class zTextWindow : MonoBehaviour, IPointerClickHandler {
  [HideInInspector]
  public TextMeshProUGUI m_text;

  public GameObject m_panel;
  Image m_cursorImage;

  // if not null, Update() will change text to this
  private string m_newText;

  private zNubCore m_core;
  private string m_currentText;   // keep a copy for reloads

  //------------------------------------------------------------------------
  void Start () {
    m_core = GetComponent<zNubCore>();
    m_cursorImage = m_panel.GetComponentInChildren<Image>();
    m_text = gameObject.GetComponentInChildren<TextMeshProUGUI>();
    if(m_text == null) {
      Debug.Log("[zTW:Start] Failed to find Text");
    } else {
      Debug.Log("[zTW:Start] Found Text");
      m_text.text = "<line-height=7><mspace=5><color=blue>HELLO</color>\nWoirld3";
      m_text.fontSize = 8;
    }
  }

  //------------------------------------------------------------------------
  void Update () {
    //gameObject.transform.Rotate(0, 1, 0);

    if(m_newText != null) {
      m_currentText = m_newText;
      m_text.text = m_newText;
      m_newText = null;
    }

    if (Input.GetKeyDown(KeyCode.Return)) {
      Debug.Log ("Call a method that will handle this key press");

      m_cursorImage.CrossFadeAlpha(0, 2, false);
    }

    if(m_cursorImage.canvasRenderer.GetAlpha() == 0)
      m_cursorImage.canvasRenderer.SetAlpha(1);

    Msg msg = m_core.peekMessage();
    if(msg != null && msg.text != null) {
      m_newText = msg.text;
      m_core.popMessage();
    }
  }

  //------------------------------------------------------------------------
  void OnGUI() {
    //if(Event.current.Equals(Event.KeyboardEvent("[enter]")))
    Event e = Event.current;
    //Debug.Log("[zTW:OnGUI] " + e);
  }

#if UNITY_EDITOR
  //------------------------------------------------------------------------
  void OnEnable() {
    Debug.Log("[zTW:OnEnable] Called");
    m_newText = m_currentText;
    if(m_text == null)
      m_text = gameObject.GetComponentInChildren<TextMeshProUGUI>();
  }
#endif

  public void OnPointerClick(PointerEventData _data) {
    _data.Use();

    // have focus!
    Debug.Log("[zTW:OnPointerClick] Called");

    //string msg = "{\"type\" : \"mouse\", \"mouseclick\" : 0}";
    BodyMsg msg = new BodyMsg();
    msg.type = "mouseclick";
    msg.button = 0;
    msg.pos = new int[3];
    /*
    msg.pos[0] = (int)(_data.worldPosition[0] + 0.5);
    msg.pos[1] = (int)(_data.worldPosition[1] + 0.5);
    msg.pos[2] = (int)(_data.worldPosition[2] + 0.5);
    */
    m_core.dispatchMsg("window", msg);
  }

  internal void setText(string _text) {
    m_newText = _text;
  }
}

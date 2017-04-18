using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class aeTextWindow : MonoBehaviour {

  [HideInInspector] public TextMeshProUGUI m_text;
  public GameObject m_panel;

  Image m_cursorImage;

  void Start () {
    m_cursorImage = m_panel.GetComponentInChildren<Image>();
    m_text = gameObject.GetComponentInChildren<TextMeshProUGUI>();
    if(m_text == null)
    {
      Debug.Log("[aeTW:Start] Failed to find Text");
    } else {
      Debug.Log("[aeTW:Start] Found Text");
      m_text.text = "<line-height=7><mspace=5><color=blue>HELLO</color>\nWoiild3";
      m_text.fontSize = 8;
    }
  }

  void Update () {
    gameObject.transform.Rotate(0, 1, 0);

    if (Input.GetKeyDown(KeyCode.Return)) {
      Debug.Log ("Call a method that will handle this key press");

      m_cursorImage.CrossFadeAlpha(0, 2, false);
    }

    if(m_cursorImage.canvasRenderer.GetAlpha() == 0)
      m_cursorImage.canvasRenderer.SetAlpha(1);
  }

  void OnGUI() {
    //if(Event.current.Equals(Event.KeyboardEvent("[enter]")))
    Event e = Event.current;
    //Debug.Log("[aeTW:OnGUI] " + e);
  }

  void OnEnable() {
    Debug.Log("[aeTW:OnEnable] Called");
    if(m_text) {
      m_text.text = "<line-height=7><mspace=5><color=blue>HELLO</color>\nWoiild3";
      m_text.fontSize = 8;
    }
  }
}

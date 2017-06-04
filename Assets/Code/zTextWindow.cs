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
  private int m_fontSize;
  private int m_lineHeight;
  private int m_charWidth;
  private string m_textPreamble;

  public GameObject m_panel;
  Image m_cursorImage;

  // if not null, Update() will change text to this
  private string m_newText;
  private zNubCore m_core;
  private int m_downKeyDown;
  private string m_currentText;   // keep a copy for reloads

  // cursor related
  private bool m_hasNewCursorPos;
  private Vector2 m_cursorOffset;     // offset to apply to all calcs
  private int[] m_currentCursorPos; // current position
  private int[] m_newCursorPos;     // from mind msg

  private static zTextWindow m_hasFocus;

  private static KeyCode[] m_keyCode = {KeyCode.UpArrow, KeyCode.DownArrow,
                                        KeyCode.LeftArrow, KeyCode.RightArrow,
                                        KeyCode.Escape, KeyCode.Tab};

  //------------------------------------------------------------------------
  private void initTextParams() {
    m_fontSize = 4;
    m_lineHeight = m_fontSize;
    m_charWidth = m_fontSize/2+1;
    m_textPreamble = "<line-height=" + m_lineHeight +
                      "><mspace=" + m_charWidth + ">";
  }

  //------------------------------------------------------------------------
  void Start () {
    m_core = GetComponent<zNubCore>();
    m_currentCursorPos = new int[2];
    m_cursorOffset = new Vector2();
    m_newCursorPos = new int[2];
    m_hasNewCursorPos = false;
    m_cursorImage = m_panel.GetComponentInChildren<Image>();
    m_cursorOffset = m_cursorImage.rectTransform.anchoredPosition;
    m_text = gameObject.GetComponentInChildren<TextMeshProUGUI>();
    if(m_text == null) {
      Debug.Log("[zTW:Start] Failed to find Text");
    } else {
      Debug.Log("[zTW:Start] Found TextMeshPro");
      initTextParams();
      m_text.text = m_textPreamble + "<color=blue>HELLO</color>\nWoirld3";
    }

    if(m_hasFocus == null) {
      m_hasFocus = this;
      m_downKeyDown = 0;
    }
  }

  //------------------------------------------------------------------------
  void Update () {

    // update any pending text updates
    if(m_newText != null) {
      m_currentText = m_newText;
      m_text.text = m_textPreamble + m_newText;
      m_newText = null;
    }

    // need to change cursor pos?
    if(m_hasNewCursorPos) {
      m_hasNewCursorPos = false;
      m_currentCursorPos = m_newCursorPos;
      Debug.Log("[zTW:Update] cursor pos:" + m_cursorImage.rectTransform.anchoredPosition);
      m_cursorImage.rectTransform.anchoredPosition = m_cursorOffset +
          Vector2.Scale(new Vector2(m_currentCursorPos[0], m_currentCursorPos[1]),
            new Vector2(m_charWidth, -m_lineHeight));
    }

    // Test
    if (Input.GetKeyDown(KeyCode.Return)) {
      m_cursorImage.CrossFadeAlpha(0, 2, false);
    }
    if(m_cursorImage.canvasRenderer.GetAlpha() == 0)
      m_cursorImage.canvasRenderer.SetAlpha(1);

    if(Input.anyKey) {
      if(Input.inputString.Length > 0) {
        // verify it's a valid value. macOS seems to generate 'strings' for
        // arrow key values
        int iValue = Input.inputString[0];
        // is ascii?
        if(iValue < 256 && iValue >= 8) {
          BodyMsg bmsg = new BodyMsg();
          bmsg.type = "char";
          bmsg.text = Input.inputString;
          m_core.dispatchMsg("window", bmsg);
        } else {
          Debug.Log("[zTW:Update] INVALID TEXT[" + Input.inputString.Length + "]: " +
                       Input.inputString + " [" + iValue + "]");
        }
      }
    }

    if(Input.anyKeyDown) {
      for(int i=0; i<m_keyCode.Length; i++) {
        if(Input.GetKeyDown(m_keyCode[i]))
        {
          BodyMsg bmsg = new BodyMsg();
          bmsg.type = "keydown";
          bmsg.keycode = (int)m_keyCode[i];
          m_core.dispatchMsg("window", bmsg);
          m_downKeyDown++;
        }
      }
    }

    if(m_downKeyDown > 0) {
      for(int i=0; i<m_keyCode.Length; i++) {
        if(Input.GetKeyUp(m_keyCode[i]))
        {
          BodyMsg bmsg = new BodyMsg();
          bmsg.type = "keyup";
          bmsg.keycode = (int)m_keyCode[i];
          m_core.dispatchMsg("window", bmsg);
          m_downKeyDown--;
        }
      }

      //if(m_downKeyDown < 0)
      //  m_downKeyDown = 0;
    }

    // verify
    if(!(Input.anyKey || Input.anyKeyDown))
      m_downKeyDown = 0;

    // FIXME!
    Msg msg = m_core.peekMessage();
    if(msg != null) {
      if(msg.text != null) {
        m_newText = msg.text;
      }

      if(msg.cursor != null && msg.cursor.Length > 0) {
        m_newCursorPos = msg.cursor;
        m_hasNewCursorPos = true;
      }
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

    initTextParams();
  }
#endif

  //------------------------------------------------------------------------
  public void OnPointerClick(PointerEventData _data) {
    _data.Use();

    // have focus!
    Debug.Log("[zTW:OnPointerClick] Called");

    //string msg = "{\"type\" : \"mouse\", \"mouseclick\" : 0}";
    BodyMsg msg = new BodyMsg();
    msg.type = "mouseclick";
    msg.button = 0;
    msg.pos = new float[3];
    msg.pos[0] = _data.pointerCurrentRaycast.worldPosition[0];
    msg.pos[1] = _data.pointerCurrentRaycast.worldPosition[1];
    msg.pos[2] = _data.pointerCurrentRaycast.worldPosition[2];
    m_core.dispatchMsg("window", msg);

    m_hasFocus = this;
    m_downKeyDown = 0;
  }

  //------------------------------------------------------------------------
  internal void setText(string _text) {
    m_newText = _text;
  }
}

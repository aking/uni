using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//public delegate void dispatchDelegate(string _msg);

struct WisperMsg {
  public string cmd;
  public string cmdParam;
}

[System.Serializable]
public class MCP : MonoBehaviour {

  [SerializeField]
  private dispatchDelegate m_dispatchFn;
  [SerializeField]
  private GameObject m_player;

  void Start() {
    m_player = GameObject.FindWithTag("Player");
    print("[MCP:Start] Player:" + (m_player ? "FOUND" : "NOT FOUND"));

    WisperMsg msg = new WisperMsg();
    msg.cmd = "cmd";
    msg.cmdParam = "START";
    //m_dispatchFn?.Invoke(JsonUtility.ToJson(msg));
    if(m_dispatchFn != null)
      m_dispatchFn(JsonUtility.ToJson(msg));
    else
      print("[MCP:Start] No DISPATCH fn");
  }

  void Update() {
    if(!m_player)
      return;

    // send position
    Vector3 pos = m_player.transform.position;
    m_dispatchFn("{\"pos\" : [" + pos.x + ", " + pos.y + ", " + pos.z + "]}");
  }

  public void setDispatchFn(dispatchDelegate _fn) {
    m_dispatchFn = _fn;
  }
}

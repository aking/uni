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

  private WebSocketServer m_socketServer;
  private GameObject m_player;

  // doesn't serialize
  private dispatchDelegate m_dispatchFn;

  void OnEnable() {
    Debug.Log("[MCP:OnEnable] Called");

    if(m_socketServer == null) {
      Debug.Log("[MCP:OnEnable] WebSocket server NULL!");
      return;
    }

    if(m_dispatchFn == null) {
      Debug.Log("[MCP:OnEnable] Dispatch fn is NULL!");
      m_dispatchFn = m_socketServer.dispatch;
    }
  }

  void Start() {
    m_socketServer = ScriptableObject.CreateInstance<WebSocketServer>();
    m_socketServer.init();
    m_dispatchFn = m_socketServer.dispatch;

    m_player = GameObject.FindWithTag("Player");
    Debug.Log("[MCP:Start] Player:" + (m_player ? "FOUND" : "NOT FOUND"));

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
    if(m_socketServer != null)
      m_socketServer.update();

    if(!m_player) {
      m_player = GameObject.FindWithTag("Player");
      if(!m_player)
        return;
    }

    // send position
    Vector3 pos = m_player.transform.position;
    if(m_dispatchFn != null)
      m_dispatchFn("{\"pos\" : [" + pos.x + ", " + pos.y + ", " + pos.z + "]}");
  }
}

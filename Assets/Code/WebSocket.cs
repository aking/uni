using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public delegate void dispatchDelegate(string _msg);

[System.Serializable]
public class TagObj {
  public string tag;
}

public class WebSocket : ScriptableObject {
  private MCP m_mcp;
  private CellManager m_cellManager;
  private GameObject m_stdHex;

  //---------------------------------------------------------------------------
  // Native Implementation interface
#if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
  [DllImport("WebSocket")]
  private static extern void _init(int _port);

  [DllImport("WebSocket")]
  private static extern void _tick();

  [DllImport("WebSocket")]
  private static extern void _dispatch(string _msg);

  [DllImport("WebSocket")]
  private static extern string _getData();
#elif UNITY_IOS   // NOTE: will also match EDITOR_OSX!!!
  [DllImport("__Internal")]
  private static extern void _init(int _port);

  [DllImport("__Internal")]
  private static extern void _tick();

  [DllImport("__Internal")]
  private static extern void _dispatch(string _msg);

  [DllImport("__Internal")]
  private static extern string _getData();
#else
    private static void _init(int _port) {Debug.Log("[WebSocket:_init] IMPLEMENT ME");}
    private static void _tick() {Debug.Log("[WebSocket:_tick] IMPLEMENT ME");}
    private static void _dispatch(string _msg) {
      Debug.Log("[WebSocket:_dispatch] IMPLEMENT ME:" + _msg);}
    private static string _getData() {Debug.Log("[WebSocket:_getData] IMPLEMENT ME"); return null;}
#endif
  //---------------------------------------------------------------------------

  //------------------------------------------------------------------------
  void Awake() {
    Debug.Log("[WebSocket:Awake] Call...");
    m_stdHex = Resources.Load<GameObject>("Geom/stdHex");
  }

  // Public interface
  //---------------------------------------------------------------------------
  public void init(MCP _mcp, CellManager _cm) {
    m_cellManager = _cm;
    m_cellManager.setDispatchFn(dispatch);
    m_mcp = _mcp;

    debugNative();
    _init(9001);
  }

  //---------------------------------------------------------------------------
  public void update() {
    WebSocket._tick();
    tickReceivedPackets();
  }

  //---------------------------------------------------------------------------
  bool tickReceivedPackets() {
    string data = WebSocket._getData();
    bool hadData = (data != null);
    if(hadData) {
      Debug.Log("[WebSocket:tickReceivedPackets] DATA:" + data);
      TagObj obj = null;
      try {
        obj = JsonUtility.FromJson<TagObj>(data);
      } catch(Exception ex) {
        Debug.Log("[WebSOcket:tick] INVALID DATA: " + data + "\n" + ex);
        return false;
      }
      Debug.Log("OBJ: " + obj.tag);
      string msg = data;
      if (obj.tag == "hex") {
        Debug.Log("[WebSocket:tickReceivedPackets] Got a hex obj");
        handleHex(msg);
      } else if (obj.tag == "cell") {
        Debug.Log("[WebSocket:tickReceivedPackets] Got a Cell obj");
        handleCell(msg);
      } else if (obj.tag == "nub") {
        Debug.Log("[WebSocket:tickReceivedPackets] Got a NUB obj");
        m_mcp.handleNub(msg);
      } else if (obj.tag == "body") {
        Debug.Log("[WebSocket:tickReceivedPackets] Got a BODY cmd");
        m_mcp.handleCmd(msg);
      } else {
        Debug.LogError("WebSocket:tickReceivedPackets] UNKNOWN tag type:" + obj.tag);
      }
    } else {
      //Debug.Log("[WebSocket:_getData] NO DATA");
    }
    return hadData;
  }

  //---------------------------------------------------------------------------
  void handleCell(string _msg) {
    CellMsg cellMsg = JsonUtility.FromJson<CellMsg>(_msg);
    m_cellManager.handleCellMsg(cellMsg);
  }

  //------------------------------------------------------------------------
  void handleHex(string _msg) {
    HexMsg nub = JsonUtility.FromJson<HexMsg>(_msg);
    switch(nub.cmd) {
      case "add":
        Debug.Log("[WSS:handleHex] add cmd");
        Vector3 pos = new Vector3(nub.pos[0], nub.pos[1], nub.pos[2]);
        GameObject hex = Instantiate(m_stdHex, pos, Quaternion.identity);
        hex.name = "hex";
        //hex.transform.parent = m_envRoot.transform;
        break;

      case "delete-all":
        Debug.Log("[WSS:handleHex] delete-all cmd");
/*        int numChild = m_envRoot.transform.childCount;
        for(int i = numChild; i > 0; i--) {
          Destroy(m_envRoot.transform.GetChild(i - 1).gameObject);
        }*/
        break;

      default:
        Debug.Log("[WSS:handleHex] UNKNOWN cmd:" + nub.cmd);
        break;
    }
  }
  //---------------------------------------------------------------------------
  public void dispatch(string _msg) {
    Debug.Log("[WebSocket:dispatch] Need to dispatch [" + _msg.Length + "]:" + _msg);
    WebSocket._dispatch(_msg);
  }

  //---------------------------------------------------------------------------
#if UNITY_EDITOR
  [UnityEditor.Callbacks.DidReloadScripts]
  private static void OnScriptsReloaded() {
    Debug.Log("[WebSocket] RELOADED -------------");
  }
#endif

  private void OnEnable() {
    Debug.Log("[WebSocket:OnEnable] BEING ENABLED\n");
    if(m_cellManager) {
      m_cellManager.setDispatchFn(dispatch);
    } else {
      Debug.Log("[WebSocket:OnEnable] CAN'T SET");
    }
  }

  private void OnDisable() {
    Debug.Log("[WebSocket:OnDisable] BEING DISABLED\n");
  }

  //---------------------------------------------------------------------------
  private static void debugNative() {
#if UNITY_IOS
    Debug.Log("[WebSocket:init] IOS");
#endif
#if UNITY_STANDALONE_OSX
    Debug.Log("[WebSocket:init] STANDALONE_OSX");
#endif
#if UNITY_EDITOR_OSX
    Debug.Log("[WebSocket:init] EDITOR_OSX");
#endif
  }
}

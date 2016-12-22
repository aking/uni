using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;

[System.Serializable]
public class TagObj
{
  public string tag;
}

[System.Serializable]
public class NubMsg
{
  public int[] pos;
  public string style;
  public string tag;
  public string cmd;
}

[System.Serializable]
public class Hex
{
  public int[] pos;
  public string style;
  public GameObject obj;
}

[System.Serializable]
public class CellMsg
{
  public string tag;
  public string cmd;
  public Cell cell;
}

public class WebSocketServer : MonoBehaviour
{
  int m_hostId;
  int m_chanId;
  int m_connId;
  private CellManager m_cellManager;

  public GameObject m_envRoot;
  public GameObject m_stdHex;

  //------------------------------------------------------------------------
  void Start() {
    SetupServer();
    m_cellManager = ScriptableObject.CreateInstance<CellManager>();
    m_cellManager.setDispatchFn(dispatch);

    int val = WebBrowserWrapper.openBrowser();
    Debug.Log("[WSS:Start] val = " + val);

  }

  private void OnEnable() {
    Debug.Log("[WSS:OnEnable] BEING ENABLED\n");
    if(m_cellManager) {
      m_cellManager.setDispatchFn(dispatch);
    } else {
      Debug.Log("[WSS:OnEnable] CAN'T SET");
    }
  }

  private void OnDisable() {
    Debug.Log("[WSS:OnDisable] BEING DISABLED\n");
    WebBrowserWrapper.closeBrowser();
  }

  //------------------------------------------------------------------------
  void Update() {
    int hostId;
    int connId;
    int chanId;
    byte[] buffer = new byte[1024];
    int recSize;
    byte errorCode;

    NetworkEventType recData = NetworkTransport.Receive(out hostId,
                                     out connId, out chanId, buffer, 1024, out recSize, out errorCode);

    switch(recData) {
      case NetworkEventType.Nothing:
        break;
      case NetworkEventType.ConnectEvent:
        Debug.Log("[WSS:Update] CONNECTION");
        m_connId = connId;
        break;
      case NetworkEventType.DisconnectEvent:
        Debug.Log("[WSS:Update] DIS CONNECITON!");
        break;
      case NetworkEventType.DataEvent:
        string msg = "{" + System.Text.Encoding.UTF8.GetString(buffer);
        Debug.Log("[WSS:Update] Got Data:" + msg);
        Debug.Log("[WSS:Update] [" + recSize + "] [" + errorCode + "]");
        TagObj obj = JsonUtility.FromJson<TagObj>(msg);
        Debug.Log("OBJ: " + obj.tag);
        if(obj.tag == "hex") {
          Debug.Log("[WSS:Update] Got a hex obj");
          handleHex(msg);
        } else if(obj.tag == "cell") {
            Debug.Log("[WSS:Update] Got a Cell obj");
            handleCell(msg);
          } else {
            Debug.LogError("WSS:Update] UNKNOWN tag type:" + obj.tag);
          }
        break;
      default:
        Debug.Log("[WSS:Update] SOMETHING ELSE");
        break;
    }

    // test
    /*
    byte[] buf = System.Text.Encoding.UTF8.GetBytes("{HI}");
    //int bufCount = System.Text.Encoding.UTF8.GetByteCount("{HI}");
    bool result = NetworkTransport.Send(hostId, connId, chanId, buf, buf.Length, out errorCode);
    Debug.Log("[WSS:Update] result=" + result + " ERROR:" + errorCode);
*/
  }

  //------------------------------------------------------------------------
  public void dispatch(string _str) {
    Debug.Log("[WSS:dispatch] Need to dispatch:" + _str);

    byte errorCode;
    byte[] buf = System.Text.Encoding.UTF8.GetBytes("{\"HI\": 23}");
    //int bufCount = System.Text.Encoding.UTF8.GetByteCount("{HI}");
    bool result = NetworkTransport.Send(m_hostId, m_connId, m_chanId, buf, buf.Length, out errorCode);
    Debug.Log("[WSS:dispatch] result=" + result + " ERROR:" + errorCode);
  }

  //------------------------------------------------------------------------
  void handleHex(string _msg) {
    NubMsg nub = JsonUtility.FromJson<NubMsg>(_msg);
    switch(nub.cmd) {
      case "add":
        Debug.Log("[WSS:handleHex] add cmd");
        Vector3 pos = new Vector3(nub.pos[0], nub.pos[1], nub.pos[2]);
        GameObject hex = Instantiate(m_stdHex, pos, Quaternion.identity);
        hex.name = "hex";
        hex.transform.parent = m_envRoot.transform;
        break;

      case "delete-all":
        Debug.Log("[WSS:handleHex] delete-all cmd");
        int numChild = m_envRoot.transform.childCount;
        for(int i = numChild; i > 0; i--) {
          Destroy(m_envRoot.transform.GetChild(i - 1).gameObject);
        }
        break;

      default:
        Debug.Log("[WSS:handleHex] UNKNOWN cmd:" + nub.cmd);
        break;
    }
  }

  //------------------------------------------------------------------------
  void handleCell(string _msg) {
    CellMsg cellMsg = JsonUtility.FromJson<CellMsg>(_msg);
    m_cellManager.handleCellMsg(cellMsg);
  }

  //------------------------------------------------------------------------
  void SetupServer() {
    Debug.Log("[WSS:SetupServer] CALLED");
    NetworkTransport.Init();
    ConnectionConfig config = new ConnectionConfig();
    m_chanId = config.AddChannel(QosType.Reliable);
    HostTopology topology = new HostTopology(config, 10);
    m_hostId = NetworkTransport.AddWebsocketHost(topology, 9001, null);
    Debug.Log("[WSS:SetupServer] WebSocket HostID:" + m_hostId + "  ChanID:" + m_chanId);
  }
    
#if UNITY_EDITOR
  [UnityEditor.Callbacks.DidReloadScripts]
  private static void OnScriptsReloaded() {
    Debug.Log("[WSS] RELOADED -------------");
  }
#endif
}

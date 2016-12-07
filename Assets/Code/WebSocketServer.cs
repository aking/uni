using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;

[System.Serializable]
public class TagObj {
    public string tag;
}

[System.Serializable]
public class NubMsg {
    public int[] pos;
    public string style;
    public string tag;
    public string cmd;
}

[System.Serializable]
public class Hex {
    public int[] pos;
    public string style;
    public GameObject obj;
}

[System.Serializable]
public class CellMsg {
    public string tag;
    public string cmd;
    public Cell cell;
}

public class WebSocketServer : MonoBehaviour {

    int m_hostId;
    int m_chanId;

    public CellManager _cellManager;
    public GameObject _envRoot;
    public GameObject _stdHex;
    public GameObject m_testHex;

    //------------------------------------------------------------------------
    void Start() {
        SetupServer();
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
    }

    //------------------------------------------------------------------------
    void handleHex(string _msg) {
        NubMsg nub = JsonUtility.FromJson<NubMsg>(_msg);
        switch(nub.cmd) {
            case "add":
                Debug.Log("[WSS:handleHex] add cmd");
                Vector3 pos = new Vector3(nub.pos[0], nub.pos[1], nub.pos[2]);
                GameObject hex = Instantiate(_stdHex, pos, Quaternion.identity);
                hex.name = "hex";
                hex.transform.parent = _envRoot.transform;
                break;

            case "delete-all":
                Debug.Log("[WSS:handleHex] delete-all cmd");
                int numChild = _envRoot.transform.childCount;
                for (int i = numChild; i > 0; i--) {
                    Destroy(_envRoot.transform.GetChild(i-1).gameObject);
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
        _cellManager.handleCellMsg(cellMsg);
        /*
        switch (cellMsg.cmd)
        {
            case "new-cell":
                Debug.Log("[WSS:handleCell] NEW CELL:" + cellMsg.cell);
                _cellManager.addCell(cellMsg.cell);
                break;

            case "delete-all":
                Debug.Log("[WSS:handleCell] DELETE ALL CELLs");
                _cellManager.deleteAll();
                break;

            default:
                Debug.LogError("[WSS:handleCell] UNKNOWN cmd:" + cellMsg.cmd);
                break;
        }
        */
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

    [UnityEditor.Callbacks.DidReloadScripts]
    private static void OnScriptsReloaded() {
        Debug.Log("[WSS] RELOADED -------------");
    }
}

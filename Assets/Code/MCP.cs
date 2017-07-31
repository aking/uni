using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//public delegate void dispatchDelegate(string _msg);

struct WisperMsg {
  public string cmd;
  public string cmdParam;
}

//------------------------------------------------------------------------
[System.Serializable]
public class MCP : MonoBehaviour {
  public GameObject m_textWindow;
  public GameObject m_coreNub;
  public Camera m_activeCamera;

  private WebSocketServer m_socketServer;
  private CellManager m_cellManager;
  private GameObject m_player;

  private GameObject m_sphereObj;

  // doesn't serialize
  private dispatchDelegate m_dispatchFn;

  private Dictionary<string, GameObject> m_nubMap;
  private Queue<BodyNubMsg> m_dispatchQueue;

  //------------------------------------------------------------------------
  void OnEnable() {
    Debug.Log("[MCP:OnEnable] Called....");

    if(m_socketServer == null) {
      Debug.Log("[MCP:OnEnable] WebSocket server NULL!");
      return;
    }

    // Items that need resetting from a C# hot code reload
    if(m_dispatchQueue == null) {
      m_dispatchQueue = new Queue<BodyNubMsg>();
    }

    if(m_dispatchFn == null) {
      Debug.Log("[MCP:OnEnable] Dispatch fn is NULL!");
      m_dispatchFn = m_socketServer.dispatch;
    }

    if(m_nubMap == null) {
      m_nubMap = new Dictionary<string, GameObject>();

      // now populate it!
      GameObject[] nubs = GameObject.FindGameObjectsWithTag("Nub");
      Debug.Log("[MCP:OnEnable] nub map is NULL! " + nubs.Length + " nubs to add back.");
      for(int i=0; i<nubs.Length; i++) {
        zNubCore core = nubs[i].GetComponent<zNubCore>();
        core.setDispatchQueue(m_dispatchQueue);
        m_nubMap.Add(core.m_id, nubs[i]);
      }
    }
  }

  //------------------------------------------------------------------------
  void Start() {
    // test
    WebSocket.init();

    m_nubMap = new Dictionary<string, GameObject>();
    m_dispatchQueue = new Queue<BodyNubMsg>();

    m_cellManager = ScriptableObject.CreateInstance<CellManager>();
    m_socketServer = ScriptableObject.CreateInstance<WebSocketServer>();
    m_socketServer.init(this, m_cellManager);
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

#if UNITY_EDITOR
    //m_coreNub = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Nub.prefab", typeof(GameObject));
#endif

    m_sphereObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
    m_sphereObj.hideFlags = HideFlags.HideAndDontSave;
    m_sphereObj.SetActive (false);
    Debug.Assert(m_coreNub!=null, "[WSS:Awake] Unable to find Nub prefab");
  }

  //------------------------------------------------------------------------
  void Update() {
    if(m_socketServer != null)
      m_socketServer.update();

    WebSocket.tick();

    if(m_dispatchQueue.Count > 0) {
      Debug.Log("[MCP:Update] Dispatching msg");
      BodyNubMsg msg = m_dispatchQueue.Dequeue();
      m_dispatchFn(JsonUtility.ToJson(msg));
    }

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

  //------------------------------------------------------------------------
  GameObject findNub(string _id) {
    GameObject[] nubs = GameObject.FindGameObjectsWithTag("Nub");
    for(int i=0; i<nubs.Length; i++) {
      zNubCore core = nubs[i].GetComponent<zNubCore>();
      if(core != null) {
        if(core.m_id == _id) {
          return nubs[i];
        }
      }
    }
    Debug.LogWarning("[MCP:findNub] Unable to find nub:" + _id);
    return null;
  }

  //------------------------------------------------------------------------
  internal void handleCmd(string _msg) {
    BodyCmdMsg cmdMsg = JsonUtility.FromJson<BodyCmdMsg>(_msg);
    switch(cmdMsg.cmd) {
      case "reset":
        Debug.Log("[MCP:handleCmd] RESET requested");
        foreach (var item in m_nubMap) {
          Debug.Log("[MCP:handleCmd] Deleteing Nub:" + item.Key);
          Destroy(item.Value);
        }
        m_nubMap.Clear();

        m_cellManager.deleteAll();
        break;

      default:
        Debug.Log("[MCP:handleCmd] Unknown cmd:" + cmdMsg.cmd);
        break;
    }
  }

  //------------------------------------------------------------------------
  internal void handleNub(string _msg) {
    NubMsg nubMsg = JsonUtility.FromJson<NubMsg>(_msg);
    switch(nubMsg.cmd) {
      case "add": {
        Debug.Log("[MCP:handleNub] add cmd");

        // determine type of nub to add.
        GameObject newNub = null;
        if(nubMsg.nub.window != null &&nubMsg.nub.window.width > 0) {
          newNub = addWindowNub(nubMsg.nub);
        } else {
          newNub = addNub(nubMsg.nub);
        }

        m_nubMap.Add(nubMsg.nub.id, newNub);
        break;
      }

      case "remove": {
        Debug.Log("[MCP:handleNub] remove cmd:" + nubMsg.id);

        GameObject nub = null;
        if(m_nubMap.TryGetValue(nubMsg.id, out nub)) {
            Debug.Log("[MCP:handleNub:remove] FOUND NUB IN CACHE");
            m_nubMap.Remove(nubMsg.id);
            Destroy(nub);
        } else {
            Debug.Log("[MCP:handleNub:remove] NUB NOT FOUND IN CACHE. Fallback find..");
            nub = findNub(nubMsg.id);
            if (nub != null)
            {
              Destroy(nub);
            } else {
              Debug.LogWarning("[MCP:handleNub:remove] Fallback find failed. No nub to REMOVE");
            }
        }

        break;
      }

      case "delete-all": {
        Debug.Log("[MCP:handleNub] delete-all cmd");
/*        int numChild = m_envRoot.transform.childCount;
        for(int i = numChild; i > 0; i--) {
          Destroy(m_envRoot.transform.GetChild(i - 1).gameObject);
        }*/
        break;
      }

      case "msg": {
        Debug.Log("[MCP:handleNub:msg] msg cmd");
        if(nubMsg.msg == null) {
          Debug.LogWarning("[MCP:handleNub:msg] Missing message!");
          break;
        }

        GameObject nub = null;
        if(m_nubMap.TryGetValue(nubMsg.id, out nub)) {
            Debug.Log("[MCP:handleNub:MSG] FOUND NUB IN CACHE");
            nub.GetComponent<zNubCore>().newMessage(nubMsg.msg);
        } else {
            Debug.Log("[MCP:handleNub:MSG] NUB NOT FOUND IN CACHE");
        }

        break;
      }

      case "window": { // gene!
        GameObject nub;
        if(m_nubMap.TryGetValue(nubMsg.id, out nub)) {
            Debug.Log("[MCP:handleNub:window] FOUND NUB IN CACHE");
            nub.GetComponent<zNubCore>().newMessage(nubMsg.msg);
        } else {
            Debug.Log("[MCP:handleNub:window] NUB NOT FOUND IN CACHE:" + nubMsg.id);
        }
        break;
      }

      default:
        Debug.Log("[MCP:handleNub] UNKNOWN cmd:" + nubMsg.cmd);
        break;
    }
  }

  //------------------------------------------------------------------------
  private GameObject createCoreNub(GameObject _prefab, Nub _nub) {
    Core core = _nub.core;
    Vector3 pos = new Vector3(core.pos[0], core.pos[1], core.pos[2]);

    GameObject nub = Instantiate(_prefab, pos, Quaternion.identity);
    zNubCore nubCore = nub.GetComponent<zNubCore>();
    nubCore.init(_nub);
    nubCore.setDispatchQueue(m_dispatchQueue);

    nub.transform.position = pos;
    nub.transform.rotation = Quaternion.Euler(core.rot[0], core.rot[1], core.rot[2]);

    return nub;
  }

  //------------------------------------------------------------------------
  private void addSphereComponent(GameObject _nub, Visual _vis) {
    float radius = _vis.radius;

    MeshFilter mesh = _nub.GetComponent<MeshFilter>();
    mesh.mesh = m_sphereObj.GetComponent<MeshFilter> ().mesh;

    // Physics hack
    //Rigidbody rb = _nub.GetComponent<Rigidbody> ();
    //rb.isKinematic = true;

    // Sphere Collider
    SphereCollider col = _nub.AddComponent<SphereCollider> ();
    col.radius = 0.5f;
    PhysicMaterial mat = Resources.Load<PhysicMaterial> ("PhysMats/PerfectBounce");
    col.material = mat;

    // Scale as needed
    _nub.transform.localScale = new Vector3 (radius, radius, radius);
  }

  //------------------------------------------------------------------------
  internal GameObject addNub(Nub _nub) {
    GameObject nub = createCoreNub(m_coreNub, _nub);

    switch(_nub.visual.prim) {
      case "sphere": {
        Debug.Log("[MCP:addNub] sphere visual");
        addSphereComponent(nub, _nub.visual);
        break;
      }

      default: {
        Debug.Log("[MCP:addNub] UNKNOWN visual prim");
        addSphereComponent(nub, _nub.visual);
        break;
      }

    }

    return nub;
  }

  //------------------------------------------------------------------------
  internal GameObject addWindowNub(Nub _nub) {
    Debug.Log("[MCP:addWindowNub] add window");

    GameObject nub = createCoreNub(m_textWindow, _nub);

/*
    Core core = _nub.core;
    Vector3 pos = new Vector3(core.pos[0], core.pos[1], core.pos[2]);

    GameObject nub = null;
    nub = Instantiate(m_textWindow, pos, Quaternion.identity);
    zNubCore nubCore = nub.GetComponent<zNubCore>();
    nubCore.init(_nub);
    nubCore.setDispatchQueue(m_dispatchQueue);

    nub.transform.position = pos;
    nub.transform.rotation = Quaternion.Euler(core.rot[0], core.rot[1], core.rot[2]);
*/
    // set the EventCamera on the canvas
    Canvas canvas = nub.GetComponent<Canvas>();
    if(canvas) {
      canvas.worldCamera = m_activeCamera;
    }

    zTextWindow win = nub.GetComponent<zTextWindow>();
    //win.GetComponent<zNubCore>().init(_nub);

    // If no name specified, override the default nub name
    if(_nub.core.name != null) {
      nub.name = "[WIN]:" + _nub.core.name;
    } else {
      nub.name = "[WIN]:" + _nub.id;
    }

    // set any intial text
    if(_nub.window.text != null) {
      win.setText(_nub.window.text);
    }

    return nub;
  }
}

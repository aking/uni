using UnityEngine;

//-----------------------------------------------------------------------------
// Nub CREATION Msgs
//-----------------------------------------------------------------------------
[System.Serializable]
public class Core {
  public float[] pos;
  public float[] rot;
  public string name;
}

[System.Serializable]
public class Window {
  public int width;
  public int height;
  public string text;
}

[System.Serializable]
public class Visual {
  public string prim;
  public float radius;
}

[System.Serializable]
public class Nub {
  public string id;
  public Core core;
  public Window window;
  public Visual visual;
}

//-----------------------------------------------------------------------------
// Messages

// To be replaced with a msgpack
[System.Serializable]
public class Msg {
  public string text;
  public int[] cursor;
}

[System.Serializable]
public class NubMsg {
  public string tag;
  public string cmd;
  public string id;
  public Nub nub;
  public Msg msg;   // replace with msgpack
}

[System.Serializable]
public class BodyMsg {
  public string type;
  public int keycode;
  public string text;
  public int button;
  public float[] pos;
  public float[] normal;
  public string hitName;
}

[System.Serializable]
public class BodyNubMsg {
  public string tag = "body";
  public string cmd;
  public string id;
  public BodyMsg msg;
}

[System.Serializable]
public class BodyCmdMsg {
  public string tag;
  public string cmd;
}

[System.Serializable]
public class HexMsg {
  public float[] pos;
  public string style;
  public string cmd;
  //public GameObject obj;
}

[System.Serializable]
public class CellMsg {
  public string tag;
  public string cmd;
  public Cell cell;
}

[System.Serializable]
public class Hex {
  public float[] pos;
  public string style;
  public GameObject obj;
}


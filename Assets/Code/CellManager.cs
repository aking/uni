using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Cell {
    public string id;
    public int[] pos;
    public GameObject rootObj;
    public Hex[] hexes;

    public override string ToString() {
        string hexStr = "";
        for(int i=0; i<hexes.Length; ++i) {
            hexStr += "\n     " + i + ": ( " + hexes[i].pos[0] + ", " + hexes[i].pos[1] + ", " + hexes[i].pos[2] + ") " +
                " style: " + hexes[i].style;
        }
        return "Cell <" + hexes.Length + "> : [" + id + "]\n" +
            "    pos: [" + pos[0] + ", " + pos[1] + ", " + pos[2] + "]\n" +
            "    hexes: " + hexStr;
    }
}

public class CellManager : MonoBehaviour {

    private GameObject m_stdHex;

    private Dictionary<string, Cell> m_cells;

    void Start() {
        m_cells = new Dictionary<string, Cell>();
        m_stdHex = Resources.Load<GameObject>("Geom/stdHex");
    }

    void Update() {
    }

    public void addCell(Cell _cell) {
        Debug.Log("[CM:addCell] Adding new cell");
        // Set the Cell's root obj
        int[] p = _cell.pos;
        _cell.rootObj = new GameObject("CellRoot");
        _cell.rootObj.transform.position = new Vector3(p[0], p[1], p[2]);
        m_cells.Add(_cell.id, _cell);

        foreach(Hex h in _cell.hexes) {
            GameObject hex = Instantiate(m_stdHex, _cell.rootObj.transform);
            hex.name = "hex";

            Vector3 pos = new Vector3(h.pos[0], h.pos[1], h.pos[2]);
            hex.transform.localPosition = pos;
            h.obj = hex;
        }
    }

    public void deleteAll() {
        foreach(Cell c in m_cells.Values) {
            Debug.Log("[CM:deleteAll] Need to delete cell:" + c.id);
            foreach(Hex h in c.hexes) {
                Destroy(h.obj);
                h.obj = null;
            }
            Destroy(c.rootObj);
            for(int i = 0; i < c.hexes.Length; ++i)
                c.hexes[i] = null;
        }

        m_cells.Clear();
    }

    public void handleCellMsg(CellMsg _msg) {
        switch(_msg.cmd)
        {
            case "new-cell":
                Debug.Log("[CM:handleCell] NEW CELL:" + _msg.cell);
                addCell(_msg.cell);
                break;

            case "delete-all":
                Debug.Log("[CM:handleCell] DELETE ALL CELLs");
                deleteAll();
                break;

            default:
                Debug.LogError("[CM:handleCell] UNKNOWN cmd:" + _msg.cmd);
                break;
        }
    }
}

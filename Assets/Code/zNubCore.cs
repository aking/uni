using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class zNubCore : MonoBehaviour
{
  public string m_id;
  public string m_name;

	private Queue<Msg> m_msgQueue;
  private Queue<BodyNubMsg> m_dispatchQueue;

  //------------------------------------------------------------------------
  void Start() {
    Debug.Log("[NubCore:Awake] Called:'" + m_name + "'");
    m_msgQueue = new Queue<Msg>();
  }

  //------------------------------------------------------------------------
  void OnEnable() {
    // Used for reloads
    Debug.Log("[NubCore:OnEnable] Called:'" + m_name + "'");
    if(m_msgQueue == null)
      m_msgQueue = new Queue<Msg>();
  }

  //------------------------------------------------------------------------
  internal void init(Nub _nubData) {
    m_id = _nubData.id;

    // set nub name, if valid, otherwise the id
    m_name = _nubData.core.name;
    if (m_name == null || m_name.Length == 0)
      m_name = "NUB:["+m_id+"]";
    gameObject.name = m_name;

    Debug.Log("[NubCore:init] Called:'" + m_name + "'");
  }

  //------------------------------------------------------------------------
  internal void setDispatchQueue(Queue<BodyNubMsg> _queue) {
    m_dispatchQueue = _queue;
  }

  //------------------------------------------------------------------------
  internal void dispatchMsg(string _gene, BodyMsg _msg) {
    BodyNubMsg nubMsg = new BodyNubMsg();
    nubMsg.tag = "nub";
    nubMsg.id = m_id;
    nubMsg.cmd = _gene;
    nubMsg.msg = _msg;
    m_dispatchQueue.Enqueue(nubMsg);
  }

  //------------------------------------------------------------------------
  internal void newMessage(Msg _msg) {
    m_msgQueue.Enqueue(_msg);
  }

  //------------------------------------------------------------------------
  internal int getMessageCount() {
    return m_msgQueue.Count;
  }

  //------------------------------------------------------------------------
  internal Msg peekMessage() {
    if(m_msgQueue.Count > 0)
      return m_msgQueue.Peek();
    else
      return null;
  }

  //------------------------------------------------------------------------
  internal Msg popMessage() {
    return m_msgQueue.Dequeue();
  }
}
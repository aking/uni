using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour {

	void Start () {
    Debug.Log("[TestScript:Start] Called");

    ResourceLoad();
	}
	
	void Update () {
	}

  // Test Resource Loading
  void ResourceLoad() {
    TextAsset htmlText = Resources.Load<TextAsset>("index");
    if(htmlText == null) {
      Debug.Assert(false, "[TestScript:ResourceLoad] UNABLE to load html file");
    } else {
      Debug.Log("[TestScript:ResourceLoad] html file LOADED:" + htmlText.name +
        "  bytes:" + htmlText.text);
    }

    TextAsset jsText = Resources.Load<TextAsset>("compiled/v01d.js");
    if(jsText == null) {
      Debug.Log("[TestScript:ResourceLoad] UNABLE to load js file");
    } else {
      Debug.Log("[TestScript:ResourceLoad] js file LOADED:" + jsText.name +
        "  bytes:" + jsText.text);
    }
  }
}
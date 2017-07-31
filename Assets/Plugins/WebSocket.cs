using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class WebSocket {
  // Native Implementation interface
#if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
  [DllImport("WebSocket")]
  private static extern void _init();

  [DllImport("WebSocket")]
  private static extern void _tick();
#elif UNITY_IOS   // NOTE: will also match EDITOR_OSX!!!
  [DllImport("__Internal")]
  private static extern void _init();

  [DllImport("__Internal")]
  private static extern void _tick();
#else
    private static void _init() {Debug.Log("[WebSocket:_init] IMPLEMENT ME");}
    private static void _tick() {}
#endif

  // Public interface
  public static void init() {
#if UNITY_IOS
    Debug.Log("[WebSocket:init] IOS");
#endif
#if UNITY_STANDALONE_OSX
    Debug.Log("[WebSocket:init] STANDALONE_OSX");
#endif
#if UNITY_EDITOR_OSX
    Debug.Log("[WebSocket:init] EDITOR_OSX");
#endif

    if (Application.platform != RuntimePlatform.OSXEditor) {
      _init();
    } else {
      _init();
    }
  }

  public static void tick() {
    _tick();
  }
}

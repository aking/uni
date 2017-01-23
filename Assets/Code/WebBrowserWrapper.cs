using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public class WebBrowserWrapper {
#if UNITY_STANDALONE_OSX
  private const string dllName = "Ava";
#elif UNITY_IOS
  private const string dllName = "__Internal";
#elif UNITY_STANDALONE_WIN
  private const string dllName = "Ava";
#endif

#if UNITY_STANDALONE_WIN
  public static int openBrowser() {return 0;}
  public static void closeBrowser() {}
#else
  [DllImport(dllName)]
  public static extern int openBrowser();

  [DllImport(dllName)]
  public static extern void closeBrowser();
#endif
}

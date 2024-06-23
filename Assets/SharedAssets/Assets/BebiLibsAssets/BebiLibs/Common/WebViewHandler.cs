// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using VoxelBusters.EssentialKit;
// using VoxelBusters.CoreLibrary;
// namespace BebiLibs
// {
//     public class WebViewHandler : MonoBehaviour
//     {
//         internal WebView webView;
//         public string scheme = "bebi.family.kids.learning.games";

//         public void Show(string urlToOpen)
//         {
//             if (this.webView == null)
//             {
//                 this.webView = WebView.CreateInstance();
//                 this.webView.SetNormalizedFrame(Screen.safeArea);
//                 this.webView.SetFullScreen();
//                 this.webView.Style = WebViewStyle.Browser;
//                 this.webView.AddURLScheme(this.scheme);
//                 this.webView.AutoShowOnLoadFinish = true;
//             }

//             this.webView.Show();
//             this.webView.LoadURL(URLString.URLWithPath(urlToOpen));
//         }

//         public void Hide()
//         {
//             if (this.webView != null)
//                 this.webView.Hide();
//             else
//                 Debug.LogWarning("Unable To Hide, WebView Is Null");
//         }

//         private void OnEnable()
//         {
//             WebView.OnShow += this.OnWebViewShow;
//             WebView.OnHide += this.OnWebViewHide;
//             WebView.OnLoadStart += this.OnWebViewLoadStart;
//             WebView.OnLoadFinish += this.OnWebViewLoadFinish;
//             WebView.OnURLSchemeMatchFound += this.OnURLSchemeMatchFound;
//         }

//         private void OnDisable()
//         {
//             WebView.OnShow -= this.OnWebViewShow;
//             WebView.OnHide -= this.OnWebViewHide;
//             WebView.OnLoadStart -= this.OnWebViewLoadStart;
//             WebView.OnLoadFinish -= this.OnWebViewLoadFinish;
//             WebView.OnURLSchemeMatchFound -= this.OnURLSchemeMatchFound;
//         }


//         private void OnWebViewShow(WebView webView)
//         {
//             Debug.Log("Webview is being displayed : " + webView);
//         }

//         private void OnWebViewHide(WebView webView)
//         {
//             Debug.Log("Webview is hidden : " + webView);
//             webView.ClearCache();
//         }

//         private void OnWebViewLoadStart(WebView webView)
//         {
//             Debug.Log("Webview loading started hidden : " + webView);
//         }

//         private void OnWebViewLoadFinish(WebView webView, Error error)
//         {
//             Debug.Log("Webview loading finished hidden : " + error);
//         }

//         private void OnURLSchemeMatchFound(string result)
//         {
//             Debug.Log("Webview loading URL Schema: " + result);
//         }
//     }
// }

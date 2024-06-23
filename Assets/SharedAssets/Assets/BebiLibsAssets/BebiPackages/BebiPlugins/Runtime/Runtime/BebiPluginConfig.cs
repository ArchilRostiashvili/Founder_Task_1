using UnityEngine;
using BebiLibs;

[CreateAssetMenu(fileName = "BebiPluginConfig", menuName = "BebiLibs/BebiPlugin/BebiPluginConfig", order = 0)]
public class BebiPluginConfig : ScriptableConfig<BebiPluginConfig>
{
    public bool UseDefaultBrowser;
    public bool IsAppInstalledEditor = false;

    public string GroupID { get; internal set; }
    public string FullGroupID { get; internal set; }
    public string BundleID { get; internal set; }
}
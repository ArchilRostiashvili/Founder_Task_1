using BebiLibs.ServerConfigLoaderSystem.Core;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


[CreateAssetMenu(fileName = "ModuleConfig", menuName = "Registration/ModuleConfig", order = 0)]
public class ModuleConfig : BaseRequestData
{
    public List<Module> modules;
    public List<FairyTaleCustomData> fairyTales;



    public override void LoadDataFromMemory()
    {
        base.LoadDataFromMemory();
        if (System.DateTime.UtcNow - lastUpdateTime.DateTime > System.TimeSpan.FromDays(1))
        {
            _isDataLoadSuccessfull = false;
        }
    }

    public override void ResetData()
    {
        modules.Clear();
        fairyTales.Clear();
        _isDataLoadSuccessfull = false;
    }

    protected override bool ValidateDeserializedData()
    {
        return modules.Count > 0 && fairyTales.Count > 0;
    }
}


[System.Serializable]
public class CategoryList
{
    public string id;
    public bool isLocked;
}

[System.Serializable]
public class FairyTaleCustomData
{
    public string id;
    public string name;
    public bool isLocked;
    public string thumb;
    public string audio;
    public string video;
    public List<string> subtitles;

    public FairyTaleCustomData Copy()
    {
        FairyTaleCustomData cd = new FairyTaleCustomData()
        {
            id = this.id,
            name = this.name,
            isLocked = this.isLocked,
            thumb = this.thumb,
            audio = this.audio,
            video = this.video,
            subtitles = this.subtitles.Select(x => x).ToList(),
        };
        return cd;
    }
}



[System.Serializable]
public class Module
{
    public string moduleName;
    public bool isEnable;
    public List<CategoryList> categoryList;
    public List<string> deny;
}

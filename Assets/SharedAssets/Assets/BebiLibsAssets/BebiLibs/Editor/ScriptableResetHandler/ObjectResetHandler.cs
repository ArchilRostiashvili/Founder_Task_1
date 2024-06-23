using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Build;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;
using BebiLibs.ServerConfigLoaderSystem.Core;
using BebiLibs;
using System.Reflection;

public class ObjectResetHandler : IPreprocessBuildWithReport
{
    public int callbackOrder { get { return 0; } }

    public delegate void ResetObjectDelegate(Object objectToReset);

    [InitializeOnLoadMethod]
    static void RegisterResets()
    {
        EditorApplication.playModeStateChanged += ResetSOsWithIResetOnExitPlay;
    }

    public void OnPreprocessBuild(BuildReport report)
    {
        ResetDataPreBuild();
    }

    static void ResetSOsWithIResetOnExitPlay(PlayModeStateChange change)
    {
        if (change == PlayModeStateChange.ExitingPlayMode)
        {
            ResetDataOnPlayerExit();
        }
    }

    [MenuItem("BebiLibs/Resetters/Manual Reset On Pre Build")]
    public static void ResetDataPreBuild()
    {
        var types = GetTypesWithInterface<IResetOnPreBuild>();
        foreach (var item in types)
        {
            List<Object> objects = FindAssets(item);
            ResetObjects(objects, ResetObjectOnPlay);
        }
    }

    [MenuItem("BebiLibs/Resetters/Manual Reset On Exit Play")]
    public static void ResetDataOnPlayerExit()
    {
        var types = GetTypesWithInterface<IResetOnExitPlay>();
        foreach (var item in types)
        {
            List<Object> objects = FindAssets(item);
            ResetObjects(objects, ResetObjectOnExitPlay);
        }
    }


    static void ResetObjects(List<Object> objectsToReset, ResetObjectDelegate resetObjectDelegate)
    {
        foreach (var item in objectsToReset)
        {
            resetObjectDelegate(item);
        }
    }

    public static void ResetObjectOnPlay(Object objectToReset)
    {
        if (!TryGetInterface(objectToReset, out IResetOnPreBuild item))
        {
            return;
        }

        Debug.Log($"Resetting {objectToReset.name}");
        item.ResetOnPreBuild();
        EditorUtility.SetDirty(objectToReset);
    }

    public static void ResetObjectOnExitPlay(Object objectToReset)
    {
        if (!TryGetInterface(objectToReset, out IResetOnExitPlay item))
        {
            return;
        }

        Debug.Log($"Resetting {objectToReset.name}");
        item.ResetOnExitPlay();
        EditorUtility.SetDirty(objectToReset);
    }


    public static List<Object> FindAssets(System.Type type)
    {
        var guids = AssetDatabase.FindAssets($"t:{type.Name}");
        var assets = new List<Object>(guids.Length);
        for (int i = 0; i < guids.Length; i++)
        {
            var path = AssetDatabase.GUIDToAssetPath(guids[i]);
            var asset = AssetDatabase.LoadAssetAtPath(path, type);
            if (asset != null)
            {
                assets.Add(asset);
            }
        }
        return assets;
    }


    public static bool TryGetInterface<T>(Object gObj, out T result)
    {
        if (!typeof(T).IsInterface)
        {
            result = default;
            return false;
        }

        bool isObjectImplementsInterface = gObj.GetType().GetInterfaces().Any(k => k == typeof(T));
        if (isObjectImplementsInterface)
        {
            result = (T)(object)gObj;
            return true;
        }
        else
        {
            result = default;
            return false;
        }
    }

    public static IEnumerable<System.Type> GetTypesWithInterface<T>()
    {
        Assembly[] asm = System.AppDomain.CurrentDomain.GetAssemblies();
        List<System.Type> types = new List<System.Type>(asm.Length);
        foreach (Assembly a in asm)
        {
            types.AddRange(GetTypesWithInterface<T>(a));
        }
        return types;
    }

    public static IEnumerable<System.Type> GetTypesWithInterface<T>(Assembly asm)
    {
        var it = typeof(T);
        return GetLoadableTypes(asm).Where(x => x != it && it.IsAssignableFrom(x)).ToList();
    }

    public static IEnumerable<System.Type> GetLoadableTypes(Assembly assembly)
    {
        if (assembly == null) throw new System.ArgumentNullException("assembly");
        try
        {
            return assembly.GetTypes();
        }
        catch (ReflectionTypeLoadException e)
        {
            return e.Types.Where(t => t != null);
        }
    }

}
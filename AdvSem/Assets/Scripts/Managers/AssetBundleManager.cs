using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public static class AssetBundleManager
{
    public static string assetBundleDirectory = "AssetBundles";

    public static Dictionary<string, AssetBundle> assetBundleDictionary;

    public static void LoadAssetBundle(string bundleName)
    {
        if (assetBundleDictionary == null)
        {
            assetBundleDictionary = new Dictionary<string, AssetBundle>();
        }

        if (assetBundleDictionary.ContainsKey(bundleName))
            return;

        string path = Path.Combine(Application.streamingAssetsPath, assetBundleDirectory);

        AssetBundle loadedBundle = AssetBundle.LoadFromFile(Path.Combine(path, bundleName));

        if (loadedBundle == null)
        {
            Debug.LogError("Failed to load AssetBundle");
        }
        else
        {
            assetBundleDictionary.Add(bundleName, loadedBundle);
        }
    }

    public static AssetBundle GetAssetBundle(string bundleName)
    {
        return assetBundleDictionary[bundleName];
    }
}

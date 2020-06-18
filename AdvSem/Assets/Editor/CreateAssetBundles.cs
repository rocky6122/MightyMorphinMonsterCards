using UnityEditor;
using System.IO;
using UnityEngine;

public class CreateAssetBundles
{
    [MenuItem("Assets/Build Asset Bundles")]
    static void BuildAllAssetBundles()
    {
        if (!Directory.Exists(AssetBundleManager.assetBundleDirectory))
        {
            Directory.CreateDirectory(Path.Combine(Application.streamingAssetsPath, AssetBundleManager.assetBundleDirectory));
        }

        BuildPipeline.BuildAssetBundles(Path.Combine(Application.streamingAssetsPath, AssetBundleManager.assetBundleDirectory), BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);
    }
}

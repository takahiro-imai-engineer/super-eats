// Copyright 2020 Google LLC
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     https://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Google.Android.AppBundle.Editor;
using Google.Android.AppBundle.Editor.AssetPacks;

public class BuildRuntimeAssets
{
    private const string assetPacksName = "AssetPacks";

    private const int MAX_ASSET_PACK_NUM = 50;

    [MenuItem("Tools/Save AssetPackConfig")]
    public static void BuildRTAssets_AssetPacks_Scripted()
    {

        // Create an AssetPackConfig and start creating asset packs
        AssetPackConfig assetPackConfig = new AssetPackConfig();

        // Create asset packs using AssetBundles
        string assetBundlePath = Path.Combine(Application.dataPath, assetPacksName);

        // PlayAssetDeliveryに含めるアセットを設定する
        // assetPackConfig.AddAssetsFolder("init_assetbundles", assetBundlePath, AssetPackDeliveryMode.InstallTime);
        List<string> assetBundleList = new List<string>()
        {
            "title_base",
            "charactericon",
            "foodicon",
            "shopicon",
            "skybox",
            "sound",
            "tips",
            "tutorial_image"
        };
        foreach (var item in assetBundleList)
        {
            assetPackConfig.AddAssetBundle(Path.Combine(assetBundlePath, item), AssetPackDeliveryMode.InstallTime);
        }
        if (assetPackConfig.AssetPacks.Count > MAX_ASSET_PACK_NUM)
        {
            throw new System.OperationCanceledException($"AssetPackConfig SetUp Failed. AssetPacks.Count={assetPackConfig.AssetPacks.Count} > {MAX_ASSET_PACK_NUM}");
        }

        // Configures the build system to use the newly created 
        // assetPackConfig when calling Google > Build and Run or 
        // Google > Build Android App Bundle.
        AssetPackConfigSerializer.SaveConfig(assetPackConfig);
    }
}

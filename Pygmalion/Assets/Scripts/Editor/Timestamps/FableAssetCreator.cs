using UnityEngine;
using UnityEditor;

public static class FableAssetCreator {
	public static string folderPath = "Assets/Prefabs/Narration/";

	public static bool CreateAsset(FableAsset fable) {
		FableAsset newFable = FillAsset(fable);
		AssetDatabase.CreateAsset(fable, folderPath + fable.fableTitle + ".asset");
		AssetDatabase.SaveAssets();

		EditorUtility.FocusProjectWindow();

		return true;
	}

	public static FableAsset LoadAsset(string fablePath) {
		return AssetDatabase.LoadAssetAtPath(fablePath, typeof(FableAsset)) as FableAsset;
	}

	public static FableAsset FillAsset(FableAsset fable) {
		FableAsset fableInstance = ScriptableObject.CreateInstance<FableAsset>();

		fableInstance.audioClip = fable.audioClip;
		fableInstance.fableText = fable.fableText;
		fableInstance.fableTitle = fable.fableTitle;
		fableInstance.markers = fable.markers;

		return fableInstance;
	}
}
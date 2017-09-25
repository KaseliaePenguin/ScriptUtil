using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public static class TimelineScriptUtil
{
	static string rootPath = "/Assets/Plugins/TimelineKit";
	static string playableAssetTemplatePath = rootPath + "/Templates/PlayableAssetTemplate.txt";
	static string playableBehaviourTemplatePath = rootPath + "/Templates/PlayableBehaviourTemplate.txt";

	[MenuItem("PhantomIsland/ScriptUtil/Timeline/PlayableScript")]
	[MenuItem("Assets/Create/Playables/BothPlayableScript", false, 101)]
	public static void PlayableScript()
	{
		Create();
	}

	static string Create()
	{
		string resourceAssetPath = Directory.GetCurrentDirectory() + playableAssetTemplatePath;
		string resourceBehaviourPath = Directory.GetCurrentDirectory() + playableBehaviourTemplatePath;
		string templatePlayableAssetTexts = File.ReadAllText(resourceAssetPath);
		string templatePlayableBehaviourTexts = File.ReadAllText(resourceBehaviourPath);

		var absolutePath = EditorUtility.SaveFilePanel(
			"Choose name for Script",
			GetSelectedPathInProjectsTab(),
			"NewPlayableScript.cs",
			"cs");

		if(absolutePath == "")
		{
			return "";
		}

		var className = Path.GetFileNameWithoutExtension(absolutePath);
		var directoryPath = Path.GetDirectoryName(absolutePath);

		File.WriteAllText(directoryPath + "/" + className + "Asset.cs", templatePlayableAssetTexts.Replace("#SCRIPTNAME#", className));
		File.WriteAllText(directoryPath + "/" + className + "Behaviour.cs", templatePlayableBehaviourTexts.Replace("#SCRIPTNAME#", className));

		AssetDatabase.Refresh();

		return className;
	}

	static string GetSelectedPathInProjectsTab()
	{
		var paths = new List<string>();

		UnityEngine.Object[] selectedAssets = Selection.GetFiltered(
			typeof(UnityEngine.Object), SelectionMode.Assets);

		foreach(var item in selectedAssets)
		{
			var relativePath = AssetDatabase.GetAssetPath(item);

			if(!string.IsNullOrEmpty(relativePath))
			{
				var fullPath = Path.GetFullPath(Path.Combine(
					Application.dataPath, Path.Combine("..", relativePath)));

				paths.Add(fullPath);
			}
		}

		return paths[0];
	}
}

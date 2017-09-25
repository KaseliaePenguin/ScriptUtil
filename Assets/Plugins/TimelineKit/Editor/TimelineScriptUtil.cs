using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text.RegularExpressions;

public static class TimelineScriptUtil
{
	static string append = "//APPEND:";
	static string appendRef = "//APPENDREF:";
	static string rootPath = "/Assets/Plugins/TimelineKit";
	static string playableAssetTemplatePath = rootPath + "/Templates/PlayableAssetTemplate.txt";
	static string playableBehaviourTemplatePath = rootPath + "/Templates/PlayableBehaviourTemplate.txt";

	[MenuItem("PhantomIsland/ScriptUtil/Timeline/PlayableScript")]
	[MenuItem("Assets/Create/Playables/BothPlayableScript", false, 101)]
	public static void PlayableScript()
	{
		Create();
	}

	public static void PlayableScriptFromEditor(List<ReferenceData> refData,
												List<string> refNamespace)
	{
		var className = Create();
		WriteRefData(className, refData, refNamespace);

		AssetDatabase.SaveAssets();
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

	static void WriteRefData(string className, List<ReferenceData> refData, List<string> refNamespace)
	{
		var paths = AssetDatabase.GetAllAssetPaths();

		for(int i = 0;i < paths.Length;i++)
		{
			if(Regex.Match(paths[i], string.Format(className + "Asset")).Success)
			{
				var current = Directory.GetCurrentDirectory() + "/" + paths[i];
				var playableAsset = File.ReadAllText(current);

				foreach(var ns in refNamespace)
				{
					playableAsset = playableAsset.Insert(0, string.Format("using {0};" + Environment.NewLine, ns));
				}

				foreach(var rd in refData)
				{
					int appendIndex = playableAsset.IndexOf(append);

					string insertText = "";
					string insertRefText = "";

					if(rd.ReferenceType == ReferenceType.ExposedReference)
					{
						insertText = string.Format(
							"public ExposedReference<{0}> {1};" + 
							Environment.NewLine + 
							"\t", 
							rd.ClassType, 
							rd.PropertyName);

						insertRefText = string.Format(
							"behaviour.{0} = {0}.Resolve(graph.GetResolver());" +
							Environment.NewLine + 
							"\t\t", 
							rd.PropertyName);
					}
					else
					{
						insertText = string.Format(
							"public {0} {1};" + 
							Environment.NewLine +
							"\t", 
							rd.ClassType, 
							rd.PropertyName);

						insertRefText = string.Format(
							"behaviour.{0} = {0};" +
							Environment.NewLine +
							"\t\t",
							rd.PropertyName);
					}

					playableAsset = playableAsset.Insert(appendIndex, insertText);

					int appendRefIndex = playableAsset.IndexOf(appendRef);

					playableAsset = playableAsset.Insert(appendRefIndex, insertRefText);
				}

				File.WriteAllText(current, playableAsset.Replace(append, "").Replace(appendRef, ""));
			}

			if(Regex.Match(paths[i], string.Format(className + "Behaviour")).Success)
			{
				var current = Directory.GetCurrentDirectory() + "/" + paths[i];
				var playableBehaviour = File.ReadAllText(current);

				foreach(var ns in refNamespace)
				{
					playableBehaviour = playableBehaviour.Insert(0, string.Format("using {0};" + Environment.NewLine, ns));
				}

				foreach(var rd in refData)
				{
					int appendIndex = playableBehaviour.IndexOf(append);

					string insertText = "";

					insertText = string.Format(
						"public {0} {1};" +
						Environment.NewLine +
						"\t",
						rd.ClassType,
						rd.PropertyName);

					playableBehaviour = playableBehaviour.Insert(appendIndex, insertText);
				}

				File.WriteAllText(current, playableBehaviour.Replace(append, ""));
			}
		}
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

		if(paths.Count == 0) return Directory.GetCurrentDirectory();
		else return paths[0];
	}
}

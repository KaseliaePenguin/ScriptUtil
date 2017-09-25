using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class TimelineScriptEditor : EditorWindow
{
	bool fold = true;

	string addNamespace = "UnityEngine.UI";
	string addType = "Text";
	string propertyName = "text";
	ReferenceType referenceType = ReferenceType.ExposedReference;

	List<string> nameSpaceList = new List<string>();
	List<ReferenceData> referenceList = new List<ReferenceData>();

	[MenuItem("PhantomIsland/Window/TimelineScriptEditor")]
	static void Open()
	{
		GetWindow<TimelineScriptEditor>();
	}

	void OnGUI()
	{
		EditorGUILayout.LabelField("PlayableTrackScriptを生成します");

		EditorGUILayout.LabelField("■namespace設定");
		EditorGUILayout.LabelField("");

		EditorGUILayout.BeginHorizontal();

		addNamespace = EditorGUILayout.TextField("参照するnamespace", addNamespace);

		if(GUILayout.Button("AddNameSpace"))
		{
			AddNamespace();
		}

		EditorGUILayout.EndHorizontal();

		EditorGUILayout.LabelField("");
		EditorGUILayout.LabelField("■変数設定");
		EditorGUILayout.LabelField("");

		addType = EditorGUILayout.TextField("追加する変数の型", addType);
		propertyName = EditorGUILayout.TextField("追加する変数名", propertyName);

		referenceType = (ReferenceType)EditorGUILayout.EnumPopup(referenceType);

		using(new EditorGUILayout.HorizontalScope())
		{
			if(GUILayout.Button("AddProperty"))
			{
				AddProperty();
			}

			if(GUILayout.Button("RemoveProperty"))
			{
				RemoveProperty();
			}
		}

		fold = EditorGUILayout.Foldout(fold, "現在の設定");
		if(fold)
		{
			EditorGUILayout.BeginVertical();

			foreach(var ns in nameSpaceList)
			{
				EditorGUILayout.LabelField(string.Format("using {0}", ns));
			}

			foreach(var rd in referenceList)
			{
				if(rd.ReferenceType == ReferenceType.Public)
				{
					EditorGUILayout.LabelField(string.Format("public {0} {1}", rd.ClassType, rd.PropertyName));
				}
				else
				{
					EditorGUILayout.LabelField(string.Format("public ExposedReference<{0}> {1}", rd.ClassType, rd.PropertyName));
				}
			}

			EditorGUILayout.EndVertical();
		}

		if(GUILayout.Button("CreateScript"))
		{
			Create();
		}
	}

	void AddNamespace()
	{
		nameSpaceList.Add(addNamespace);
	}

	void AddProperty()
	{
		var rd = new ReferenceData()
		{
			ClassType = addType,
			PropertyName = propertyName,
			ReferenceType = referenceType
		};

		referenceList.Add(rd);
	}

	void RemoveProperty()
	{
		referenceList.RemoveAt(referenceList.Count - 1);
	}

	void Create()
	{
		TimelineScriptUtil.PlayableScriptFromEditor(referenceList, nameSpaceList);
		referenceList.Clear();
		nameSpaceList.Clear();
	}
}

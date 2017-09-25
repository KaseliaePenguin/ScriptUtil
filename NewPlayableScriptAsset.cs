using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

[System.Serializable]
public class NewPlayableScriptAsset : PlayableAsset
{
	//APPEND:

	public override Playable CreatePlayable(PlayableGraph graph, GameObject go) 
	{
		var behaviour = new NewPlayableScriptBehaviour();

		//APPENDREF:

		return ScriptPlayable<NewPlayableScriptBehaviour>.Create(graph, behaviour);
	}
}

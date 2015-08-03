using UnityEngine;
using UnityEditor;
using System.Collections;

public class CharacterDefineBuilder : MonoBehaviour
{
	[MenuItem ("Assets/Create/CharacterDefineBuilder")]
	// Use this for initialization

	public static void CreateAsset ()
	{
		CreateAsset<CharacterDefine> ();
	}
	
	// Update is called once per frame
	public static void CreateAsset<Type> () where Type : ScriptableObject
	{
		Type item = ScriptableObject.CreateInstance<Type> ();
		string path = AssetDatabase.GenerateUniqueAssetPath ("Assets/Define/" + typeof(Type) + ".asset");
		AssetDatabase.CreateAsset (item, path);
		AssetDatabase.SaveAssets ();
		EditorUtility.FocusProjectWindow ();
		Selection.activeObject = item;
	}
}

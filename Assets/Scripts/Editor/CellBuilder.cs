using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class CellBuilder : EditorWindow
{
	[MenuItem ("Window/CellBuilder")]
	// Use this for initialization

	public static void Open ()
	{
		var window = EditorWindow.GetWindow<CellBuilder> ("CellBuilder");
		window.Show ();
	}

	public Object Cell;
	public Object OutputFolder;

	public enum CellType
	{
		Tile,
		Wall,
		Door,
	}

	CellType enumPopup = CellType.Tile;
	private Vector2 ScrollView;

	private string PrefabPath;
	private string FolderPath;


	void OnGUI ()
	{
		EditorGUILayout.LabelField ("Cell生成用Editor");
		ScrollView = EditorGUILayout.BeginScrollView (ScrollView, GUI.skin.box);
		{
			EditorGUILayout.HelpBox ("Cellとして定義したいGameObjectをProject Windowから選択してください。", MessageType.Info);
			Cell = EditorGUILayout.ObjectField ("Cell Prefab", Cell, typeof(Object), false);

			if (Cell != null && Cell is GameObject) {
			
				string prefabPath = AssetDatabase.GetAssetPath (Cell);
				PrefabPath = EditorGUILayout.TextField ("PrefabPath", prefabPath);

				EditorGUILayout.HelpBox ("CellのTypeを選択してください", MessageType.Info);
				enumPopup = (CellType)EditorGUILayout.EnumPopup ("CellType", (System.Enum)enumPopup);

				EditorGUILayout.HelpBox ("CellDefineを生成する先のフォルダを選択してください。", MessageType.Info);
				OutputFolder = EditorGUILayout.ObjectField ("Output Folder", OutputFolder, typeof(Object), false);

				if (OutputFolder != null) {
					string folderPath = AssetDatabase.GetAssetPath (OutputFolder);
					FolderPath = EditorGUILayout.TextField ("FolderPath", folderPath);

					EditorGUILayout.HelpBox ("上記の内容でCellDefineを生成してよければGenerateボタンを押してください", MessageType.Info);
					if (GUILayout.Button ("Generate")) {
						CellDefine define = ScriptableObject.CreateInstance<CellDefine> ();
						define.path = PrefabPath;

						switch (enumPopup) {
						case CellType.Tile:
							define.type = "Tile";
							break;
						case CellType.Wall:
							define.type = "Wall";
							break;
						case CellType.Door:
							define.type = "Door";
							break;
						}

						string assetPath = AssetDatabase.GenerateUniqueAssetPath (FolderPath + "/" + typeof(CellDefine) + define.type + ".asset");
						AssetDatabase.CreateAsset (define, assetPath);
						AssetDatabase.SaveAssets ();
						EditorUtility.FocusProjectWindow ();
						Selection.activeObject = define;
					}
				}

			}
			EditorGUILayout.EndScrollView ();
		}

		EditorGUILayout.HelpBox ("すべての参照をリセットします", MessageType.Info);
		if (GUILayout.Button ("Reset")) {
			Cell = null;
			OutputFolder = null;
			PrefabPath = null;
			FolderPath = null;
		}
	}

}

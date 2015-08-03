using UnityEngine;
using UnityEditor;
using System.Collections;

public class RoomBuilder : EditorWindow
{
	[MenuItem ("Window/RoomBuilder")]
	// Use this for initialization

	public static void Open ()
	{
		var window = EditorWindow.GetWindow<RoomBuilder> ("RoomBuilder");
		window.Show ();
	}

	private string RoomName = "";

	public Vector2 Depth;
	public Object CellWall;
	public Object CellTile;
	public Object CellDoor;

	public Object OutputFolder;

	private Vector2 ScrollView;
	private string FolderPath;

	void OnGUI ()
	{
		EditorGUILayout.LabelField ("Room生成用Editor");
		ScrollView = EditorGUILayout.BeginScrollView (ScrollView, GUI.skin.box);
		{
			EditorGUILayout.HelpBox ("このRoomの名前を設定してください", MessageType.Info);
			RoomName = EditorGUILayout.TextField ("RoomName", RoomName);

			EditorGUILayout.HelpBox ("このRoomの奥行きを設定してください", MessageType.Info);
			Depth = EditorGUILayout.Vector2Field ("Depth", Depth);

			EditorGUILayout.HelpBox ("壁用CellDefineをProject Windowから選択してください。", MessageType.Info);
			CellWall = EditorGUILayout.ObjectField ("Wall CellDefine", CellWall, typeof(Object), false);

			EditorGUILayout.HelpBox ("タイル用CellDefineをProject Windowから選択してください。", MessageType.Info);
			CellTile = EditorGUILayout.ObjectField ("Tile CellDefine", CellTile, typeof(Object), false);

			EditorGUILayout.HelpBox ("ドア用CellDefineをProject Windowから選択してください。", MessageType.Info);
			CellDoor = EditorGUILayout.ObjectField ("Door CellDefine", CellDoor, typeof(Object), false);

			if (RoomName != null && CellWall != null && CellTile != null && CellDoor != null) {
				
				EditorGUILayout.HelpBox ("RoomDefineを生成する先のフォルダを選択してください。", MessageType.Info);
				OutputFolder = EditorGUILayout.ObjectField ("Output Folder", OutputFolder, typeof(Object), false);

				if (OutputFolder != null) {
					string folderPath = AssetDatabase.GetAssetPath (OutputFolder);
					FolderPath = EditorGUILayout.TextField ("FolderPath", folderPath);

					EditorGUILayout.HelpBox ("上記の内容でRoomDefineを生成してよければGenerateボタンを押してください", MessageType.Info);
					if (GUILayout.Button ("Generate")) {
						RoomDefine define = ScriptableObject.CreateInstance<RoomDefine> ();

						define.Name = RoomName;
						define.Depth = Depth;
						define.Tile = CellTile as CellDefine;
						define.Door = CellDoor as CellDefine;
						define.Wall = CellWall as CellDefine;

						string assetPath = AssetDatabase.GenerateUniqueAssetPath (FolderPath + "/" + typeof(RoomDefine) + define.Name + ".asset");
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
			CellWall = null;
			CellTile = null;
			CellDoor = null;
			OutputFolder = null;
			FolderPath = null;
			RoomName = null;
		}
	}
}

using UnityEngine;
using UnityEditor;
using System.Collections;

public class MapBuilder : EditorWindow
{
	[MenuItem ("Window/MapBuilder")]
	// Use this for initialization

	public static void Open ()
	{
		var window = EditorWindow.GetWindow<MapBuilder> ("MapBuilder");
		window.Show ();
	}

	private string MapName;
	public Vector2 MapDepth;
	public Vector2 RoomDepthMax;

	public Object Room;
	public Object Corridor;

	public Object OutputFolder;

	private Vector2 ScrollView;
	private string FolderPath;

	void OnGUI ()
	{
		EditorGUILayout.LabelField ("Map生成用Editor");
		ScrollView = EditorGUILayout.BeginScrollView (ScrollView, GUI.skin.box);
		{
			EditorGUILayout.HelpBox ("このMapの名前を設定してください", MessageType.Info);
			MapName = EditorGUILayout.TextField ("MapName", MapName);

			EditorGUILayout.HelpBox ("このMapの奥行きを設定してください", MessageType.Info);
			MapDepth = EditorGUILayout.Vector2Field ("Map Depth", MapDepth);

			EditorGUILayout.HelpBox ("RoomDefineをProject Windowから選択してください。", MessageType.Info);
			Room = EditorGUILayout.ObjectField ("RoomDefine", Room, typeof(Object), false);

			EditorGUILayout.HelpBox ("Roomの最大奥行きを設定してください", MessageType.Info);
			RoomDepthMax = EditorGUILayout.Vector2Field ("Room Depth Max", RoomDepthMax);

			if (Room != null && MapName != null) {
				
				EditorGUILayout.HelpBox ("MapDefineを生成する先のフォルダを選択してください。", MessageType.Info);
				OutputFolder = EditorGUILayout.ObjectField ("Output Folder", OutputFolder, typeof(Object), false);

				if (OutputFolder != null) {
					string folderPath = AssetDatabase.GetAssetPath (OutputFolder);
					FolderPath = EditorGUILayout.TextField ("FolderPath", folderPath);

					EditorGUILayout.HelpBox ("上記の内容でRoomDefineを生成してよければGenerateボタンを押してください", MessageType.Info);
					if (GUILayout.Button ("Generate")) {
						MapDefine define = ScriptableObject.CreateInstance<MapDefine> ();

						define.MapName = MapName;
						define.MapDepth = MapDepth;
						define.RoomDepthMax = RoomDepthMax;
						define.Room = Room as RoomDefine;

						string assetPath = AssetDatabase.GenerateUniqueAssetPath (FolderPath + "/" + typeof(MapDefine) + define.MapName + ".asset");
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
			OutputFolder = null;
			FolderPath = null;
			Room = null;
			Corridor = null;
			MapName = null;
		}
	}
}

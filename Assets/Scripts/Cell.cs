using UnityEngine;

/// <summary>
/// グリッドマップ用セルクラス
/// </summary>
public class Cell : MonoBehaviour
{
	private bool _isActive;
	private GameObject _marker;
	private Vector2 _id;
	private GameObject _obj;

	/// <summary>
	/// このCellがActiveかどうかをboolで返す
	/// </summary>
	public bool GetIsActive ()
	{
		return _isActive;
	}

	/// <summary>
	/// CellのIdを返す
	/// </summary>
	public Vector2 GetID ()
	{
		return _id;
	}

	/// <summary>
	/// Cellに配置されたGameObjectを返す
	/// </summary>
	public GameObject GetObj ()
	{
		return _obj;
	}

	/// <summary>
	/// CellにGameObjectを配置
	/// </summary>
	public void SetObj (GameObject obj)
	{
		_obj = obj;
	}

	/// <summary>
	/// 接触感知用GameObjectの生成
	/// </summary>
	public void CreateCellData (Vector2 id)
	{
		_id = id;
		_marker = GameObject.CreatePrimitive (PrimitiveType.Cube);
		_marker.transform.SetParent (transform);
		_marker.transform.localPosition = new Vector3 (0, 0, 0);
		//_marker.transform.localScale = Vector3.zero;
	}

	/// <summary>
	/// 接触感知用GameObjectが反応しなかった場合は削除
	/// </summary>
	public void DestroyMaker ()
	{
		//if (!_isActive)
		Destroy (_marker);
	}

	/// <summary>
	/// 衝突判定コールバック
	/// </summary>
	public void OnTriggerEnter (Collider other)
	{
		_isActive = true;
	}
}

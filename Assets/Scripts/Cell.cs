using UnityEngine;


public class Cell : MonoBehaviour
{
	[SerializeField] private bool _isActive;
	private GameObject _marker;
	private Vector2 _id;
	private GameObject _obj;

	/// <summary>
	/// このCellがActiveかどうかをboolで返します
	/// </summary>
	public bool GetIsActive ()
	{
		return _isActive;
	}

	public Vector2 GetID ()
	{
		return _id;
	}

	public GameObject GetObj ()
	{
		return _obj;
	}

	/// <summary>
	/// Sets the object.
	/// </summary>
	/// <param name="obj">GameObject.</param>
	public void SetObj (GameObject obj)
	{
		_obj = obj;
	}

	public void CreateCellData (Vector2 id)
	{
		_id = id;
		_marker = GameObject.CreatePrimitive (PrimitiveType.Cube);
		_marker.transform.SetParent (transform);
		_marker.transform.localPosition = new Vector3 (0, 0, 0);
		//_marker.transform.localScale = Vector3.zero;
	}

	public void DestroyMaker ()
	{
		if (!_isActive)
			Destroy (_marker);
	}

	public void OnTriggerEnter (Collider other)
	{
		_isActive = true;
	}
}

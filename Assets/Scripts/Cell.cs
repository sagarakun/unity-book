using UnityEngine;
using System.Collections;

public class Cell : MonoBehaviour
{
	public string type;
	public int doorSetVector;
	[SerializeField] public Vector3 cellPosition;

	public void InitCell (Vector3 pos)
	{
		cellPosition = pos;
		transform.localPosition = cellPosition;
	}

	public void CreateCell ()
	{
		GameObject obj;

		switch (type) {
		case "Wall_F":
			obj = GameObject.CreatePrimitive (PrimitiveType.Capsule);
			obj.transform.SetParent (transform);
			obj.transform.localPosition = Vector3.zero;
			break;
		case "Wall_L":
			obj = GameObject.CreatePrimitive (PrimitiveType.Capsule);
			obj.transform.SetParent (transform);
			obj.transform.localPosition = Vector3.zero;
			break;
		case "Tile":
			obj = GameObject.CreatePrimitive (PrimitiveType.Cube);
			obj.transform.SetParent (transform);
			obj.transform.localPosition = Vector3.zero;
			break;
		case "Door_Entrance":
			obj = GameObject.CreatePrimitive (PrimitiveType.Cylinder);
			obj.transform.SetParent (transform);
			obj.transform.localPosition = Vector3.zero;
			break;
		case "Door_Exit":
			obj = GameObject.CreatePrimitive (PrimitiveType.Cylinder);
			obj.transform.SetParent (transform);
			obj.transform.localPosition = Vector3.zero;
			break;
		}
	}
}

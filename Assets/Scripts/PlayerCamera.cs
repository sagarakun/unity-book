using UnityEngine;
using System.Collections;

public class PlayerCamera : MonoBehaviour
{
	/* - - - - - - - - - - - - - */
	[SerializeField] private Player _player;
	private Vector3 _offsetPosition;

	/* - - - - - - - - - - - - - */
	private void Start ()
	{
		_offsetPosition = transform.position;
	}

	private void LateUpdate ()
	{
		float x = _player.transform.position.x + _offsetPosition.x;
		float y = _player.transform.position.y + _offsetPosition.y;
		float z = _player.transform.position.z + _offsetPosition.z;

		Vector3 vec = new Vector3 (x, y, z);
		transform.position = vec;
	}
}

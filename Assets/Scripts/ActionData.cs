using UnityEngine;
using System.Collections;

public class ActionData : ScriptableObject
{
	public enum enumActionType
	{
		MOVE,
		ATTACK,
		DAMAGE,
	}

	public enumActionType type;
	public Vector2 vec;
}

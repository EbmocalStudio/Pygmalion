using UnityEngine;

public static class DebugTools {
	public static void DrawRectangle(Vector2 center, float halfWidth, float halfHeight, Color color, float duration) {
		Debug.DrawLine(new Vector3(center.x - halfWidth, center.y + halfHeight, 0), new Vector3(center.x + halfWidth, center.y + halfHeight, 0), color, duration);
		Debug.DrawLine(new Vector3(center.x + halfWidth, center.y + halfHeight, 0), new Vector3(center.x + halfWidth, center.y - halfHeight, 0), color, duration);
		Debug.DrawLine(new Vector3(center.x + halfWidth, center.y - halfHeight, 0), new Vector3(center.x - halfWidth, center.y - halfHeight, 0), color, duration);
		Debug.DrawLine(new Vector3(center.x - halfWidth, center.y - halfHeight, 0), new Vector3(center.x - halfWidth, center.y + halfHeight, 0), color, duration);
	}
}
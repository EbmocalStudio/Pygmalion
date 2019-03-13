using UnityEditor;

public static class MenuItems {
	[MenuItem("Ebmocal/Timestamps Editor", false, 10)]
	private static void ShowTimestampsWindow() {
		TimestampsEditor.ShowWindow();
	}
}
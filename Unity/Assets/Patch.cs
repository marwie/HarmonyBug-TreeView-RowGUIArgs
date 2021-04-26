using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

public static class Patch
{
	// how to reproduce:

	// open profiler window
	// select "Editor" in dropdown at the top (possibly says "PlayMode")
	// enable profiling (button with circle icon to the right of Editor)
	// note tree view is rendered at the bottom if hierarchy is selected
	// click "HarmonyBug/Patch Now" in unity menu bar
	// note index out ot range exceptions in console and tree view is not rendering anymore


	// https://github.com/Unity-Technologies/UnityCsReference/blob/61f92bd79ae862c4465d35270f9d1d57befd1761/Editor/Mono/GUI/TreeView/TreeViewControl/TreeViewControl.cs#L764

	[InitializeOnLoadMethod]
	private static void Init()
	{
		Debug.Log("Reproduction steps:\n" +
		          "open profiler window\n" +
		          "select \"Editor\" in dropdown at the top (possibly says \"PlayMode\")\n" +
		          "enable profiling (button with circle icon to the right of Editor)\n" +
		          "note tree view is rendered at the bottom if hierarchy is selected\n" +
		          "click \"HarmonyBug/Patch Now\" in unity menu bar\n" +
		          "note index out ot range exceptions in console and tree view is not rendering anymore\n\n");
	}

	[MenuItem("HarmonyBug/Open Profiler")]
	private static void OpenProfiler()
	{
		var window = EditorWindow.CreateInstance(typeof(EditorWindow).Assembly.GetType("UnityEditor.ProfilerWindow")) as EditorWindow;
		window.Show();
	}

	[MenuItem("HarmonyBug/Patch Now")]
	private static void patchNow()
	{
		var inst = new Harmony("com.needle.unity_row_gui");
		Harmony.DEBUG = true;

		var t = typeof(TreeView).Assembly.GetType("UnityEditor.IMGUI.Controls.TreeView+RowGUIArgs");
		Debug.Assert(t != null, "Could not find tree view");

		var method = t.GetMethod("GetCellRect", BindingFlags.Public | BindingFlags.Instance);
		inst.Patch(method);

		Debug.Log("Patched " + method);
	}
}
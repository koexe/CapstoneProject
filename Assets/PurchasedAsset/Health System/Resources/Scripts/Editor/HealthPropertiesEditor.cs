using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This class is only used to set inspector properties correctly
/// </summary>
[CustomEditor(typeof(HealthPreferences))]
public class HealthPropertiesEditor : Editor
{
	public override void OnInspectorGUI()
	{
		HealthPreferences preferences = (HealthPreferences)target;
		serializedObject.Update();

		EditorGUILayout.PropertyField(serializedObject.FindProperty("fullHeartsContainer"));
		EditorGUILayout.PropertyField(serializedObject.FindProperty("emptyHeartsContainer"));
		EditorGUILayout.Space();

		EditorGUILayout.PropertyField(serializedObject.FindProperty("fullHeartSprite"));
		EditorGUILayout.PropertyField(serializedObject.FindProperty("emptyHeartSprite"));
		EditorGUILayout.Space();

		EditorGUILayout.PropertyField(serializedObject.FindProperty("imagesAmount"));
		EditorGUILayout.Space();

		EditorGUILayout.PropertyField(serializedObject.FindProperty("baseHealth"));
		EditorGUILayout.PropertyField(serializedObject.FindProperty("currentHealth"));
		EditorGUILayout.Space();

		// 채우기 방향 설정
		SerializedProperty fillDirectionProp = serializedObject.FindProperty("fillDirection");
		EditorGUILayout.PropertyField(fillDirectionProp, new GUIContent("Fill Direction"));

		// 하트 간격 설정
		SerializedProperty heartSpacingProp = serializedObject.FindProperty("heartSpacing");
		EditorGUILayout.PropertyField(heartSpacingProp, new GUIContent("Heart Spacing"));

		if (GUI.changed)
		{
			EditorUtility.SetDirty(target);
			serializedObject.ApplyModifiedProperties();
		}
	}
}

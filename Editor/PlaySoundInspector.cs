using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PlaySound)), CanEditMultipleObjects]
public class PlaySoundInspector : Editor
{
    public override void OnInspectorGUI()
    {
        //source
        EditorGUILayout.PropertyField(serializedObject.FindProperty("source"));

        //type
        SerializedProperty typeProp = serializedObject.FindProperty("playMode");
        EditorGUILayout.PropertyField(typeProp);

        //clips
        EditorGUILayout.PropertyField(serializedObject.FindProperty("clips"), true);

        //bonus
        if(typeProp.enumValueIndex != (int)PlaySoundMode.SCATTER)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("cooldown"), new GUIContent("Play Cooldown Time"));
        }

        EditorGUILayout.PropertyField(serializedObject.FindProperty("autoPlay"));

        if (serializedObject.FindProperty("autoPlay").boolValue)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("startCooldownOffset"), new GUIContent("Start Delay"));
        }

        if (typeProp.enumValueIndex == (int)PlaySoundMode.LOOPING)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("randomiseLoopStart"));

            if(serializedObject.FindProperty("randomiseLoopStart").boolValue == false)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("clipStartOffset"), new GUIContent("Clip Start Offset"));
            }

            EditorGUILayout.PropertyField(serializedObject.FindProperty("maxVolume"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("attackTime"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("releaseTime"));
        }
        else if (typeProp.enumValueIndex == (int)PlaySoundMode.SCATTER)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("minCooldownTime"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("maxCooldownTime"));
        }
        else if(typeProp.enumValueIndex != (int)PlaySoundMode.SINGLE)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("randomStartIndex"));
            if (!serializedObject.FindProperty("randomStartIndex").boolValue)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("curIndex"), new GUIContent("Start Index"));
            }
        }

        EditorGUILayout.PropertyField(serializedObject.FindProperty("pitchRange"));

        serializedObject.ApplyModifiedProperties();
    }
}

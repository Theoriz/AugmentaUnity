﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Augmenta
{
    [CustomEditor(typeof(AugmentaVideoOutput))]
    public class AugmentaVideoOutputEditor : Editor
    {
        SerializedProperty augmentaManager;
        SerializedProperty augmentaVideoOutputCamera;

        SerializedProperty useExternalCamera;
        SerializedProperty paddingColor;

        SerializedProperty autoOutputSizeInPixels;
        SerializedProperty autoOutputSizeInMeters;
        SerializedProperty autoOutputOffset;

        SerializedProperty videoOutputSizeInPixels;
        SerializedProperty videoOutputSizeInMeters;
        SerializedProperty videoOutputOffset;

        void OnEnable() {

            augmentaManager = serializedObject.FindProperty("augmentaManager");
            augmentaVideoOutputCamera = serializedObject.FindProperty("camera");

            useExternalCamera = serializedObject.FindProperty("useExternalCamera");
            paddingColor = serializedObject.FindProperty("paddingColor");

            autoOutputSizeInPixels = serializedObject.FindProperty("autoOutputSizeInPixels");
            autoOutputSizeInMeters = serializedObject.FindProperty("autoOutputSizeInMeters");
            autoOutputOffset = serializedObject.FindProperty("autoOutputOffset");

            videoOutputSizeInPixels = serializedObject.FindProperty("_videoOutputSizeInPixels");
            videoOutputSizeInMeters = serializedObject.FindProperty("_videoOutputSizeInMeters");
            videoOutputOffset = serializedObject.FindProperty("_videoOutputOffset");
        }

        public override void OnInspectorGUI() {

            AugmentaVideoOutput augmentaVideoOutput = target as AugmentaVideoOutput;

            serializedObject.Update();

            EditorGUILayout.LabelField("AUGMENTA COMPONENTS", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(augmentaManager, new GUIContent("Augmenta Manager"));
            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(useExternalCamera, new GUIContent("Use External Output Camera", "If disabled, the script will look for an AugmentaVideoOutputCamera in its hierarchy.\nIf enabled, any camera from the scene can be used. Note that the specified camera will be assigned a render texture target."));
            EditorGUILayout.Space();

            if (useExternalCamera.boolValue) {
                EditorGUILayout.PropertyField(augmentaVideoOutputCamera, new GUIContent("Output Camera"));
                EditorGUILayout.PropertyField(paddingColor, new GUIContent("Texture Padding Color"));
                EditorGUILayout.Space();
            }

            EditorGUILayout.LabelField("VIDEO OUTPUT SETTINGS", EditorStyles.boldLabel);

            EditorGUILayout.PropertyField(autoOutputSizeInPixels, new GUIContent("Auto Size Output in Pixels", "Use data from Fusion to determine the output size in pixels."));

            if (!autoOutputSizeInPixels.boolValue) {
				EditorGUI.BeginChangeCheck();
				EditorGUILayout.PropertyField(videoOutputSizeInPixels, new GUIContent("Output Size in Pixels"));
				if (EditorGUI.EndChangeCheck() && Application.isPlaying) {
                    serializedObject.ApplyModifiedProperties();
                    augmentaVideoOutput.RefreshVideoTexture();
				}

				EditorGUILayout.Space();
            }

            EditorGUILayout.PropertyField(autoOutputSizeInMeters, new GUIContent("Auto Size Output in Meters", "Use data from Fusion to determine the output size in meters."));

            if (!autoOutputSizeInMeters.boolValue) {
                EditorGUILayout.PropertyField(videoOutputSizeInMeters, new GUIContent("Output Size in Meters"));

                EditorGUILayout.Space();
            }

            EditorGUILayout.PropertyField(autoOutputOffset, new GUIContent("Auto Output Offset", "Use data from Fusion to determine the output offset."));

            if (!autoOutputOffset.boolValue) {
                EditorGUILayout.PropertyField(videoOutputOffset, new GUIContent("Output Offset"));
            }

            EditorGUILayout.Space();

            serializedObject.ApplyModifiedProperties();

        }
    }
}

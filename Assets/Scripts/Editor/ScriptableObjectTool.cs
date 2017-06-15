using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Reflection;

public class ScriptableObjectTool : EditorWindow
{
    [MenuItem("Tools/DataObject")]
    public static void ShowWindow()
    {
        var window = GetWindow<ScriptableObjectTool>();
        window.Show();

    }

    private static string CreateDirectory = string.Empty;

    private UnityEngine.Object mTargetScript = null;


    private void OnGUI()
    {
      
        GUILayout.Label("Asset Create Directory");
        EditorGUILayout.BeginHorizontal();
        GUI.enabled = false;
        GUILayout.TextField(CreateDirectory,GUILayout.Width(350));
        GUI.enabled = true;
        if (GUILayout.Button("...", GUILayout.Width(30)))
        {
            string tempDirectory = EditorUtility.OpenFolderPanel("경로", Environment.CurrentDirectory + "/Assets", "");
            CreateDirectory = tempDirectory.Substring(tempDirectory.IndexOf("Assets"));
        }
        EditorGUILayout.EndHorizontal();

        mTargetScript = EditorGUILayout.ObjectField("TargetScript", mTargetScript, typeof(MonoScript), true);

        if(mTargetScript == null)
        {
            GUI.enabled = false;
        }
        if (GUILayout.Button("Create Object"))
        {
            string path = string.Format("{0}/{1}.asset", CreateDirectory, mTargetScript.name);

            Debug.Log(path);
            if (File.Exists(Environment.CurrentDirectory + "/" + path))
            {
                Debug.Log("Exist");
                return;
            }

            var instance = CreateInstance(mTargetScript.name);
            if(instance == null)
            {
                mTargetScript = null;
                return;
            }
            AssetDatabase.CreateAsset(instance, path);
            AssetDatabase.Refresh();
        }
        GUI.enabled = true;
    }
}

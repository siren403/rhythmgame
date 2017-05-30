using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

public class CThemeEditTool : EditorWindow
{
    [MenuItem("Tools/GameEditor/ThemeEditTool %5")]
    public static void OpenWindow()
    {
        var tWindow = GetWindow<CThemeEditTool>();
        tWindow.minSize = new Vector2(800, 550);
        tWindow.Show();
    }

    private string mCreateDataName = string.Empty;

    private CStageData mCurrentEditData = null;

    private SerializedObject mSequenceSO = null;
    private ReorderableList mReorderableList = null;
    private void OnEnable()
    {
        RefrashSequenceList();
    }

    private void RefrashSequenceList()
    {
        if (mCurrentEditData != null)
        {
            mSequenceSO = new SerializedObject(mCurrentEditData);
            mReorderableList = new ReorderableList(mSequenceSO, mSequenceSO.FindProperty("SequenceList"),
                true, true, false, false);

            mReorderableList.drawHeaderCallback = (rect) => EditorGUI.LabelField(rect, "Sequence Items");
            mReorderableList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                rect.y += 2;
                rect.height = EditorGUIUtility.singleLineHeight;
                var tElement = mReorderableList.serializedProperty.GetArrayElementAtIndex(index);
                EditorGUI.PropertyField(rect, tElement, tElement.isExpanded);

                //if (index == mReorderableList.index)
                //{
                //    mReorderableList.elementHeight = EditorGUI.GetPropertyHeight(tElement, GUIContent.none, tElement.isExpanded);
                //}
            };
            mReorderableList.drawElementBackgroundCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
            };
            mReorderableList.elementHeightCallback = (int index) =>
            {
                SerializedProperty arrayElement = mReorderableList.serializedProperty.GetArrayElementAtIndex(index);
                return EditorGUI.GetPropertyHeight(arrayElement, GUIContent.none, arrayElement.isExpanded) + 3;
            };
            Repaint();
        }
    }

    private void OnGUI()
    {
        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Name : ", GUILayout.Width(50));
        mCreateDataName = EditorGUILayout.TextField(mCreateDataName,GUILayout.Width(200));
        GUI.enabled = !string.IsNullOrEmpty(mCreateDataName);
        if (GUILayout.Button("Create",GUILayout.Width(100)))
        {
            mCurrentEditData = CreateInstance<CStageData>();
            AssetDatabase.CreateAsset(mCurrentEditData, string.Format("Assets/Resources/ThemeData/{0}.asset", mCreateDataName));
            AssetDatabase.Refresh();
        }
        GUI.enabled = true;
        EditorGUILayout.LabelField("DataObject : ", GUILayout.Width(80));
        mCurrentEditData = EditorGUILayout.ObjectField(mCurrentEditData, typeof(CStageData),true) as CStageData;
        if (GUILayout.Button("Refrash", GUILayout.Width(80)))
        {
            RefrashSequenceList();
        }
        GUILayout.EndHorizontal();

        GUILayout.Space(10);

        if (mCurrentEditData == null)
            return;

        GUILayout.BeginHorizontal();
        mCurrentEditData.BPM = CCustomField.IntField("BPM : ", ref mCurrentEditData.BPM);
        EditorGUILayout.LabelField(string.Format("BPS : {0}", mCurrentEditData.BPS),GUILayout.Width(120));
        mCurrentEditData.StartBeatOffset = CCustomField.FloatField("StartBeatOffset : ", ref mCurrentEditData.StartBeatOffset, 110);
        if(GUILayout.Button("ToJson",GUILayout.Width(50)))
        {
            Debug.Log(JsonUtility.ToJson(mCurrentEditData));
        }
        GUILayout.EndHorizontal();

        if(mSequenceSO != null)
        {

            GUILayout.Space(9);
            GUILayout.BeginHorizontal();

            if(GUILayout.Button("Add",GUILayout.Width(60)))
            {
                mCurrentEditData.SequenceList.Add(new CSequenceData(0, 0));
            }
            if (GUILayout.Button("Remove", GUILayout.Width(60)))
            {
                mCurrentEditData.SequenceList.RemoveAt(mCurrentEditData.SequenceList.Count - 1);
            }
            GUILayout.EndHorizontal();
            mSequenceSO.Update();
            mReorderableList.DoList(new Rect(0,EditorGUIUtility.singleLineHeight * 5,this.minSize.x,this.minSize.y));
            mSequenceSO.ApplyModifiedProperties();
        }
        else
        {
            Debug.Log("SeqSo is NUll");
        }
    }


}

public class CCustomField
{
    public static int IntField(string tTitle,ref int tValue,float tLabelWidth = 50,float tFieldWidth = 50)
    {
        EditorGUILayout.LabelField(tTitle, GUILayout.Width(tLabelWidth));
        return EditorGUILayout.IntField(tValue, GUILayout.Width(tFieldWidth));
    }
    public static float FloatField(string tTitle, ref float tValue, float tLabelWidth = 50, float tFieldWidth = 50)
    {
        EditorGUILayout.LabelField(tTitle, GUILayout.Width(tLabelWidth));
        return EditorGUILayout.FloatField(tValue, GUILayout.Width(tFieldWidth));
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Linq;

public class CStageEditTool : EditorWindow
{
    private const string PATH_EDIT_DATA = "Assets/Resources/StageData";

    [MenuItem("Tools/GameEditor/ThemeEditTool %5")]
    public static void OpenWindow()
    {
        var tWindow = GetWindow<CStageEditTool>();
        tWindow.minSize = new Vector2(800, 550);
        tWindow.Show();
    }

    private string mCreateDataName = string.Empty;
    private float mAddSeqBeat = 0.0f;

    private CStageData mEditData = null;

    private SerializedObject mStageDataSO = null;
    private SerializedObject mToolDataSO = null;
    //SequenceList 
    private ReorderableList mSeqReorderableList = null;
    private Vector2 mReorderScrollViewPos = Vector2.zero;
    //SoundEffects
    private ReorderableList mSEReorderableList = null;


    public enum SequenceEditTap
    {
        Edit, Template,
    }
    private SequenceEditTap mCurrentTap = SequenceEditTap.Edit;

    private void OnEnable()
    {
        RefrashSequenceList();
    }

    private void RefrashSequenceList()
    {
        if (mEditData != null)
        {
            mStageDataSO = new SerializedObject(mEditData);
            mSeqReorderableList = new ReorderableList(mStageDataSO, mStageDataSO.FindProperty("SequenceList"),
                false, true, false, false);

            mSeqReorderableList.drawHeaderCallback = (rect) => EditorGUI.LabelField(rect, "Sequence Items");
            mSeqReorderableList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                rect.y += 2;
                rect.height = EditorGUIUtility.singleLineHeight;
                var tElement = mSeqReorderableList.serializedProperty.GetArrayElementAtIndex(index);
                EditorGUI.PropertyField(rect, tElement, tElement.isExpanded);
                //if (index == mReorderableList.index)
                //{
                //    mReorderableList.elementHeight = EditorGUI.GetPropertyHeight(tElement, GUIContent.none, tElement.isExpanded);
                //}
            };
            mSeqReorderableList.elementHeightCallback = (int index) =>
            {
                SerializedProperty arrayElement = mSeqReorderableList.serializedProperty.GetArrayElementAtIndex(index);
                return EditorGUI.GetPropertyHeight(arrayElement, GUIContent.none, arrayElement.isExpanded) + 3;
            };

            mSEReorderableList = new ReorderableList(mStageDataSO, mStageDataSO.FindProperty("SoundEffects"),
                true, true, true, true);
            mSEReorderableList.drawHeaderCallback = (rect) => EditorGUI.LabelField(rect, "SoundEffects");
            mSEReorderableList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                rect.y += 2;
                rect.height = EditorGUIUtility.singleLineHeight;
                var tElement = mSEReorderableList.serializedProperty.GetArrayElementAtIndex(index);
                EditorGUI.PropertyField(rect, tElement, tElement.isExpanded);
            };
            mSEReorderableList.elementHeightCallback = (int index) =>
            {
                SerializedProperty arrayElement = mSEReorderableList.serializedProperty.GetArrayElementAtIndex(index);
                return EditorGUI.GetPropertyHeight(arrayElement, GUIContent.none, arrayElement.isExpanded) + 3;
            };


            Repaint();
        }
    }

    private void OnGUI()
    {
        
        DrawCreateEditData();

        GUILayout.Space(10);

        if (mEditData == null || mStageDataSO == null)
            return;

        mStageDataSO.Update();

        GUILayout.BeginHorizontal();
        mEditData.BPM = CCustomField.IntField("BPM : ", ref mEditData.BPM);
        EditorGUILayout.LabelField(string.Format("BPS : {0}", mEditData.BPS), GUILayout.Width(120));
        mEditData.StartBeatOffset = CCustomField.FloatField("StartBeatOffset : ", mEditData.StartBeatOffset, 110);
        if (GUILayout.Button("ToJson", GUILayout.Width(50)))
        {
            Debug.Log(JsonUtility.ToJson(mEditData));
        }
        GUILayout.EndHorizontal();


        //Audio Resources
        GUILayout.BeginHorizontal();
        mEditData.Music = CCustomField.ObjectField<AudioClip>("Music : ", mEditData.Music, tFieldWidth: 200);
        GUILayout.EndHorizontal();

        mSEReorderableList.DoLayoutList();

        GUILayout.BeginHorizontal();
        GUI.enabled = !(mCurrentTap == SequenceEditTap.Edit);
        if (GUILayout.Button("Edit"))
            mCurrentTap = SequenceEditTap.Edit;

        GUI.enabled = !(mCurrentTap == SequenceEditTap.Template);
        if (GUILayout.Button("Template"))
            mCurrentTap = SequenceEditTap.Template;

        GUI.enabled = true;
        GUILayout.EndHorizontal();

        //Sequence Data
        switch(mCurrentTap)
        {
            case SequenceEditTap.Edit:
                DrawSequenceEdit();
                break;
            case SequenceEditTap.Template:
                break;
        }
    }

    private void DrawCreateEditData()
    {
        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Name : ", GUILayout.Width(50));
        mCreateDataName = EditorGUILayout.TextField(mCreateDataName, GUILayout.Width(200));
        GUI.enabled = !string.IsNullOrEmpty(mCreateDataName);
        if (GUILayout.Button("Create", GUILayout.Width(100)))
        {
            mEditData = CreateInstance<CStageData>();
            mEditData.StageName = mCreateDataName;
            AssetDatabase.CreateAsset(mEditData, string.Format(PATH_EDIT_DATA + "/{0}Data.asset", mCreateDataName));

            AssetDatabase.Refresh();
        }
        GUI.enabled = true;
        EditorGUILayout.LabelField("DataObject : ", GUILayout.Width(80));
        mEditData = EditorGUILayout.ObjectField(mEditData, typeof(CStageData), false) as CStageData;
        if (GUILayout.Button("Refrash", GUILayout.Width(80)))
        {
            RefrashSequenceList();
        }
        GUILayout.EndHorizontal();
    }
    private void DrawSequenceEdit()
    {
        EditorGUILayout.BeginHorizontal();
        mAddSeqBeat = CCustomField.FloatField("Beat : ", mAddSeqBeat, 40);
        if (GUILayout.Button("Add", GUILayout.Width(60)))
        {
            mEditData.SequenceList.Add(new CSequenceData(mAddSeqBeat));
        }
        if (GUILayout.Button("Append", GUILayout.Width(60)))
        {
            mAddSeqBeat = mEditData.SequenceList[mEditData.SequenceList.Count - 1].Beat + 1;
            mEditData.SequenceList.Add(new CSequenceData(mAddSeqBeat));
        }
        if (GUILayout.Button("Remove", GUILayout.Width(60)))
        {
            mEditData.SequenceList.RemoveAt(mEditData.SequenceList.Count - 1);
        }
        EditorGUILayout.EndHorizontal();

        Rect tReorderRect = new Rect(0, EditorGUIUtility.singleLineHeight * 5, this.minSize.x - 30, this.minSize.y);
        mReorderScrollViewPos = GUILayout.BeginScrollView(mReorderScrollViewPos);
        mSeqReorderableList.DoLayoutList();
        GUILayout.EndScrollView();

        mStageDataSO.ApplyModifiedProperties();
    }
}

public class CCustomField
{
    public static int IntField(string tTitle,ref int tValue,float tLabelWidth = 50,float tFieldWidth = 50)
    {
        EditorGUILayout.LabelField(tTitle, GUILayout.Width(tLabelWidth));
        return EditorGUILayout.IntField(tValue, GUILayout.Width(tFieldWidth));
    }
    public static float FloatField(string tTitle,float tValue, float tLabelWidth = 50, float tFieldWidth = 50)
    {
        EditorGUILayout.LabelField(tTitle, GUILayout.Width(tLabelWidth));
        return EditorGUILayout.FloatField(tValue, GUILayout.Width(tFieldWidth));
    }
    public static T ObjectField<T>(string tTitle, T tValue, float tLabelWidth = 50, float tFieldWidth = 50) where T: Object
    {
        EditorGUILayout.LabelField(tTitle, GUILayout.Width(tLabelWidth));
        return EditorGUILayout.ObjectField(tValue, typeof(T),false,GUILayout.Width(tFieldWidth)) as T;
    }
}

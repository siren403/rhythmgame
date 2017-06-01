using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Linq;
using System.Text;
using System.IO;

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
    private string mAddActionCode = string.Empty;
    private int mSelectedActionCodeIndex = 0;

    private CStageData mEditData = null;

    private SerializedObject mStageDataSO = null;
    private SerializedObject mToolDataSO = null;
    //SequenceList 
    private ReorderableList mSeqReorderableList = null;
    private Vector2 mReorderScrollViewPos = Vector2.zero;


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
                true, true, false, false);

            mSeqReorderableList.drawHeaderCallback = (rect) => EditorGUI.LabelField(rect, "Sequence Items");
            mSeqReorderableList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                rect.y += 2;
                rect.height = EditorGUIUtility.singleLineHeight;
                var tElement = mSeqReorderableList.serializedProperty.GetArrayElementAtIndex(index);

                //EditorGUI.PropertyField(rect, tElement, tElement.isExpanded);
                EditorGUI.PropertyField(rect, tElement, false);

                if (tElement.isExpanded)
                {
                    float tSingle = EditorGUIUtility.singleLineHeight;
                    rect.x += 20;
                    rect.y += tSingle * 1.3f;
                    rect.width -= 25;
                    EditorGUI.PropertyField(rect, tElement.FindPropertyRelative("_Beat"));
                    rect.y += tSingle;
                    EditorGUI.PropertyField(rect, tElement.FindPropertyRelative("_Input"));
                    rect.y += tSingle;

                    string tActionCode = tElement.FindPropertyRelative("_ActionCode").stringValue;
                    int tActionCodeIndex = mEditData.ActionCodeList.IndexOf(tActionCode);
                    if (tActionCodeIndex == -1)
                        tActionCodeIndex = 0;

                    tElement.FindPropertyRelative("_ActionCode").stringValue
                        = mEditData.ActionCodeList[EditorGUI.Popup(rect, "ActionCode",
                        tActionCodeIndex,
                        mEditData.ActionCodeList.ToArray())];
                }

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
        mEditData.StartBeatOffset = CCustomField.FloatField("StartBeatOffset : ", mEditData.StartBeatOffset, 100);
        mEditData.PerfectRange = CCustomField.FloatField("Perfect Range : ", mEditData.PerfectRange, 100);

        if (GUILayout.Button("ToJson", GUILayout.Width(50)))
        {
            Debug.Log(JsonUtility.ToJson(mEditData));
        }
        GUILayout.EndHorizontal();


        //Audio Resources
        GUILayout.BeginHorizontal();
        mEditData.Music = CCustomField.ObjectField<AudioClip>("Music : ", mEditData.Music, tFieldWidth: 200);
        GUILayout.EndHorizontal();


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

        GUILayout.Space(8);
        GUILayout.Label("ActionCode : ", GUILayout.Width(80));
        mSelectedActionCodeIndex = EditorGUILayout.Popup(mSelectedActionCodeIndex, mEditData.ActionCodeList.ToArray(), GUILayout.Width(100));
        mAddActionCode = GUILayout.TextField(mAddActionCode, GUILayout.Width(100));
        if(GUILayout.Button("Add", GUILayout.Width(60)))
        {
            if(mEditData.ActionCodeList.Contains(mAddActionCode) == false)
            {
                mEditData.ActionCodeList.Add(mAddActionCode);
                mAddActionCode = string.Empty;
            }
        }
        if (GUILayout.Button("Remove", GUILayout.Width(60)))
        {
            mEditData.ActionCodeList.RemoveAt(mSelectedActionCodeIndex);
        }
        if (GUILayout.Button("Generate", GUILayout.Width(75)))
        {
            StringArrayToClass.Generate(mEditData.ActionCodeList.ToArray(), "C"+mEditData.StageName + "ActionCode", "Assets/Scripts/Stage/ActionCode");
        }
        EditorGUILayout.EndHorizontal();

        Rect tReorderRect = new Rect(0, EditorGUIUtility.singleLineHeight * 5, this.minSize.x - 30, this.minSize.y);
        mReorderScrollViewPos = GUILayout.BeginScrollView(mReorderScrollViewPos);
        mSeqReorderableList.DoLayoutList();
        GUILayout.EndScrollView();

        mStageDataSO.ApplyModifiedProperties();
    }
}

public static class CCustomField
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

public static class StringArrayToClass
{
    public static void Generate(string[] array, string name, string path, string nameSpace = null, string tag = null)
    {
        if (array == null || string.IsNullOrEmpty(name) || string.IsNullOrEmpty(path))
            return;

        bool isNameSpace = !string.IsNullOrEmpty(nameSpace);

        StringBuilder enumString = new StringBuilder();

        if (isNameSpace)
        {
            enumString.Append("\t");
        }

        enumString.AppendFormat("public static class {0}\n", name);

        if (isNameSpace)
        {
            enumString.Append("\t");
        }

        enumString.Append("{");

        foreach (var str in array)
        {
            enumString.Append("\n\t");

            if (isNameSpace)
            {
                enumString.Append("\t");
            }

            if (string.IsNullOrEmpty(tag) == false)
                enumString.AppendFormat("{0}_", tag);

            //enumString.AppendFormat("{0},", str);
            enumString.AppendFormat("public const string {0} = \"{1}\";", str.ToUpper(), str);

        }

        enumString.AppendLine();

        if (isNameSpace)
        {
            enumString.Append("\t");
        }

        enumString.Append("}");

        string result = enumString.ToString();
        enumString.Remove(0, enumString.Length);
        //Debug.Log(result);

        if (isNameSpace)
        {
            result = enumString
                .AppendFormat("namespace {0}\n", nameSpace)
                .Append("{")
                .AppendLine()
                .AppendFormat("{0}", result)
                .AppendLine()
                .Append("}")
                .ToString();
        }


        path = string.Format("{0}/{1}/{2}.cs",
            System.Environment.CurrentDirectory.Replace('\\', '/'),
            path,
            name);

        //Debug.Log(result);
        //Debug.Log(path);
        using (StreamWriter sw = new StreamWriter(path))
        {
            sw.Write(result);
        }
        AssetDatabase.Refresh();
    }
}
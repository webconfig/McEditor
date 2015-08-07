using UnityEditor;
using UnityEngine;

/// <summary>
/// 编辑器属性窗口扩展
/// </summary>
[CustomEditor(typeof(EditorBase))]
public class DesignerInspector : UnityEditor.Editor
{
    /// <summary>
    /// 绘制自定义Inspector视图
    /// </summary>
    public override void OnInspectorGUI()
    {
        EditorBase _editor = base.target as EditorBase;
        if (_editor == null)
        {
            return;
        }
        
        if(DesignerInspector.DrawInspectorGUI(_editor,base.serializedObject))
        {//显示数据里面的变量
            EditorUtility.SetDirty(_editor);//Unity将会知道对象的内容被修改了，需要重新序列化并保存到磁盘上
        }

        if (GUILayout.Button("打开设计器", new GUILayoutOption[0]))
        {//弹出行为树设计窗口
            DesignerWindow.Open();
            DesignerWindow.Instance.LoadData(_editor.Data);
        }
    }
    /// <summary>
    /// 显示数据里面的变量
    /// </summary>
    /// <param name="_editor"></param>
    /// <param name="serializedObject"></param>
    /// <returns></returns>
    public static bool DrawInspectorGUI(EditorBase _editor, SerializedObject serializedObject)
    {
        serializedObject.Update();
        EditorGUI.BeginChangeCheck();
        GUILayout.BeginHorizontal(new GUILayoutOption[0]);
        EditorGUILayout.LabelField("名称", new GUILayoutOption[]
			{
				GUILayout.Width(120f)
			});
        _editor.Data.Name = EditorGUILayout.TextField(_editor.Data.Name, new GUILayoutOption[0]);
        GUILayout.EndHorizontal();
        if (EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties();
            return true;
        }
        return false;
    }
}

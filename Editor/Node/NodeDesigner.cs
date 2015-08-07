using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[Serializable]
public class NodeDesigner : ScriptableObject
{
    Texture2D nodeTexture;
    // Selection
    Texture2D selectionTexture;
    Color selColor = new Color(1f, .78f, .353f);
    float selMargin = 2f;
    float selWidth = 2f;

    private Dictionary<string, Texture2D> textures = new Dictionary<string, Texture2D>();

    public static float Width { get { return GridRenderer.step.x * 6; } }
    public static float Height { get { return GridRenderer.step.y * 6; } }

    #region GUI
    public void Draw(bool selected)
    {

        Vector2 editorPosition = action.NodeData.Position;// new Vector2(50, 50);
        Rect nodeRect = new Rect(editorPosition.x, editorPosition.y, Width, Height);

        // Node
        if (nodeTexture == null)
        {

            Color colA = new Color(0.765f, 0.765f, 0.765f);
            Color colB = new Color(0.886f, 0.886f, 0.886f);

            nodeTexture = new Texture2D(1, (int)Height);
            nodeTexture.hideFlags = HideFlags.DontSave;
            for (int y = 0; y < Height; y++)
            {
                nodeTexture.SetPixel(0, y, Color.Lerp(colA, colB, (float)y / 75));
            }
            nodeTexture.Apply();
        }
        GUI.DrawTexture(nodeRect, nodeTexture);

        // Node title
        string title = "阿斯顿发";
        title = title.Replace(".", ".\n");
        Vector2 textSize = GUI.skin.label.CalcSize(new GUIContent(title));
        float x = editorPosition.x + (Width / 2) - (textSize.x / 2) - 6;
        Rect titleRect = new Rect(x, (editorPosition.y - textSize.y) / 2 + Height, textSize.x + 10, textSize.y);
        EditorGUI.LabelField(titleRect, new GUIContent(title));

        // 高亮
        if (selected)
        {
            if (selectionTexture == null)
            {
                selectionTexture = new Texture2D(1, 1);
                selectionTexture.hideFlags = HideFlags.DontSave;
                selectionTexture.SetPixel(0, 0, selColor);
                selectionTexture.Apply();
            }

            float mbOffset = selMargin + selWidth; // Margin + Border offset
            GUI.DrawTexture(new Rect(nodeRect.x - mbOffset, nodeRect.y - mbOffset, nodeRect.width + mbOffset * 2, selWidth), selectionTexture); // Top
            GUI.DrawTexture(new Rect(nodeRect.x - mbOffset, nodeRect.y - selMargin, selWidth, nodeRect.height + selMargin * 2), selectionTexture); // Left
            GUI.DrawTexture(new Rect(nodeRect.x + nodeRect.width + selMargin, nodeRect.y - selMargin, selWidth, nodeRect.height + selMargin * 2), selectionTexture); // Right
            GUI.DrawTexture(new Rect(nodeRect.x - mbOffset, nodeRect.y + nodeRect.height + selMargin, nodeRect.width + mbOffset * 2, selWidth), selectionTexture); // Top
        }
    }
    #endregion



    #region 数据
    /// <summary>
    /// 节点的任务名称
    /// </summary>
    [SerializeField]
    public string taskName = "";
    /// <summary>
    /// 行为
    /// </summary>
    private ActionBase action;
    /// <summary>
    /// 数据源
    /// </summary>
    public EditorBase Owner;
    /// <summary>
    /// 初始化该节点
    /// </summary>
    /// <param name="task"></param>
    /// <param name="behaviorSource"></param>
    /// <param name="position"></param>
    /// <param name="id"></param>
    public void loadNode(ActionBase _action, DataSource behaviorSource, Vector2 position, ref int id)
    {
        action = _action;
        Owner = behaviorSource.Owner;
        FieldInfo[] fields = action.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);//只返回公开的字段
        for (int i = 0; i < fields.Length; i++)
        {
            if (fields[i].FieldType.IsSubclassOf(typeof(SharedVariable)))
            {//该类型是 共享数据
                SharedVariable sharedVariable = fields[i].GetValue(action) as SharedVariable;
                if (sharedVariable == null)
                {
                    sharedVariable = (ScriptableObject.CreateInstance(fields[i].FieldType) as SharedVariable);
                    fields[i].SetValue(action, sharedVariable);
                }
            }
        }
        action.ID = id++;
        action.NodeData = new NodeData();
        action.NodeData.Position = position;
        action.NodeData.NodeDesigner = this;
        //this.loadTaskIcon();
        this.init();
        //this.mTask.NodeData.FriendlyName = this.taskName;
    }
    private void init()
    {//获取名称
        this.taskName = action.GetType().Name.ToString();

        ////Debug.Log("taskName:" + taskName);
        //this.isParent = this.mTask.GetType().IsSubclassOf(typeof(ParentTask));
        //if (this.isParent)
        //{
        //    this.outgoingNodeConnections = new List<NodeConnection>();
        //}
    }

    #endregion
}
  
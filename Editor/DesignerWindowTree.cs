using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 树形控件
/// </summary>
public class DesignerTree : ScriptableObject
{
    /// <summary>
    /// 树的节点
    /// </summary>
    private List<DesignerTreeNode> TreeNodes;
    /// <summary>
    /// 搜索的字符串
    /// </summary>
    private string SearchStr="";
    /// <summary>
    /// 滚动条位置
    /// </summary>
    private Vector2 ScrollPosition = Vector2.zero;
    public void Init()
    {
        TreeNodes = new List<DesignerTreeNode>();
        //获取程序集里面所有的任务
        Assembly assembly = null;
        List<Type> list = new List<Type>();
        string[] strs = new string[] { "Assembly-CSharp" }; //new string[] { "Assembly-CSharp", "Assembly-UnityScript", "Assembly-UnityScript-firstpass", "Assembly-CSharp-firstpass" };
        for (int i = 0; i < strs.Length; i++)
        {
            try
            {
                assembly = Assembly.Load(strs[i]);
                if (assembly != null)
                {
                    Type[] types = assembly.GetTypes();
                    for (int j = 0; j < types.Length; j++)
                    {
                        if (types[j].IsSubclassOf(typeof(ActionBase)))
                        {//是ActionBase的子类
                            list.Add(types[j]);
                        }
                    }
                }
            }
            catch { }
        }
        list.Sort(new AlphanumComparator<Type>());

        //形成树状结构
        Dictionary<string, DesignerTreeNode> dictionary = new Dictionary<string, DesignerTreeNode>();
        int id = 0;
        for (int k = 0; k < list.Count; k++)
        {
            ActionPathAttribute[] array;
            string pp = "";
            DesignerTreeNode node = null, node_parent = null;
            if ((array = (list[k].GetCustomAttributes(typeof(ActionPathAttribute), false) as ActionPathAttribute[])).Length > 0)
            {
                string[] datas = array[0].path.Split('/');
                for (int l = 0; l < datas.Length; l++)
                {
                    if (l > 0) { pp += "/"; }
                    pp += datas[l];
                    if (!dictionary.ContainsKey(pp))
                    {
                        node = new DesignerTreeNode(datas[l], pp, false, id++);
                        if (node_parent == null)
                        {
                            TreeNodes.Add(node);
                        }
                        else
                        {
                            node_parent.AddNode(node);
                        }
                        dictionary.Add(pp, node);
                    }
                    else
                    {
                        node = dictionary[pp];
                    }
                    node_parent = node;
                }
                dictionary[pp].AddTask(list[k]);//末尾节点赋予任务
            }
        }

        if (!string.IsNullOrEmpty(SearchStr))
        {
            //搜索
            Search(SearchStr.ToLower().Replace(" ", ""), TreeNodes);
        }
    }
    /// <summary>
    /// 搜索
    /// </summary>
    /// <param name="searchString"></param>
    /// <param name="categoryList"></param>
    /// <returns></returns>
    private bool Search(string searchString, List<DesignerTreeNode> nodes)
    {
        bool result = searchString.Equals("");
        for (int i = 0; i < nodes.Count; i++)
        {//便利节点
            bool flag = false;
            nodes[i].Visible = false;//节点变为不可见
            if (nodes[i].Childs != null && this.Search(searchString, nodes[i].Childs))
            {//搜索节点的子节点
                nodes[i].Visible = true;//子节点搜索到，自己也显示
                result = true;
            }
            if (nodes[i].Name.ToLower().Replace(" ", "").Contains(searchString))
            {//包含搜索的字符串
                result = true;
                flag = true;
                nodes[i].Visible = true;//节点显示
                if (nodes[i].Childs != null)
                {
                    this.markVisible(nodes[i].Childs);//让子节点显示
                }
            }
            if (nodes[i].Tasks != null)
            {//便利节点的任务
                for (int j = 0; j < nodes[i].Tasks.Count; j++)
                {
                    nodes[i].Tasks[j].Visible = searchString.Equals("");
                    if (flag || nodes[i].Tasks[j].Type.Name.ToLower().Replace(" ", "").Contains(searchString))
                    {
                        nodes[i].Tasks[j].Visible = true;
                        result = true;
                        nodes[i].Visible = true;
                    }
                }
            }
        }
        return result;
    }
    /// <summary>
    /// 让子节点显示
    /// </summary>
    /// <param name="categoryList">父节点</param>
    private void markVisible(List<DesignerTreeNode> categoryList)
    {
        for (int i = 0; i < categoryList.Count; i++)
        {
            categoryList[i].Visible = true;
            if (categoryList[i].Childs != null)
            {
                this.markVisible(categoryList[i].Childs);
            }
            if (categoryList[i].Tasks != null)
            {
                for (int j = 0; j < categoryList[i].Tasks.Count; j++)
                {
                    categoryList[i].Tasks[j].Visible = true;
                }
            }
        }
    }

    /// <summary>
    /// 绘制行为树所有的行为节点
    /// </summary>
    /// <param name="window"></param>
    public void drawTaskList(DesignerWindow window)
    {
        //搜索
        GUILayout.BeginHorizontal(new GUILayoutOption[0]);
        GUI.SetNextControlName("Search");
        string value = GUILayout.TextField(SearchStr, GUI.skin.FindStyle("ToolbarSeachTextField"), new GUILayoutOption[0]);
        //if (this.mFocusSearch)
        //{
        //    GUI.FocusControl("Search");
        //    this.mFocusSearch = false;
        //}
        if (!this.SearchStr.Equals(value))
        {
            this.SearchStr = value;
            this.Search(SearchStr.ToLower().Replace(" ", ""), this.TreeNodes);
        }
        if (GUILayout.Button("", this.SearchStr.Equals("") ? GUI.skin.FindStyle("ToolbarSeachCancelButtonEmpty") : GUI.skin.FindStyle("ToolbarSeachCancelButton"), new GUILayoutOption[0]))
        {
            this.SearchStr = "";
            this.Search("", this.TreeNodes);
            GUI.FocusControl(null);
        }
        GUILayout.EndHorizontal();

        DesignerHelp.DrawContentSeperator(2);
        GUILayout.Space(4f);
        ScrollPosition = GUILayout.BeginScrollView(ScrollPosition, new GUILayoutOption[0]);

        for (int i = 0; i < TreeNodes.Count; i++)
        {
            this.DrawCategory(window, this.TreeNodes[i]);
        }
        GUILayout.EndScrollView();
    }

    /// <summary>
    /// 绘制树的叶子结点
    /// </summary>
    /// <param name="window"></param>
    /// <param name="node"></param>
    private void DrawCategory(DesignerWindow window, DesignerTreeNode node)
    {
        if (node.Visible)
        {
            node.Expanded = EditorGUILayout.Foldout(node.Expanded, node.Name, DesignerHelp.TaskFoldoutGUIStyle);
           // this.SetExpanded(node.ID, node.Expanded);
            if (node.Expanded)
            {
                EditorGUI.indentLevel = EditorGUI.indentLevel + 1;
                if (node.Tasks != null)
                {
                    for (int i = 0; i < node.Tasks.Count; i++)
                    {
                        if (node.Tasks[i].Visible)
                        {
                            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
                            GUILayout.Space((float)(EditorGUI.indentLevel * 10));
                            if (GUILayout.Button(node.Tasks[i].Type.Name.ToString(), EditorStyles.toolbarButton, new GUILayoutOption[0]))
                            {//点击按钮，添加一个任务
                                window.addTask(node.Tasks[i].Type, false);
                            }
                            GUILayout.Space(3f);
                            GUILayout.EndHorizontal();
                        }
                    }
                }
                if (node.Childs != null)
                {
                    this.DrawCategoryTaskList(window, node.Childs);
                }
                EditorGUI.indentLevel = EditorGUI.indentLevel - 1;
            }
        }
    }

    private void DrawCategoryTaskList(DesignerWindow window, List<DesignerTreeNode> categoryList)
    {
        for (int i = 0; i < categoryList.Count; i++)
        {
            this.DrawCategory(window, categoryList[i]);
        }
    }

}
/// <summary>
/// 树节点
/// </summary>
public class DesignerTreeNode
{
    public string Name;
    public string Fullpath;
    public bool Expanded;
    public int ID;
    public bool Visible = true;

    public List<SearchableType> Tasks = new List<SearchableType>();

    public List<DesignerTreeNode> Childs = new List<DesignerTreeNode>();

    public DesignerTreeNode(string name, string fullpath, bool expanded, int id)
    {
        Name = name;
        Fullpath = fullpath;
        Expanded = expanded;
        ID = id;
    }

    public void AddNode(DesignerTreeNode _node)
    {
        Childs.Add(_node);
    }
    public void AddTask(Type taskType)
    {
        this.Tasks.Add(new SearchableType(taskType));
    }
}
public class SearchableType
{
    private Type mType;

    private bool mVisible = true;

    public Type Type
    {
        get
        {
            return this.mType;
        }
    }

    public bool Visible
    {
        get
        {
            return this.mVisible;
        }
        set
        {
            this.mVisible = value;
        }
    }

    public SearchableType(Type type)
    {
        this.mType = type;
    }
}


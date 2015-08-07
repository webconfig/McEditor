using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
/// <summary>
/// 设计器窗口
/// </summary>
public class DesignerWindow : EditorWindow
{
    [SerializeField]
    public static DesignerWindow Instance;
    private Rect mGraphRect;

    private Rect mFileToolBarRect;

    private Rect mPropertyToolbarRect;
    /// <summary>
    /// 属性窗口
    /// </summary>
    private Rect mPropertyBoxRect;

    private Rect mPreferencesPaneRect;

    private Vector2 mGraphScrollSize = new Vector2(20000f, 20000f);

    private bool mSizesInitialized;

    private Vector2 mCurrentMousePosition = Vector2.zero;

    private Vector2 mGraphScrollPosition = new Vector2(-1f, -1f);

    private Vector2 mGraphOffset = Vector2.zero;

    private float mGraphZoom = 1f;

    private DesignerTree tree;

    /// <summary>
    /// 显示设计窗口
    /// </summary>
    [MenuItem("Window/open")]
    public static void Open()
    {
        DesignerWindow behaviorDesignerWindow = EditorWindow.GetWindow(typeof(DesignerWindow)) as DesignerWindow;//显示窗体
        behaviorDesignerWindow.wantsMouseMove = true;
        behaviorDesignerWindow.minSize = new Vector2(600f, 500f);
        UnityEngine.Object.DontDestroyOnLoad(behaviorDesignerWindow);
        //BehaviorDesignerPreferences.InitPrefernces();
    }

    /// <summary>
    /// 当窗口获得焦点时调用一次
    /// </summary>
    public void OnFocus()
    {
        DesignerWindow.Instance = this;
        base.wantsMouseMove = true;
        init();
        //if (!this.mLockActiveGameObject)
        //{
        //    this.mActiveObject = Selection.activeObject;
        //}
        //this.ReloadPreviousBehavior();
        //this.UpdateGraphStatus();
    }

    private void init()
    {
        if (tree == null)
        {
            tree = ScriptableObject.CreateInstance<DesignerTree>();
        }
        tree.Init();
        //this.UpdateBreadcrumbMenus();
    }

    #region 数据
    /// <summary>
    /// 当前数据
    /// </summary>
    public DataSource DataNow;
    /// <summary>
    /// 加载数据
    /// </summary>
    public void LoadData(DataSource _data)
    {
        DataNow = _data;
    }
    #endregion

    #region  GUI
    [SerializeField]
    private GraphDesigner mGraphDesigner = ScriptableObject.CreateInstance<GraphDesigner>();

    /// <summary>
    /// 窗口绘制函数
    /// </summary>
    public void OnGUI()
    {
        this.setupSizes();
        if(Draw())
        {
            base.Repaint();
        }
    }
    /// <summary>
    /// 设置窗体内部布局
    /// </summary>
    private void setupSizes()
    {

        this.mFileToolBarRect = new Rect((float)DesignerHelp.PropertyBoxWidth, 0f, (float)(Screen.width - DesignerHelp.PropertyBoxWidth), (float)DesignerHelp.ToolBarHeight);
        this.mPropertyToolbarRect = new Rect(0f, 0f, (float)DesignerHelp.PropertyBoxWidth, (float)DesignerHelp.ToolBarHeight);
        this.mPropertyBoxRect = new Rect(0f, this.mPropertyToolbarRect.height, (float)DesignerHelp.PropertyBoxWidth, (float)Screen.height - this.mPropertyToolbarRect.height - (float)DesignerHelp.EditorWindowTabHeight);
        this.mGraphRect = new Rect((float)DesignerHelp.PropertyBoxWidth, (float)DesignerHelp.ToolBarHeight, (float)(Screen.width - DesignerHelp.PropertyBoxWidth - DesignerHelp.ScrollBarSize), (float)(Screen.height - DesignerHelp.ToolBarHeight - DesignerHelp.EditorWindowTabHeight - DesignerHelp.ScrollBarSize));
        this.mPreferencesPaneRect = new Rect((float)DesignerHelp.PropertyBoxWidth + this.mGraphRect.width - (float)DesignerHelp.PreferencesPaneWidth, (float)(DesignerHelp.ToolBarHeight + (EditorGUIUtility.isProSkin ? 1 : 2)), (float)DesignerHelp.PreferencesPaneWidth, (float)DesignerHelp.PreferencesPaneHeight);
        if (this.mGraphScrollPosition == new Vector2(-1f, -1f))
        {
            this.mGraphScrollPosition = (this.mGraphScrollSize - new Vector2(this.mGraphRect.width, this.mGraphRect.height)) / 2f - 2f * new Vector2((float)DesignerHelp.ScrollBarSize, (float)DesignerHelp.ScrollBarSize);
        }
    }


    private int ToolbarSelection = 1;
    private string[] ToolbarStrings = new string[]
		{
			"Behavior",
			"Tasks",
			"Variables",
			"Inspector"
		};
    /// <summary>
    /// 绘制顶部菜单
    /// </summary>
    private bool Draw()
    {
        bool result = false;
        Color color = GUI.color;
        Color backgroundColor = GUI.backgroundColor;
        GUI.color = Color.white;
        GUI.backgroundColor = Color.white;
        //this.drawFileToolbar();
        this.drawPropertiesBox();
        if (this.drawGraphArea())
        {
            result = true;
        }
        //this.drawPreferencesPane();
        GUI.color = color;
        GUI.backgroundColor = backgroundColor;
        return result;
    }

    private void drawPropertiesBox()
    {
        //绘制顶部菜单
        GUILayout.BeginArea(this.mPropertyToolbarRect, EditorStyles.toolbar);
        ToolbarSelection = GUILayout.Toolbar(ToolbarSelection, ToolbarStrings, EditorStyles.toolbarButton, new GUILayoutOption[0]);
        GUILayout.EndArea();

        //绘制左边菜单
        GUILayout.BeginArea(mPropertyBoxRect, DesignerGUIStyle.PropertyBoxGUIStyle);
        switch (ToolbarSelection)
        {
            case 0://本身属性
                if (DataNow != null)
                {
                    GUILayout.Space(3f);
                    if (DataNow.Owner as EditorBase != null)
                    {
                        DesignerInspector.DrawInspectorGUI(DataNow.Owner as EditorBase, new SerializedObject(this.DataNow.Owner as EditorBase));
                    }
                }
                break;
            case 1://节点树
                tree.drawTaskList(this);
                break;
            case 2://变量
                break;
            case 3://节点属性
                break;
        }
        GUILayout.EndArea();
    }
    /// <summary>
    /// 绘制作图区域
    /// </summary>
    /// <returns></returns>
    private bool drawGraphArea()
    {
        Vector2 vector = GUI.BeginScrollView(new Rect(this.mGraphRect.x, this.mGraphRect.y, this.mGraphRect.width + (float)DesignerHelp.ScrollBarSize,
            this.mGraphRect.height + (float)DesignerHelp.ScrollBarSize), this.mGraphScrollPosition, new Rect(0f, 0f, this.mGraphScrollSize.x, this.mGraphScrollSize.y), true, true);
        if (vector != this.mGraphScrollPosition && Event.current.type != EventType.layout && Event.current.type != EventType.ignore)
        {
            this.mGraphOffset -= (vector - this.mGraphScrollPosition) / this.mGraphZoom;
            this.mGraphScrollPosition = vector;
            //this.mGraphDesigner.graphDirty();
        }
        GUI.EndScrollView();
        GUI.Box(this.mGraphRect, "",DesignerHelp.GraphBackgroundGUIStyle);


        EditorZoomArea.Begin(this.mGraphRect, this.mGraphZoom);
        Vector2 mousePosition;
        if (!this.getMousePositionInGraph(out mousePosition))
        {
            mousePosition = new Vector2(-1f, -1f);
        }
        bool result = false;



        this.mGraphDesigner.drawNodes(mousePosition, this.mGraphOffset, this.mGraphZoom);

        //if (this.mGraphDesigner != null && this.mGraphDesigner.drawNodes(mousePosition, this.mGraphOffset, this.mGraphZoom))
        //{
        //    result = true;
        //}
        //if (this.mIsSelecting)
        //{
        //    GUI.Box(this.getSelectionArea(), "", BehaviorDesignerUtility.SelectionGUIStyle);
        //}
        EditorZoomArea.End();


        //this.drawGraphStatus();
        //this.drawSelectedTaskDescription();
        return result;
    }
    #endregion

    #region 节点操作
    /// <summary>
    /// 添加一个任务
    /// </summary>
    /// <param name="type"></param>
    /// <param name="useMousePosition"></param>
    public void addTask(Type type, bool useMousePosition)
    {
        //得到任务显示的坐标
        Vector2 vector = new Vector2(this.mGraphRect.width / (2f * this.mGraphZoom), 150f);
        if (useMousePosition)
        {
            this.getMousePositionInGraph(out vector);
        }
        vector -= this.mGraphOffset;
        BehaviorUndo.RegisterUndo("Add", this.mGraphDesigner, true, true);
        //====添加节点
        if (this.mGraphDesigner.addNode(DataNow, type, vector) != null)
        {
            //this.saveBehavior();
        }
    }
    #endregion

    private bool mShowPrefPane=false;
    /// <summary>
    /// 点击点是否在绘图区域内
    /// </summary>
    /// <param name="mousePosition"></param>
    /// <returns></returns>
    private bool getMousePositionInGraph(out Vector2 mousePosition)
    {
        mousePosition = this.mCurrentMousePosition;
        if (!this.mGraphRect.Contains(mousePosition))
        {
            return false;
        }
        if (this.mShowPrefPane && this.mPreferencesPaneRect.Contains(mousePosition))
        {
            return false;
        }
        mousePosition -= new Vector2(this.mGraphRect.xMin, this.mGraphRect.yMin);
        mousePosition /= this.mGraphZoom;
        return true;
    }
}


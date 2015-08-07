using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
/// <summary>
/// 设计器辅助类
/// </summary>
public static class DesignerHelp
{
    public static readonly int ToolBarHeight = 18;

    public static readonly int PropertyBoxWidth = 320;

    public static readonly int ScrollBarSize = 15;

    public static readonly int EditorWindowTabHeight = 21;

    public static readonly int PreferencesPaneWidth = 290;

    public static readonly int PreferencesPaneHeight = 208;

    public static readonly float GraphZoomMax = 1f;

    public static readonly float GraphZoomMin = 0.4f;

    public static readonly float GraphZoomSensitivity = 150f;

    public static readonly int LineSelectionThreshold = 7;

    public static readonly int TaskBackgroundShadowSize = 3;

    public static readonly int TitleHeight = 20;

    public static readonly int IconAreaHeight = 52;

    public static readonly int IconSize = 44;

    public static readonly int IconBorderSize = 46;

    public static readonly int ConnectionWidth = 42;

    public static readonly int TopConnectionHeight = 14;

    public static readonly int BottomConnectionHeight = 16;

    public static readonly int TaskConnectionCollapsedWidth = 26;

    public static readonly int TaskConnectionCollapsedHeight = 6;

    public static readonly int MinWidth = 100;

    public static readonly int MaxWidth = 220;

    public static readonly int MaxCommentHeight = 100;

    public static readonly int TextPadding = 20;

    public static readonly float NodeFadeDuration = 0.5f;

    public static readonly int IdentifyUpdateFadeTime = 500;

    public static readonly int MaxIdentifyUpdateCount = 2000;

    public static readonly int TaskPropertiesLabelWidth = 150;

    public static readonly int MaxTaskDescriptionBoxWidth = 400;

    public static readonly int MaxTaskDescriptionBoxHeight = 300;


    #region 分割符
    private static Texture2D contentSeparatorTexture;
    public static Texture2D ContentSeparatorTexture
    {
        get
        {
            if (contentSeparatorTexture == null)
            {
                contentSeparatorTexture = LoadTexture("ContentSeparator.png", null);
            }
            return contentSeparatorTexture;
        }
    }

    public static void DrawContentSeperator(int yOffset)
    {
        Rect lastRect = GUILayoutUtility.GetLastRect();
        lastRect.x = -5f;
        lastRect.y = lastRect.y + (lastRect.height + (float)yOffset);
        lastRect.height = 2f;
        lastRect.width = lastRect.width + 10f;
        GUI.DrawTexture(lastRect, ContentSeparatorTexture);
    }
    #endregion

    #region 展开样式
    private static GUIStyle taskFoldoutGUIStyle = null;
    public static GUIStyle TaskFoldoutGUIStyle
    {
        get
        {
            if (taskFoldoutGUIStyle == null)
            {
                taskFoldoutGUIStyle = new GUIStyle(EditorStyles.foldout);
                taskFoldoutGUIStyle.alignment = TextAnchor.MiddleLeft;
                taskFoldoutGUIStyle.fontSize = 15;
                taskFoldoutGUIStyle.fontStyle = FontStyle.Bold;
            }
            return taskFoldoutGUIStyle;
        }
    }
    #endregion

    #region 绘图区域


    private static GUIStyle graphBackgroundGUIStyle;
    /// <summary>
    /// 绘图区域的背景
    /// </summary>
    public static GUIStyle GraphBackgroundGUIStyle
    {
        get
        {
            if (graphBackgroundGUIStyle == null)
            {
                Texture2D texture2D = new Texture2D(1, 1);
                if (EditorGUIUtility.isProSkin)
                {
                    texture2D.SetPixel(1, 1, new Color(0.1333f, 0.1333f, 0.1333f));
                }
                else
                {
                    texture2D.SetPixel(1, 1, new Color(0.3647f, 0.3647f, 0.3647f));
                }
                texture2D.hideFlags = HideFlags.HideAndDontSave;
                texture2D.Apply();
               graphBackgroundGUIStyle = new GUIStyle(GUI.skin.box);
               graphBackgroundGUIStyle.normal.background = texture2D;
               graphBackgroundGUIStyle.active.background = texture2D;
               graphBackgroundGUIStyle.hover.background = texture2D;
               graphBackgroundGUIStyle.focused.background = texture2D;
               graphBackgroundGUIStyle.normal.textColor = Color.white;
               graphBackgroundGUIStyle.active.textColor = Color.white;
               graphBackgroundGUIStyle.hover.textColor = Color.white;
               graphBackgroundGUIStyle.focused.textColor = Color.white;
            }
            return graphBackgroundGUIStyle;
        }
    }

    private static Texture2D taskConnectionTexture = null;
    /// <summary>
    /// 连线
    /// </summary>
    public static Texture2D TaskConnectionTexture
    {
        get
        {
            if (taskConnectionTexture == null)
            {
                taskConnectionTexture = LoadTexture("TaskConnection.png", null);
            }
            return taskConnectionTexture;
        }
    }

    private static GUIStyle taskTitleGUIStyle = null;
    /// <summary>
    /// 绘图-图形节点-任务名称
    /// </summary>
    public static GUIStyle TaskTitleGUIStyle
    {
        get
        {
            if (taskTitleGUIStyle == null)
            {
                taskTitleGUIStyle = new GUIStyle(GUI.skin.label);
                taskTitleGUIStyle.alignment = TextAnchor.UpperCenter;
                taskTitleGUIStyle.fontSize = 12;
                taskTitleGUIStyle.fontStyle = FontStyle.Normal;
            }
            return taskTitleGUIStyle;
        }
    }
    #endregion


    #region 加载硬盘上的资源
    public static Texture2D LoadTexture(string imageName,ScriptableObject obj = null)
    {
        Texture2D texture2D = null;
        imageName = Application.dataPath + @"\" + string.Format(@"\Designer\trunk\Resource\{0}{1}",  "Dark", imageName);
        Stream manifestResourceStream = null;
        try
        {
            manifestResourceStream = File.OpenRead(imageName);
        }
        catch
        {
            Debug.Log("文件不存在:" + imageName);
            return texture2D;
        }

        if (manifestResourceStream != null)
        {
            texture2D = new Texture2D(0, 0);
            texture2D.LoadImage(ReadToEnd(manifestResourceStream));
            manifestResourceStream.Close();
        }
        if (texture2D != null)
        {
            texture2D.hideFlags = HideFlags.HideAndDontSave;
        }
        return texture2D;
    }
    private static byte[] ReadToEnd(Stream stream)
    {
        byte[] array = new byte[16384];
        byte[] result;
        using (MemoryStream memoryStream = new MemoryStream())
        {
            int count;
            while ((count = stream.Read(array, 0, array.Length)) > 0)
            {
                memoryStream.Write(array, 0, count);
            }
            result = memoryStream.ToArray();
        }
        return result;
    }
    #endregion
}


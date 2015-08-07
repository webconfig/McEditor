using UnityEditor;
using UnityEngine;
public static class DesignerGUIStyle
{
    private static GUIStyle propertyBoxGUIStyle = null;
    public static GUIStyle PropertyBoxGUIStyle
    {
        get
        {
            if (propertyBoxGUIStyle == null)
            {
                initPropertyBoxGUIStyle();
            }
            return propertyBoxGUIStyle;
        }
    }
    private static void initPropertyBoxGUIStyle()
    {
        propertyBoxGUIStyle = new GUIStyle();
        propertyBoxGUIStyle.padding = new RectOffset(2, 2, 0, 0);
    }
}


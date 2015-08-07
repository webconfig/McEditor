﻿using System;
using UnityEngine;

public static class RectExtensions
{
    public static Vector2 TopLeft(this Rect rect)
    {
        return new Vector2(rect.xMin, rect.yMin);
    }

    public static Rect ScaleSizeBy(this Rect rect, float scale)
    {
        return rect.ScaleSizeBy(scale, rect.center);
    }

    public static Rect ScaleSizeBy(this Rect rect, float scale, Vector2 pivotPoint)
    {
        Rect result = rect;
        result.x = result.x - pivotPoint.x;
        result.y = result.y - pivotPoint.y;
        result.xMin = result.xMin * scale;
        result.xMax = result.xMax * scale;
        result.yMin = result.yMin * scale;
        result.yMax = result.yMax * scale;
        result.x = result.x + pivotPoint.x;
        result.y = result.y + pivotPoint.y;
        return result;
    }

    public static Rect ScaleSizeBy(this Rect rect, Vector2 scale)
    {
        return rect.ScaleSizeBy(scale, rect.center);
    }

    public static Rect ScaleSizeBy(this Rect rect, Vector2 scale, Vector2 pivotPoint)
    {
        Rect result = rect;
        result.x = result.x - pivotPoint.x;
        result.y = result.y - pivotPoint.y;
        result.xMin = result.xMin * scale.x;
        result.xMax = result.xMax * scale.x;
        result.yMin = result.yMin * scale.y;
        result.yMax = result.yMax * scale.y;
        result.x = result.x + pivotPoint.x;
        result.y = result.y + pivotPoint.y;
        return result;
    }
}
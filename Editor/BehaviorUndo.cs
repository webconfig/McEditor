﻿using System;
using UnityEditor;
using UnityEngine;

public class BehaviorUndo
{
    public static void RegisterUndo(string undoName, UnityEngine.Object undoObject = null, bool registerUndo = true, bool sceneUndo = true)
    {
        if (undoObject != null)
        {
            Undo.RecordObject(undoObject, undoName);
        }
    }

    public static void RegisterCompleteUndo(string undoName, UnityEngine.Object undoObject)
    {
        Undo.RegisterCompleteObjectUndo(undoObject, undoName);
    }

    public static Component AddComponent(GameObject undoObject, Type type)
    {
        return Undo.AddComponent(undoObject, type);
    }

    public static void DestroyObject(UnityEngine.Object undoObject, bool registerScene)
    {
        Undo.DestroyObjectImmediate(undoObject);
    }
}

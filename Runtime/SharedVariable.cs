using System;
using UnityEngine;
public abstract class SharedVariable : ScriptableObject
{
    [SerializeField]
    protected bool mIsShared;

    [SerializeField]
    protected SharedVariableTypes mValueType;

    public bool IsShared
    {
        get
        {
            return this.mIsShared;
        }
        set
        {
            this.mIsShared = value;
        }
    }

    public SharedVariableTypes ValueType
    {
        get
        {
            return this.mValueType;
        }
    }

    public abstract object GetValue();

    public abstract void SetValue(object value);
}
public enum SharedVariableTypes
{
    Int,
    Float,
    Bool,
    String,
    Vector2,
    Vector3,
    Vector4,
    Quaternion,
    Color,
    Rect,
    GameObject,
    Transform,
    Object
}
using UnityEngine;
/// <summary>
/// 数据资源
/// </summary>
public class DataSource
{
    public string Name = "Behavior";


    public EditorBase Owner;

    public DataSource(EditorBase owner)
    {
        Owner = owner;
    }
}


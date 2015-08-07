using UnityEngine;
public class EditorBase:MonoBehaviour
{
    /// <summary>
    /// 数据
    /// </summary>
    [SerializeField]
    public DataSource Data;

    public EditorBase()
    {
        Data = new DataSource(this);
    }
}


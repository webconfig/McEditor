using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[Serializable]
public class GraphDesigner : ScriptableObject
{
    private NodeDesigner mRootNode; 
	#region 绘制节点和连线
        /// <summary>
        ///  绘制节点
        /// </summary>
        /// <param name="mousePosition"></param>
        /// <param name="offset"></param>
        /// <param name="graphZoom"></param>
        /// <returns></returns>
    public bool drawNodes(Vector2 mousePosition, Vector2 offset, float graphZoom)
    {
        if (mRootNode != null)
        {
            this.drawNodeChildren(mRootNode, offset, false);//绘制节点
        }
        return false;
    }
    private bool drawNodeChildren(NodeDesigner nodeDesigner, Vector2 offset, bool disabledNode)
    {
        if (nodeDesigner == null)
        {
            return false;
        }
        bool result = false;
        nodeDesigner.Draw(true);
        
        //if (nodeDesigner.IsParent)
        //{
        //    ParentTask parentTask = nodeDesigner.Task as ParentTask;
        //    if (!parentTask.NodeData.Collapsed && parentTask.Children != null)
        //    {
        //        for (int i = parentTask.Children.Count - 1; i > -1; i--)
        //        {
        //            if (parentTask.Children[i] != null && this.drawNodeChildren(parentTask.Children[i].NodeData.NodeDesigner as NodeDesigner, offset, parentTask.NodeData.Disabled || disabledNode))
        //            {
        //                result = true;
        //            }
        //        }
        //    }
        //}
        return result;
    }
    #endregion

    #region 添加/删除 节点
    [SerializeField]
    private int NextTaskID=0;
    /// <summary>
    /// 添加一个节点
    /// </summary>
    /// <param name="behaviorSource"></param>
    /// <param name="type"></param>
    /// <param name="position"></param>
    /// <returns></returns>
    public NodeDesigner addNode(DataSource data, Type type, Vector2 position)
    {
        BehaviorUndo.RegisterUndo("Add Task", this, true, true);
        BehaviorUndo.RegisterUndo("Add Task", data.Owner, false, true);

        ActionBase _action;
        if (this.mRootNode == null)
        {//没有根节点，创建一个根节点
            _action = (ScriptableObject.CreateInstance("RootTask") as ActionBase);
            this.mRootNode = ScriptableObject.CreateInstance<NodeDesigner>();
            mRootNode.loadNode(_action, data, new Vector2(position.x, position.y - 120f), ref NextTaskID);
            //mRootNode.RootDisplay();
        }

        _action = (ScriptableObject.CreateInstance(type) as ActionBase);
        if (_action == null)
        {
            Debug.LogError(string.Format("Unable to create task of type {0}. Is the class name the same as the file name?", type));
            return null;
        }

        //编辑器创建一个ui节点
        NodeDesigner nodeDesigner = ScriptableObject.CreateInstance<NodeDesigner>();
        nodeDesigner.loadNode(_action, data, position, ref NextTaskID);

        //if (this.mRootNode.OutgoingNodeConnections.Count == 0)
        //{
        //    //创建一个连接
        //    this.mActiveNodeConnection = ScriptableObject.CreateInstance<NodeConnection>();
        //    this.mActiveNodeConnection.loadConnection(this.mRootNode, NodeConnectionType.Outgoing);
        //    this.connectNodes(behaviorSource, nodeDesigner);
        //}
        //else
        //{
        //    this.mDetachedNodes.Add(nodeDesigner);
        //}
        return nodeDesigner;
    }
    #endregion
}

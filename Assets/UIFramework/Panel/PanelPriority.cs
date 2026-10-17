using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 规定面板属于哪个层的,便于管理
/// </summary>
public enum PanelPriority
{ 
    None=0,
    Prioritary=1,
    Tutorial=2,
    Blocker=3
}
//存储优先级(枚举) 和 父对象
[System.Serializable]
public class PanelPriorityLayerListEntry
{
    [SerializeField]
    [Tooltip("指定下面板层的优先级")]
    private PanelPriority priority;
    [SerializeField]
    [Tooltip("此优先级下所有面板的父节点")]
    private Transform targetParent;

    public Transform TargetParent
    {
        get { return targetParent; }
        set { targetParent = value; }
    }

    public PanelPriority Priority
    {
        get { return priority; }
        set { priority = value; }
    }

    public PanelPriorityLayerListEntry(PanelPriority prio,Transform parent)
    {
        priority = prio;
        targetParent = parent;
    }
}

/// <summary>
/// Z：字典存储PanelLayer的节点，用于分层显示
/// </summary>
[System.Serializable]
public class PanelPriorityLayerList 
{
    [SerializeField]
    [Tooltip("根据面板的优先级查找并存储对应的GameObject,渲染优先级由这些GameObject在层级结构中的顺序决定，列表在inspector窗口中配置")]
    private List<PanelPriorityLayerListEntry> paraLayers = null;
    private Dictionary<PanelPriority, Transform> lookUp;
    
    public Dictionary<PanelPriority, Transform> ParaLayerLookUp
    {
        get
        {
            if(lookUp==null || lookUp.Count == 0)
            {
                CacheLookUp();
            }
            return lookUp;
        }
    }

    private void CacheLookUp()
    {
        lookUp=new Dictionary<PanelPriority, Transform>();
        for(int i = 0; i < paraLayers.Count; i++)
        {
            lookUp.Add(paraLayers[i].Priority,paraLayers[i].TargetParent);
        }
    }

    public PanelPriorityLayerList(List<PanelPriorityLayerListEntry> entries)
    {
        paraLayers = entries;
    }
}



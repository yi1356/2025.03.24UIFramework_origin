using Assets.UIFramework.Window;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 界面属性的接口
/// </summary>
public interface IScreenProperties{}

/// <summary>
/// 面板属性的接口
/// </summary>
public interface IPanelProperties : IScreenProperties
{
    //设置生成的panel面板在那一层(哪一个子物体下)
    PanelPriority Priority { get; set; }
}

/// <summary>
/// 窗口属性的接口
/// </summary>
public interface IWindowProperties : IScreenProperties
{
    //决定显示时是进入显示队列，还是直接显示在前面？？？
    WindowPriority WindowQueuePriority { get; set; }
    //设置显示 显示队列或历史栈 中的Windw时，是否隐藏当前界面
    [Tooltip("当其他窗口被置前时，自己是否隐藏")]
    bool HidOnForegroundLost { get; set; }
    //设置是否显示在最前面，且后面有一个黑色面板
    bool IsPopup { get; set; }
    //设置是否支持 覆盖预设体中的WindowProperities设置
    bool SuppressPrefabProperties { get; set; }
}

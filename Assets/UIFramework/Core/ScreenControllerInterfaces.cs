using Assets.UIFramework.Window;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// UI界面的基类，窗口，面板这些都继承它，比如 AWindowController，PanelController
/// </summary>
public interface IScreenController
{
    string  ScreenId{ set; get; }//Z：在记录界面、打开、关闭时都会用到
    [Tooltip("标志是否时显示状态")]
    bool IsVisible { get; }
    

    void Show(IScreenProperties props = null);
    void Hide(bool animate=true);

    //基本都是在WindowsUILayer中注册，在UIScreenController中触发（除了ScreenDestroyed在UILayer中注册）
    Action<IScreenController> InTransitionFinished { get; set; }
    Action<IScreenController> OutTransitionFinished { get; set; }//Z：UI关闭动画完成后触发
    Action<IScreenController> CloseRequest { get; set; }//Z：开始关闭时触发
    Action<IScreenController> ScreenDestroyed { get; set; }
}

public interface IWindowController : IScreenController
{
    [Tooltip("在显示历史栈 或 UI队列中的window时，是否会隐藏现在的window")]
    bool HideOnForegroundLost { get; }
    [Tooltip("决定了是否显示在最前面，且后面是一个黑背景")]
    bool IsPopup{ get; }
    WindowPriority windowPriority { get; }
}

/// <summary>
/// 所有面板必须实现的接口
/// </summary>
public interface IPanelController : IScreenController
{ 
    PanelPriority priority { get; }
}


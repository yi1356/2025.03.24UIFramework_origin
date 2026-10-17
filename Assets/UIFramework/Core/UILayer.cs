using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 基础的UI Layer层()
/// </summary>
public abstract  class UILayer<TScreen> : MonoBehaviour where TScreen :IScreenController
{
    protected Dictionary<string, TScreen> registeredScreen;


    /// <summary>
    /// 显示界面
    /// </summary>
    /// <param name="screen"></param>
    public abstract void ShowScreen(TScreen screen);

    /// <summary>
    /// 显示一些界面，带一些参数
    /// </summary>
    /// <typeparam name="TProps">属性类型</typeparam>
    /// <param name="screen">界面类型属性</param>
    /// <param name="properties">属性参数</param>
    public abstract void ShowScreen<TProps>(TScreen screen, TProps properties) where TProps : IScreenProperties;

    /// <summary>
    /// 隐藏界面
    /// </summary>
    /// <param name="screen">界面类型参数</param>
    public abstract void HideScreen(TScreen screen);

    /// <summary>
    /// 初始化layer层
    /// </summary>
    public virtual void Initalize()
    {
        registeredScreen = new Dictionary<string, TScreen>();
    }

    /// <summary>
    /// 传进来的界面当作层的子节点
    /// </summary>
    /// <param name="controller">界面的controller</param>
    /// <param name="screeTransform">界面的节点(不是父节点不要搞混，只是界面的controller的transform)</param>
    public virtual void ReparentScreen(IScreenController controller,Transform screeTransform)
    {
        screeTransform.SetParent(transform,false);
    }
    /// <summary>
    /// 注册界面的Controller带上明确的界面id
    /// </summary>
    /// <param name="screenId">界面id</param>
    /// <param name="controller">界面Controller</param>
    public void RegisterScreen(string screenId, TScreen controller)
    {
        if (!registeredScreen.ContainsKey(screenId))
        {
            ProcessScreenRegister(screenId, controller);
        }
        else
        {
            Debug.LogError("[AUILayerController] Screen controller already registered for id: " + screenId);
        }
    }
    /// <summary>
    /// 根据id取消注册界面的controller
    /// </summary>
    /// <param name="screenId">界面id</param>
    /// <param name="controller">被取消的界面controller</param>
    public void UnRegisterScreen(string screenId, TScreen screen)
    {
        if (registeredScreen.ContainsKey(screenId))
        {
            ProcessUnRegisterScreen(screenId, screen);
        }
        else
        {
            Debug.LogError("[AUILayerController] Screen controller not registered for id: " + screenId);
        }
    }
    /// <summary>
    /// 根据id去找界面controller,并显示出来
    /// </summary>
    /// <param name="screenId"></param>
    public void ShowScreenById(string screenId)
    {
        TScreen ctl;
        if(registeredScreen.TryGetValue(screenId,out ctl))
        {
            ShowScreen(ctl);
        }
        else
        {
            Debug.LogError("[AUILayerController] Could not show Screen ID  " + screenId + " not registered to this layer!");
        }
    }

    /// <summary>
    /// 根据id显示具体的controller,带上具体的参数属性
    /// </summary>
    /// <typeparam name="TProps">属性类型</typeparam>
    /// <param name="screenId">界面ID</param>
    /// <param name="properties">属性参数</param>
    public void ShowScreenById<TProps>(string screenId, TProps properties) where TProps :IScreenProperties
    {
        TScreen ctl;
        if (registeredScreen.TryGetValue(screenId, out ctl))
        {
            ShowScreen(ctl,properties);
        }
        else
        {
            Debug.LogError("[AUILayerController] Could not show Screen ID properties " + screenId + " not registered to this layer!");
        }
    }

    /// <summary>
    /// 根据id隐藏界面
    /// </summary>
    /// <param name="screenId"></param>
    public void HideScreenById(string screenId)
    {
        TScreen ctl;
        if (registeredScreen.TryGetValue(screenId,out ctl))
        {
            HideScreen(ctl);
        }
        else
        {
            Debug.LogError("[AUILayerController] Could not Hide Screen ID" + screenId + " not registered to this layer!");
        }
    }

    /// <summary>
    /// 根据id看是否注册了
    /// </summary>
    /// <param name="screenId"></param>
    /// <returns></returns>
    public bool IsScreenRegistered(string screenId)
    {
        return registeredScreen.ContainsKey(screenId);
    }

    /// <summary>
    /// 隐藏所有界面
    /// </summary>
    /// <param name="shouldAnimateWhenHiding"></param>
    public virtual void HideAll(bool shouldAnimateWhenHiding = true)
    {
        foreach (var screen in registeredScreen)
        {
            screen.Value.Hide(shouldAnimateWhenHiding);
        }
    }


   

    

    //???
    protected virtual void ProcessScreenRegister(string screenId,TScreen controller)
    {
        controller.ScreenId = screenId;
        registeredScreen.Add(screenId, controller);
        controller.ScreenDestroyed += OnScreenDestroyed;

    }
    //???
    protected virtual void  ProcessUnRegisterScreen(string screenId, TScreen controller)
    {
        registeredScreen.Remove(screenId);
        controller.ScreenDestroyed -= OnScreenDestroyed;
    }
    private void OnScreenDestroyed(IScreenController screen)
    {
        if(!string.IsNullOrEmpty(screen.ScreenId)&&registeredScreen.ContainsKey(screen.ScreenId))
        {
            UnRegisterScreen(screen.ScreenId,(TScreen)screen);
        }
        
    }

    
}

using Assets.UIFramework;
using Assets.UIFrameworkExamples.Scripes;
using Assets.UIFrameworkExamples.Scripes.Extras;
using Assets.UIFrameworkExamples.Scripes.ScreenController;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;
/// <summary>
/// Z：真正的UI启动入口
/// 开始时，创建UIFrame物体，并添加一些监听
/// </summary>
public class UIDemoController : MonoBehaviour
{
    [SerializeField] private UIsettings defaultUISettings = null;
    [SerializeField] private FakePlayerData fakePlayerData = null;
    [SerializeField] private Camera cam = null;
    [SerializeField] private Transform transformToFollow = null;

    private UIFrame uiFrame;

    private void Awake()
    {
        uiFrame = defaultUISettings.CreateUIInstance();
        Signals.Get<StartDemoSignal>().AddListener(OnStartDemo);//Z：点击开始按钮 调用
        Signals.Get<NavigateToWindowSignal>().AddListener(OnNavigateToWindow);//Z：点击导航面板中的按钮调用
        Signals.Get<ShowConfirmationPopupSignal>().AddListener(OnShowConfirmationPopup);//Z：点击确定按钮调用
    }

    private void OnDestroy()
    {
        Signals.Get<StartDemoSignal>().RemoveListener(OnStartDemo);
        Signals.Get<NavigateToWindowSignal>().RemoveListener(OnNavigateToWindow);
        Signals.Get<ShowConfirmationPopupSignal>().RemoveListener(OnShowConfirmationPopup);
    }

    private void Start()
    {
        uiFrame.OpenWindow(ScreenIds.StartGameWindow);
    }

    private void OnStartDemo()
    {
        uiFrame.ShowPanel(ScreenIds.NavigationPanel);
        uiFrame.ShowPanel(ScreenIds.ToastPanel);
    }

    private void OnNavigateToWindow(string windowId)
    {
        uiFrame.CloseCurrentWindow();
        //Debug.Log("测试1111111111111111111111+" + windowId);
        switch (windowId)
        {
            case ScreenIds.PlayerWindow:
                uiFrame.OpenWindow(windowId, new PlayerWindowProperties(fakePlayerData.LevelProgress));
                //Debug.Log("测试2222222222222222222ScreenIds.PlayerWindow" + "已经执行");
                break;
            case ScreenIds.CameraProjectionWindow:
                uiFrame.OpenWindow(windowId, new CameraProjectionWindowProperties(cam, transformToFollow));
                break;

            default:
                uiFrame.OpenWindow(windowId);
                break;
        }

    }

    private void OnShowConfirmationPopup(ConfirmationPopupProperties popupPayLoad)
    {
        uiFrame.OpenWindow(ScreenIds.ConfirmationPopup, popupPayLoad);
    } 




}

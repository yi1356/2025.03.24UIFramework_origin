using Assets.UIFramework.Panel;
using Assets.UIFramework.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.UIFramework
{
    /// <summary>
    /// 所有的对外接口都在这，相当于UImanager之类
    /// 主要是注册/取消注册面板+窗口、显示/隐藏面板（ID、类等）+窗口等
    /// </summary>
    public class UIFrame : MonoBehaviour
    {
        [Tooltip("如果您想动手初始化此UI框架，请将其设置为false")]
        [SerializeField] private bool initializeOnAwake = true;
        private PanelUILayer panelLayer;
        private WindowUILayer windowLayer;

        private Canvas mainCanvas;
        private GraphicRaycaster graphicRaycaster;

        /// <summary>
        /// 主Canvas
        /// </summary>
        public Canvas MainCanvas
        {
            get
            {
                if (mainCanvas == null)
                {
                    mainCanvas = GetComponent<Canvas>();
                }
                return mainCanvas;
            }
        }

        /// <summary>
        /// Canvas的摄像机
        /// </summary>

        public Camera GetCamera
        {
            get
            {
                return mainCanvas.worldCamera;
            }
        }
        private void Awake()
        {
            if (initializeOnAwake)
            {
                Initialize();
            }
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public virtual void Initialize()
        {
            if (panelLayer == null)
            {
                panelLayer = gameObject.GetComponentInChildren<PanelUILayer>(true);
                if (panelLayer == null)
                {
                    Debug.Log("UI Frame UI Frame Lacks Panel Layer!");
                }
                else
                {
                    panelLayer.Initalize();
                }
            }

            if (windowLayer == null)
            {
                windowLayer = gameObject.GetComponentInChildren<WindowUILayer>(true);
                if (windowLayer == null)
                {
                    Debug.Log("UI Frame UI Frame Lacks Window Layer!");
                }
                else
                {
                    windowLayer.Initalize();
                    windowLayer.RequestScreenBlock += OnRequestScreeenBlock;
                    windowLayer.RequestScreenUnblock += OnRequestScreenUnblock;
                }
            }
            graphicRaycaster = MainCanvas.GetComponent<GraphicRaycaster>();
        }



        /// <summary>
        /// 仅通过id显示一个面板
        /// </summary>
        /// <param name="screenId"></param>
        public void ShowPanel(string screenId)
        {
            panelLayer.ShowScreenById(screenId);
        }

        /// <summary>
        /// 通过id和属性显示面板
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="screenId"></param>
        /// <param name="properties"></param>
        public void ShowPanel<T>(string screenId, T properties) where T : IPanelProperties
        {
            panelLayer.ShowScreenById<T>(screenId, properties);
        }

        /// <summary>
        /// 仅通过Id隐藏面板
        /// </summary>
        /// <param name="screenId"></param>
        public void HidePanel(string screenId)
        {
            panelLayer.HideScreenById(screenId);
        }

        /// <summary>
        /// 仅通过id显示窗口
        /// </summary>
        /// <param name="screenId"></param>
        public void OpenWindow(string screenId)
        {
            windowLayer.ShowScreenById(screenId);
        }

        /// <summary>
        /// 通过id和属性 显示窗口
        /// </summary>
        /// <param name="screenId"></param>
        public void OpenWindow<T>(string screenId, T properties) where T : IWindowProperties
        {
            windowLayer.ShowScreenById<T>(screenId, properties);
        }

        /// <summary>
        /// 仅通过Id关闭窗口
        /// </summary>
        /// <param name="screenId"></param>
        public void CloseWindow(string screenId)
        {
            windowLayer.HideScreenById(screenId);
        }

        /// <summary>
        /// 关闭当前的窗口
        /// </summary>
        public void CloseCurrentWindow()
        {
            if (windowLayer.CurrentWindow!=null)
            {
                CloseWindow(windowLayer.CurrentWindow.ScreenId);
            }
        }

        /// <summary>
        /// 二次包装了方法，给id就显示，找到什么是什么
        /// </summary>
        /// <param name="screenId"></param>
        public void ShowScreen(string screenId)
        {
            Type type;
            if (IsScreenRegistered(screenId,out type))
            {
                if (type == typeof(IWindowController))
                {
                    OpenWindow(screenId);
                }
                else if(type==typeof(IPanelController))
                {
                    ShowPanel(screenId);
                }

            }
            else
            {
                Debug.LogError(string.Format("Tried to open Screen id {0} but it's not registered as Window or Panel!",
                    screenId));
            }

        }



        /// <summary>
        /// 注册一个界面，screenTransform其实是 controller的transform
        /// </summary>
        /// <param name="screenId"></param>
        /// <param name="controller">子界面</param>
        /// <param name="screenTransform"> 界面的节点(不是父节点不要搞混，只是界面的controller的transform) </param>
        public void RegisterScreen(string screenId,IScreenController controller,Transform screenTransform)
        {
            IWindowController window = controller as IWindowController;
            if (window != null)
            {
                windowLayer.RegisterScreen(screenId,window);
                if (screenTransform!=null)
                {
                    windowLayer.ReparentScreen(controller,screenTransform);
                }
                return;
            }

            IPanelController panel = controller as IPanelController;
            if (panel != null)
            {
                panelLayer.RegisterScreen(screenId,panel);
                if (screenTransform!=null)
                {
                    panelLayer.ReparentScreen(controller,screenTransform);
                }

            }
        }

        /// <summary>
        /// 注册一个面板，不注册显示不出来
        /// </summary>
        /// <typeparam name="Tpanel"></typeparam>
        /// <param name="screenId"></param>
        /// <param name="controller"></param>
        public void RegisterPanel<Tpanel>(string screenId,Tpanel controller) where Tpanel:IPanelController
        {
            panelLayer.RegisterScreen(screenId,controller);
        }

        /// <summary>
        /// 注销一个面板
        /// </summary>
        /// <typeparam name="Tpanel"></typeparam>
        /// <param name="screenId"></param>
        /// <param name="controller"></param>
        public void UnregisterPanel<Tpanel>(string screenId, Tpanel controller) where Tpanel : IPanelController
        {
            panelLayer.UnRegisterScreen(screenId, controller);
        }

        /// <summary>
        /// 注册一个面板，不注册显示不出来
        /// </summary>
        /// <typeparam name="Tpanel"></typeparam>
        /// <param name="screenId"></param>
        /// <param name="controller"></param>
        public void RegisterWindow<TWindow>(string screenId, TWindow controller) where TWindow : IWindowController
        {
            windowLayer.RegisterScreen(screenId, controller);
        }

        /// <summary>
        /// 注销窗口
        /// </summary>
        /// <typeparam name="Tpanel"></typeparam>
        /// <param name="screenId"></param>
        /// <param name="controller"></param>
        public void UnregisterWindowl<TWindow>(string screenId, TWindow controller) where TWindow : IWindowController
        {
            windowLayer.UnRegisterScreen(screenId, controller);
        }



        /// <summary>
        /// 根据面板Id检测是否开启中
        /// </summary>
        /// <param name="panelId"></param>
        /// <returns></returns>
        public bool IsPanelOpen(string panelId)
        {
            return panelLayer.IsPanelVisible(panelId);
        }

        /// <summary>
        /// 隐藏所有界面
        /// </summary>
        /// <param name="animate"></param>
        public void HideAll(bool animate=true)
        {
            CloseAllWindows(animate);
            HideAllPanels(animate);
        }
        /// <summary>
        /// 隐藏所有面板层的界面
        /// </summary>
        /// <param name="animate"></param>
        private void HideAllPanels(bool animate=true)
        {
            panelLayer.HideAll(animate);
        }

        /// <summary>
        /// 隐藏所有窗口层,的界面
        /// </summary>
        /// <param name="animate"></param>
        private void CloseAllWindows(bool animate=true)
        {
            windowLayer.HideAll(animate);
        }

        /// <summary>
        /// 检查界面是否被注册过了
        /// </summary>
        /// <param name="screenId"></param>
        /// <returns></returns>
        public bool IsScreenRegistered(string screenId)
        {
            if (windowLayer.IsScreenRegistered(screenId))
            {
                return true;
            }
            if (panelLayer.IsScreenRegistered(screenId))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 和上面一样,只是多了个类型的返回
        /// </summary>
        /// <param name="screenId"></param>
        /// <returns></returns>
        public bool IsScreenRegistered(string screenId,out Type type)
        {
            if (windowLayer.IsScreenRegistered(screenId))
            {
                type = typeof(IWindowController);
                return true;
            }
            if (panelLayer.IsScreenRegistered(screenId))
            {
                type = typeof(IPanelController);
                return true;
            }
            type = null;
            return false;
        }

        private void  OnRequestScreeenBlock()//界面做动画时 阻止交互
        {
            if (graphicRaycaster!=null)
            {
                graphicRaycaster.enabled = false;
            }
        }
        private void OnRequestScreenUnblock()
        {
            if (graphicRaycaster !=null)
            {
                graphicRaycaster.enabled = true;
            }
        }

        
    }
}

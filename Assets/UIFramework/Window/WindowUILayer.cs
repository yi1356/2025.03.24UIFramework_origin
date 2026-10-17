using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.UIFramework.Window
{
    /// <summary>
    /// 这个Layer层控制所有窗口，最上层类型window(半黑)，普通window
    /// 看有显示记录和队列的，并且一次只显示一个
    /// </summary>
    class WindowUILayer:UILayer<IWindowController>
    {
        [SerializeField] private WindowParaLayer priorityParaLayer =null;//Z：放最前方窗口用的，且有半透明黑色背景
        public IWindowController CurrentWindow { get; private set; }

        private Queue<WindowHistoryEntry> windowQueue;//显示队列[Z：在inspector面板中设置window Queue Priority来决定是否进入队列/直接显示；在HindScreen()中判断队列中有Window就显示]   [适用于连续多个弹窗显示隐藏]
        private Stack<WindowHistoryEntry> windowHistory;//历史栈 [Z：显示方法DoShow()中会将window加入历史队列，隐藏方法HideScreen()会将window弹出；在HindScreen()中判断历史栈中有Window就显示(感觉是我们隐藏界面不用WindowLayer的方法时才管用；常用的地方估计是显示一个window，将HideOnForgroundLost设置true的currentWindow会自动隐藏，当再次调用HindScreen()时会显示这个)]

        public event Action RequestScreenBlock;// 在window动画开始时触发，用于通知其他系统阻塞用户交互
        public event Action RequestScreenUnblock;//在window动画结束时触发，用于通知其他系统  解除阻塞用户交互

        public bool IsScreenTransitionInProgress { get { return screensTransitioning.Count != 0; } }//判断是否有弹窗在做动画

        private HashSet<IScreenController> screensTransitioning;//存储正在做动画的弹窗（开始显示和隐藏时加入，在开始显示动画和隐藏动画结束后移除）
        //Z：重写初始化
        public override void Initalize()
        {
            base.Initalize();
            registeredScreen = new Dictionary<string, IWindowController>();
            windowQueue = new Queue<WindowHistoryEntry>();
            windowHistory = new Stack<WindowHistoryEntry>();
            screensTransitioning = new HashSet<IScreenController>();

        }

        //Z：重写注册方法 (增加了为注册界面的显示/消失监听添加方法)
        protected override void ProcessScreenRegister(string screenId, IWindowController controller)
        {
            base.ProcessScreenRegister(screenId, controller);
            controller.InTransitionFinished += OnInAnimationFinished;
            controller.OutTransitionFinished += OnOutAnimationFinished;
            controller.CloseRequest += OnCloseRequestedByWindow;
        }

        //Z：重写取消注册方法 (减去了  为注册界面的显示/消失监听添加方法)
        protected override void ProcessUnRegisterScreen(string screenId, IWindowController controller)
        {
            base.ProcessUnRegisterScreen(screenId, controller);
            controller.InTransitionFinished -= OnInAnimationFinished;
            controller.OutTransitionFinished -= OnOutAnimationFinished;
            controller.CloseRequest -= OnCloseRequestedByWindow;
        }

        public override void ShowScreen(IWindowController screen)
        {
            ShowScreen<IWindowProperties>(screen,null);
        }

        //Z：重写显示方法，判断是添加到显示队列/直接显示,且会自动压入历史栈中。
        public override void ShowScreen<TProps>(IWindowController screen, TProps properties)
        {
            IWindowProperties windowProp = properties as IWindowProperties;
            if (ShouldEnqueue(screen, windowProp))
            {
                EnqueueWindow(screen, windowProp);   
            }
            else
            {
                DoShow(screen,windowProp);
            }
        }

        //Z：重写隐藏方法，会自动从历史栈弹出，判断显示队列中有无弹窗并**显示**/历史栈中有无弹窗并**显示**
        public override void HideScreen(IWindowController screen)
        {
            if (screen==CurrentWindow)
            {
                windowHistory.Pop();
                AddTransition(screen);
                screen.Hide();
                Debug.Log("测试 WindowUILayer.HideScreen已经执行");//测试过隐藏窗口时会执行这里

                CurrentWindow = null;

                if (windowQueue.Count > 0)
                {
                    ShowNextInQueue();
                }else if (windowHistory.Count>0)
                {
                    ShowPreviousInHistory();
                }
            }
            else
            {
                Debug.LogError(
                    string.Format(
                        "[WindowUILayer] Hide requested on WindowId {0} but that's not the currently open one ({1})! Ignoring request.",
                        screen.ScreenId, CurrentWindow != null ? CurrentWindow.ScreenId : "current is null"));
            }
        }

        public override void HideAll(bool shouldAnimateWhenHiding = true)
        {
            base.HideAll(shouldAnimateWhenHiding);
            CurrentWindow = null;
            priorityParaLayer.RefreshDarken();
            windowHistory.Clear();
        }

        public override void ReparentScreen(IScreenController controller, Transform screenTransform)
        {
            IWindowController window = controller as IWindowController;
            if (window == null)
            {
                Debug.LogError("[WindowUILayer] Screen " + screenTransform.name + " is not a Window!");
            }
            else
            {
                if (window.IsPopup)
                {
                    priorityParaLayer.AddScreen(screenTransform);
                    return;
                }
            }
            base.ReparentScreen(controller, screenTransform);
        }

        private void EnqueueWindow<TProp>(IWindowController screen,TProp properties) where TProp:IWindowProperties
        {
            windowQueue.Enqueue(new WindowHistoryEntry(screen,(IWindowProperties)properties));
        }
        /// <summary>
        /// Z：判断是否应该  放入显示队列中
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="windowProp"></param>
        /// <returns></returns>
        private bool ShouldEnqueue(IWindowController controller,IWindowProperties windowProp)//TODU：ShouldEnqueue这里还是看不懂?????????????????????
        {
            if(CurrentWindow==null && windowQueue.Count != 0) //Z：现在显示window不为空 并且 显示队列中数量不为 0 
            {
                //Debug.Log("测试+WindowUILayer.ShouldEnqueue()--------------CurrentWindow==null && windowQueue.Count != 0+false");
                return false;
            }

            if (windowProp!=null && windowProp.SuppressPrefabProperties)//Z：传入的参数为不为空 且 支持 覆盖预设体中的WindowProperities设置
            {
                //Debug.Log("测试+WindowUILayer.ShouldEnqueue()--------------windowProp!=null && windowProp.SuppressPrefabProperties"+ (windowProp.WindowQueuePriority != WindowPriority.ForceForeGround));
                return windowProp.WindowQueuePriority != WindowPriority.ForceForeGround;      // Z：如果传入参数中WindowQueuePriority不是直接显示则 进入队列
            }

            if (controller.windowPriority!=WindowPriority.ForceForeGround)//Z：预制体中的设置windowPriority 不是直接显示则 进入队列
            {
                //Debug.Log("测试+WindowUILayer.ShouldEnqueue()--------------controller.windowPriority!=WindowPriority.ForceForeGround+false");
                return true;
            }

            //Debug.Log("测试+WindowUILayer.ShouldEnqueue()+false");
            return false;
        }


        private void ShowPreviousInHistory()
        {
            if (windowHistory.Count > 0)
            {
                WindowHistoryEntry window = windowHistory.Pop();
                DoShow(window);
            }
        }
        private void ShowNextInQueue()
        {
            if (windowQueue.Count>0)
            {
                WindowHistoryEntry window = windowQueue.Dequeue();
                DoShow(window);
            }
        }

        private void DoShow(IWindowController screen,IWindowProperties properties)
        {
            DoShow(new WindowHistoryEntry(screen,properties));

        }

        private void DoShow(WindowHistoryEntry windowEntry)
        {
            if (CurrentWindow == windowEntry.Screen)
            {
                Debug.LogWarning(
                   string.Format(
                       "[WindowUILayer] The requested WindowId ({0}) is already open! This will add a duplicate to the " +
                       "history and might cause inconsistent behaviour. It is recommended that if you need to open the same" +
                       "screen multiple times (eg: when implementing a warning message pop-up), it closes itself upon the player input" +
                       "that triggers the continuation of the flow."
                       , CurrentWindow.ScreenId));
            }
            else if (CurrentWindow !=null && CurrentWindow.HideOnForegroundLost && !windowEntry.Screen.IsPopup)//Z：在当前显示的window不为空 且 当前显示的windwow设置为HideOnForegroundLost=true(当不是在最前方时，就隐藏) 且 要显示的window没有设置为 PopUp黑色背景显示
            {
                CurrentWindow.Hide();
                Debug.Log("测试 WindowUILayer.DoShow已经执行else if (CurrentWindow !=null && CurrentWindow.HideOnForegroundLost && !windowEntry.Screen.IsPopup)");
            }
            //Z：  将要显示的window 压入历史栈，放入正在动画的集合中
            windowHistory.Push(windowEntry);
            AddTransition(windowEntry.Screen);

            if (windowEntry.Screen.IsPopup)
            {
                priorityParaLayer.DarkenBG();
            }

            windowEntry.Show();

            CurrentWindow = windowEntry.Screen;

        }
        private void OnInAnimationFinished(IScreenController screen)
        {
            RemoveTransition(screen);
        }

        private void OnOutAnimationFinished(IScreenController screen)
        {
            RemoveTransition(screen);
            var window = screen as IWindowController;
            if (window.IsPopup)
            {
                priorityParaLayer.RefreshDarken();
            }
        }

        private void OnCloseRequestedByWindow(IScreenController screen)
        {
            HideScreen(screen as IWindowController);
        }

        private void AddTransition(IScreenController screen)
        {
            screensTransitioning.Add(screen);
            if (RequestScreenBlock != null)
            {
                RequestScreenBlock();
            }
        }

        private void RemoveTransition(IScreenController screen)
        {
            screensTransitioning.Remove(screen);
            if (!IsScreenTransitionInProgress)
            {
                if (RequestScreenUnblock!=null)
                {
                    RequestScreenUnblock();
                }
            }

        }

    }
}

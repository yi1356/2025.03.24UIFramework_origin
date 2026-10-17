using Assets.UIFramework.ViewAnimation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.UIFramework.Core
{
    /// <summary>
    /// UI界面的基类，窗口，面板这些都继承它，如WindowController、PanelController
    /// 主要是实现动画方式打开/关闭UI，并执行回调
    /// </summary>
    /// <typeparam name="TProps">这个是参数类</typeparam>
    public abstract class UIScreenController<TProps> : MonoBehaviour,IScreenController  
        where TProps:IScreenProperties
    {
        [Header("Screen Animations")]
        [Tooltip("界面显示的动画")]
        [SerializeField]
        private AniComponent animIn;
        [Tooltip("界面隐藏的动画")]
        [SerializeField]
        private AniComponent animOut;

        [Header("Screen properties")]
        [Tooltip("界面的属性参数(这个比较重要)")]
        [SerializeField]
        private TProps properties;

        //[Header("Screen properties")]
        

        /// <summary>
        /// 界面Id,用字符串的形式保存
        /// </summary>
        public string ScreenId { get; set; }

        /// <summary>
        /// 动画组件，为了界面有统一的弹出效果
        /// </summary>
        public AniComponent AnimIn
        {
            get { return animIn; }
            set { animIn = value; }
        }

        /// <summary>
        /// 动画组件，为了界面有统一的隐藏效果
        /// </summary>
        public AniComponent AnimOut
        {
            get { return animOut; }
            set { animOut = value; }
        }

        /// <summary>
        /// 弹出（渐入）动画完成之后回调
        /// </summary>
        public Action<IScreenController> InTransitionFinished { set; get; }

        /// <summary>
        /// 关闭（渐隐）动画完成之后回调
        /// </summary>
        public Action<IScreenController> OutTransitionFinished { get; set; }

        /// <summary>
        /// 关闭界面的回调
        /// </summary>
        public Action<IScreenController> CloseRequest { get; set; }

        /// <summary>
        /// 界面销毁的回调
        /// </summary>
        public Action<IScreenController> ScreenDestroyed { get; set; }

        /// <summary>
        /// 界面是否  是显示中
        /// </summary>
        public bool IsVisible { get; private set; }   

        /// <summary>
        /// 界面的属性参数（封装成了类的形式了）
        /// </summary>
        protected TProps Properties
        {
            get { return properties; }
            set { properties = value; }
        }

        protected virtual void Awake()
        {
            AddListeners();
        }

        protected virtual void OnDestroy()
        {
            if(ScreenDestroyed != null)
            {
                ScreenDestroyed(this);
            }

            InTransitionFinished = null;
            OutTransitionFinished = null;
            CloseRequest = null;
            ScreenDestroyed = null;
            RemoveListeners();
        }

        /// <summary>
        /// 添加监听事件，Awake会自动调用
        /// </summary>
        protected virtual void AddListeners()
        {

        }

        /// <summary>
        /// 移除监听事件，Destroy会自动调
        /// </summary>
        protected virtual void RemoveListeners()
        {
        }

        /// <summary>
        /// 属性参数设置到界面时触发，在SetProperties之后触发，比较安全的能取到值
        /// </summary>
        protected virtual void OnPropertiesSet()
        {

        }

        /// <summary>
        /// 界面隐藏的时候触发，便于处理一些操作
        /// </summary>
        protected virtual void WhileHiding()
        {
        }

        /// <summary>
        /// 设置属性参数
        /// </summary>
        /// <param name="props"></param>
        protected virtual void SetProperties(TProps props)
        {
            properties = props;
        }

        /// <summary>
        /// 在显示的时候处理一些层级，或者属性处理等，具体看继承者重写了
        /// </summary>
        protected virtual void HierarchyFixOnShow()
        {

        }

        /// <summary>
        /// 隐藏界面（参数决定是否使用动画）
        /// </summary>
        /// <param name="animate"></param>
        public void Hide(bool animate=true)
        {
            DoAnimation(animate? animOut:null,OntransitionOutFinished,false);
            WhileHiding();
        }
        /// <summary>
        /// Z：显示界面(默认是启用动画的，且可以传入界面参数)
        /// </summary>
        /// <param name="props"></param>
        public void Show(IScreenProperties props=null)
        {
            if (props!=null)
            {
                if(props is TProps)
                {
                    SetProperties((TProps)props);
                }
                else
                {
                    Debug.LogError("Properties passed have wrong type! (" + props.GetType() + " instead of " +
                                  typeof(TProps) + ")");
                    return;
                }
            }

            HierarchyFixOnShow();
            OnPropertiesSet();

            if (!gameObject.activeSelf)
            {
                DoAnimation(animIn,OnTransitionInFinished,true);
            }
            else
            {
                if (InTransitionFinished!=null)
                {
                    InTransitionFinished(this);
                }
            }
        }

        
        //Z：执行UI动画，来显示/隐藏界面，并执行完成后的回调
        private void DoAnimation(AniComponent caller,Action callWhenFinished,bool isVisible)
        {
            if (caller == null)
            {
                gameObject.SetActive(isVisible);
                if (callWhenFinished!=null)
                {
                    callWhenFinished();
                }
            }
            else
            {
                if(isVisible && !gameObject.activeSelf)
                {
                    gameObject.SetActive(true);
                }
                caller.Animate(transform,callWhenFinished);
            }

        }

        private void OntransitionOutFinished()
        {
            IsVisible = false;
            gameObject.SetActive(false);

            if (OutTransitionFinished != null)
            {
                OutTransitionFinished(this);
            }
        }


        private void OnTransitionInFinished()
        {
            IsVisible = true;

            if (InTransitionFinished != null)
            {
                InTransitionFinished(this);
            }
        }





    }
}

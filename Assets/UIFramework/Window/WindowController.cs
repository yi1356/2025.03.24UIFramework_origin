using Assets.UIFramework.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.UIFramework.Window
{

    /// <summary>
    /// 窗口管理类
    /// </summary>
    public abstract class WindowController : AWindowController<WindowProperties> { }
    public abstract class AWindowController<TProps> : UIScreenController<TProps>, IWindowController where TProps : IWindowProperties
    {
        [Tooltip("当其他窗口被置前时，自己是否隐藏")]
        public bool HideOnForegroundLost { get { return Properties.HidOnForegroundLost; } }
        [Tooltip("是否显示在最上面且有一层黑色背景")]
        public bool IsPopup { get { return Properties.IsPopup; } }

        public WindowPriority windowPriority { get { return Properties.WindowQueuePriority; } }

        /// <summary>
        /// 关闭窗口，使用UI_前缀来方便找到对应的方法
        /// </summary>
        public virtual void UI_Close()
        {
            CloseRequest(this);
        }

        protected sealed override void SetProperties(TProps props)
        {
            if (props != null)
            {
                if (!props.SuppressPrefabProperties)
                {
                    props.HidOnForegroundLost = Properties.HidOnForegroundLost;
                    props.WindowQueuePriority = Properties.WindowQueuePriority;
                    props.IsPopup = Properties.IsPopup;
                }

                Properties = props;
            }
        }

        protected override void HierarchyFixOnShow()
        {
            transform.SetAsLastSibling();//设置为其父对象的最后一个子对象
        }
    }
}

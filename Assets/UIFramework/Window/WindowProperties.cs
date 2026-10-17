using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.UIFramework.Window
{
    /// <summary>
    /// 窗口的通用属性
    /// </summary>
    [System.Serializable]
    public class WindowProperties : IWindowProperties
    {
        [Tooltip("当其他窗口被置前时，自己是否隐藏")]
        [SerializeField] protected bool hideOnForegroundLost = true;
        [SerializeField] protected WindowPriority windowQueuePriority = WindowPriority.ForceForeGround;
        [SerializeField] protected bool isPopup = false;

        public WindowProperties()
        {
            hideOnForegroundLost = true;
            windowQueuePriority = WindowPriority.ForceForeGround;
            isPopup = false;
        }

        /// <summary>
        /// 如果另一个窗口已经打开，此窗口如何表现呢？ //??????????????????????????????
        /// </summary>
        /// ForceForeGround会立刻打开；Enqueue会将它排队，以便当前窗口关闭后打开
        public WindowPriority WindowQueuePriority
        {
            get { return windowQueuePriority; }
            set { windowQueuePriority = value; }
        }

        /// <summary>
        /// 当其他窗口被置前时，自己是否隐藏
        /// </summary>
        public bool HidOnForegroundLost 
        { get { return hideOnForegroundLost; }
            set { hideOnForegroundLost = value; }
        }

        /// <summary>
        /// 当在open()调用中传递属性时，是否应该覆盖在viewPrefab中配置的属性
        /// </summary>
        public bool SuppressPrefabProperties { get; set; }

        /// <summary>
        /// 弹出窗口在它们后面显示一个黑色背景，并在所有其他窗口的前面显示
        /// </summary>
        public bool IsPopup 
        {
            get { return isPopup; }
            set { isPopup = value; }
        }

        public WindowProperties(bool suppressPrefabProperties)
        {
            WindowQueuePriority = WindowPriority.ForceForeGround;
            HidOnForegroundLost = false;
            this.SuppressPrefabProperties = SuppressPrefabProperties;
        }

        public WindowProperties(WindowPriority windowQueuePriority, bool hideOnForegroundLost,bool suppressPrefabProperties)
        {
            WindowQueuePriority = windowQueuePriority;
            HidOnForegroundLost = hideOnForegroundLost;
            this.SuppressPrefabProperties = suppressPrefabProperties;
        }


    }
}

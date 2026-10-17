using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.UIFramework.ViewAnimation
{
    /// <summary>
    /// 界面动画组件基类
    /// </summary>
    public abstract class AniComponent : MonoBehaviour
    {
        /// <summary>
        /// 外部调用执行动画的方法，当动画执行完调用CallWhenFinished
        /// </summary>
        /// <param name="target"></param>
        /// <param name="callWhenFinished"></param>
        public abstract void Animate(Transform target, Action callWhenFinished);

    }
}

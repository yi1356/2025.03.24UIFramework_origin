using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.UIFramework.Window
{
    /// <summary>
    /// 枚举类型，用于定义窗口在打开时，在历史记录和队列中的行为
    /// </summary>
    public enum WindowPriority
    {
        ForceForeGround=0,
        Enqueue=1,
    }
}

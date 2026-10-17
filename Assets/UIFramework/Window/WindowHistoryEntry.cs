using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.UIFramework.Window
{
    /// <summary>
    /// 存储窗口和 它的属性      窗口记录 和 队列
    /// </summary>
    public struct WindowHistoryEntry
    {
        public readonly IWindowController Screen;
        public readonly IWindowProperties Properties;

        public WindowHistoryEntry(IWindowController screen, IWindowProperties properties)
        {
            this.Screen = screen;
            this.Properties = properties;

        }

        public void Show()
        {
            Screen.Show(Properties);
        }
    }
}

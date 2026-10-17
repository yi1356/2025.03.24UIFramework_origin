using Assets.UIFramework.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace Assets.UIFrameworkExamples.Scripes.ScreenController
{
    public class StartDemoSignal:ASignal
    {
    }

    public class StartWindowController : WindowController
    {
        //Z：外部的按钮会拖入这个
        public void UI_Start()
        {
            Signals.Get<StartDemoSignal>().Dispatch();
        }

    }
}

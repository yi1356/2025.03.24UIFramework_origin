using Assets.UIFramework.Panel;
using Assets.UIFramework.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Utils;

namespace Assets.UIFrameworkExamples.Scripes.ScreenController
{
    /// <summary>
    /// Z：其中是导航中单个按钮的信息
    /// </summary>
    [Serializable]
    public class NavigationPanelEntry
    {
        [SerializeField] private Sprite sprite = null;
        [SerializeField] private string buttonText = "" ;
        [SerializeField] private string targetScreen= "" ;

        public Sprite Sprite
        {
            get { return sprite; }
        }

        public string ButtonText
        {
            get { return buttonText; }
        }

        public string TargetScreen
        {
            get { return targetScreen; }
        }
    }

    public class NavigateToWindowSignal : ASignal<string>
    {

    }

    //Z：作用：导航面板
    //Z：思路：根据配置生成按钮、为按钮添加监听方法 
    public class NavigationPanelController: PanelController
    {
        [SerializeField]
        private List<NavigationPanelEntry> navigationTargets = new List<NavigationPanelEntry>();//Z：Inspector面板中要自己设置的配置信息
        [SerializeField]
        private NavigationPanelButton templateButton = null;//Z：这个是按钮模板
        private readonly List<NavigationPanelButton> currentButtons = new List<NavigationPanelButton>();//Z：这个是要在程序中根据配置信息实例生成的  导航按钮


        //一般来说AddListenner和RemoveListeners都是成对出现的，add完别忘记remove
        protected override void AddListeners()
        {
            Signals.Get<NavigateToWindowSignal>().AddListener(OnExternalNavigation);//注意这个其实有两个Add......，这个只是在处理button按钮的interactive问题
        }

        protected override void RemoveListeners()
        {
            Signals.Get<NavigateToWindowSignal>().RemoveListener(OnExternalNavigation);
        }


        /// <summary>
        /// 当界面打开时，这个函数被调用； Z：根据配置信息生成 导航的按钮
        /// </summary>
        protected override void OnPropertiesSet()
        {
            ClearEntries();
            foreach (var target  in navigationTargets)
            {
                var newBtn = Instantiate(templateButton);
                newBtn.transform.SetParent(templateButton.transform.parent,false);
                newBtn.SetData(target);
                newBtn.gameObject.SetActive(true);
                newBtn.ButtonClicked += OnNavigationButtonClicked;
                currentButtons.Add(newBtn);
            }
            //默认选中第一个按钮
            OnNavigationButtonClicked(currentButtons[0]);
        }


     //Z：点击导航按钮后的执行方法
        private void OnNavigationButtonClicked(NavigationPanelButton currentlyClickedButton)
        {
            Signals.Get<NavigateToWindowSignal>().Dispatch(currentlyClickedButton.Target);//Z：利用单参数 Signals结合 传入的(string)参数，决定了打开的界面
            foreach (var button in currentButtons)
            {
                button.SetCurrentNavigationTarget(currentlyClickedButton);
            }
            
        }

        private void OnExternalNavigation(string screenId)
        {
            foreach (var button in currentButtons)
            {
                button.SetCurrentNavigationTarget(screenId);
            }
        }

        private void ClearEntries()
        {
            foreach (var button in currentButtons)
            {
                button.ButtonClicked -= OnNavigationButtonClicked;
                Destroy(button.gameObject);
            }
        }




    }
}

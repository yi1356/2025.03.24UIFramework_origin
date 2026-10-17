using Assets.UIFrameworkExamples.Scripes.ScreenController;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.UIFramework.Window
{
    //Z：导航面板的 按钮
    [RequireComponent(typeof(Button))]
    public class NavigationPanelButton:MonoBehaviour
    {
        [SerializeField]
        private Text buttonLabel = null;
        [SerializeField]
        private Image icon = null;


        public event Action<NavigationPanelButton> ButtonClicked;//Z：会在NavigationPanelController中 添加监听方法

        private NavigationPanelEntry navigationData = null;
        private Button _button = null; 
        private Button button
        {
            get
            {
                if (_button == null)
                {
                    _button = GetComponent<Button>();
                }
                return _button;
            }
        }

        public string Target
        {
            get { return navigationData.TargetScreen; }
        }

        //Z： 设置导航按钮的信息
        public void SetData(NavigationPanelEntry target)
        {
            navigationData = target;
            buttonLabel.text = target.ButtonText;
            icon.sprite = target.Sprite;
        }

        //Z：作用是让点击的导航按钮失活，其余激活（应该是为了避免重复点击）
        public void SetCurrentNavigationTarget(NavigationPanelButton selectedButton)
        {
            button.interactable = selectedButton != this;
        }

        //Z：似乎和别的界面显示时，按钮的失活/激活  有关
        public void SetCurrentNavigationTarget(string screenId)
        {
            if (navigationData != null)
            {
                button.interactable = navigationData.TargetScreen == screenId;
            }
        }

        public void UI_Click()
        {
            if (ButtonClicked!=null)
            {
                ButtonClicked(this);
            }
        }


    }
}

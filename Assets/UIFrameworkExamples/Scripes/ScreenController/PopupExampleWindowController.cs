using Assets.UIFramework.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace Assets.UIFrameworkExamples.Scripes.ScreenController
{

    //Z：作用：点击点击按钮会 **出现另外弹窗**，来实现变色功能
    public class PopupExampleWindowController : WindowController
    {
        [SerializeField]
        private Image exampleImage = null;

        private int currentPopupExample;
        private Color originalColor;


        protected override void Awake()
        {
            base.Awake();
            originalColor = exampleImage.color;
        }

        //Z：外面按钮会拖这个
        public void UI_ShowPopUp()
        {
            Signals.Get<ShowConfirmationPopupSignal>().Dispatch(GetPopupData());
        }
        //返回一个ConfirmationPopupProperties，用在在ConfirmationPopupController打开时设置一些内容
        private ConfirmationPopupProperties GetPopupData()
        {
            ConfirmationPopupProperties testProps = null;

            switch (currentPopupExample)
            {
                case 0:
                    testProps = new ConfirmationPopupProperties("标题测试",
                        "点下按钮试试",
                        "明白");
                    break;
                case 1:
                    testProps = new ConfirmationPopupProperties("小测试",
                        "随便选个颜色",
                        "蓝色", OnBlueSelected,
                        "红色", OnRedSelected);
                    break;
                case 2:
                    testProps = new ConfirmationPopupProperties("恢复颜色",
                        "恢复成原来的颜色吧",
                        "好的", OnRevertColors);
                    break;
                case 3:
                    testProps = new ConfirmationPopupProperties("重置",
                        "随便测一下", "重置");
                    break;
            }

            currentPopupExample++;
            if (currentPopupExample > 3)
            {
                currentPopupExample = 0;
            }

            return testProps;
        }

        private void OnRedSelected()
        {
            exampleImage.color = Color.red;
        }

        private void OnBlueSelected()
        {
            exampleImage.color = Color.blue;
        }

        private void OnRevertColors()
        {
            exampleImage.color = originalColor;
        }
    }
}

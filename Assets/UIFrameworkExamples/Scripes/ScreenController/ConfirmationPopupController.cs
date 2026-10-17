using Assets.UIFramework.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.UIFrameworkExamples.Scripes.ScreenController
{
    public class ShowConfirmationPopupSignal : Utils.ASignal<ConfirmationPopupProperties> { }

    //Z：作用：通用确认弹窗，另外的小弹窗，有两个按钮来实现功能
    [Serializable]
    public class ConfirmationPopupController : AWindowController<ConfirmationPopupProperties>
    {
        [SerializeField] public Text titleLabel;
        [SerializeField] public Text messageLabel;
        [SerializeField] public Text confirmButtonLabel;
        [SerializeField] public Text cancelButtonLabel;
        [SerializeField] public GameObject cancelButtonObject;

        protected override void OnPropertiesSet()
        {
            titleLabel.text = Properties.Title;
            messageLabel.text = Properties.Message;
            confirmButtonLabel.text = Properties.ConfirmButtonText;
            cancelButtonLabel.text = Properties.CancelButtonText;

            cancelButtonObject.SetActive(Properties.CancelAction!=null);


        }

        //Z：点击红色按钮
        public void UI_Confirm()
        {
            UI_Close();
            if (Properties.ConfirmAction != null)
            {
                Properties.ConfirmAction();
            }
        }

        //Z：点击蓝色按钮
        public void UI_Cancel()
        {
            UI_Close();
            if (Properties.CancelAction != null)
            {
                Properties.CancelAction();
            }

        }
    }

    //ConfirmationPopupController的特有属性
    [Serializable]
    public class ConfirmationPopupProperties : WindowProperties
    {
        public readonly string Title;
        public readonly string Message;
        public readonly string ConfirmButtonText;
        public readonly string CancelButtonText;
        public readonly Action CancelAction;
        public readonly Action ConfirmAction;

        //如果子类使用有参（无参）构造函数，并且没有显式调用父类的构造函数，那么依然会默认调用父类的无参构造函数
        public ConfirmationPopupProperties(string title, string message,
            string confirmButtonText = "Confirm", Action confirmAction = null,
            string cancelButtonText = "Cancel", Action cancelAction = null)
        {
            Title = title;
            Message = message;
            ConfirmButtonText = confirmButtonText;
            CancelButtonText = cancelButtonText;
            CancelAction = cancelAction;
            ConfirmAction = confirmAction;
        }

       
    }
}

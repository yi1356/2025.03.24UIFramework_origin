using Assets.UIFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.UIFramework
{
    /// <summary>
    ///Z：ScripeObject 配置要注册的界面并注册,真正实现创建UIFrame物体
    /// </summary>
    [CreateAssetMenu(fileName="UISettings",menuName="UI/UISettings")]
    public class UIsettings : ScriptableObject
    {
        [Tooltip("UI Frame的预制体")]
        [SerializeField] private UIFrame templateUIPrafab = null;
        [Tooltip("界面的预制体(包括面板和窗口)")]
        [SerializeField] private List<GameObject> screenToRegister = null;
        [Tooltip("实例化时是否停用，也就是开始时界面注册后在Scence中是否为失活")]
        [SerializeField] private bool deactivateScreenGOs = true;

        /// <summary>
        /// 创建一个UI Frame对象，同时实例化并注册好所有界面（Panel+窗口）
        /// </summary>
        /// <param name="instanceAndRegisterScreens"></param>
        /// <returns></returns>
        public UIFrame CreateUIInstance(bool instanceAndRegisterScreens = true)
        {
            var newUI = Instantiate(templateUIPrafab);

            if (instanceAndRegisterScreens)
            {
                foreach (var screen in screenToRegister)
                {
                    var screenInstance = Instantiate(screen);
                    var screenController = screenInstance.GetComponent<IScreenController>();

                    if (screenController != null)
                    {
                        newUI.RegisterScreen(screen.name, screenController, screenInstance.transform);//Z：注册名是screen.name
                        if (deactivateScreenGOs && screenInstance.activeSelf)
                        {
                            screenInstance.SetActive(false);
                        }

                    }
                    else
                    {
                        Debug.LogError("[UIConfig] Screen doesn't contain a ScreenController! Skipping " + screen.name);
                    }
                }
            }


            return newUI;
        }


        //Z：Unity 编辑器的特殊回调方法，当「脚本资产被修改」或「Inspector 面板中字段被编辑」时自动调用（仅在编辑器模式下执行，运行时不触发）
        private void OnValidate()
        {
            List<GameObject> objectsToRemove = new List<GameObject>();//为了方便移除 ，非法界面(没有挂载继承了IScreenController的脚本的)
            for (int i = 0; i < screenToRegister.Count;i++)
            {
                var screenCtr = screenToRegister[i].GetComponent<IScreenController>();
                if (screenCtr == null)
                {
                    objectsToRemove.Add(screenToRegister[i]);
                }

            }

            if (objectsToRemove.Count>0)
            {
                Debug.LogError("[UISettings] Some GameObjects that were added to the Screen Prefab List didn't have ScreenControllers attached to them! Removing.");
                foreach (var obj in objectsToRemove)
                {
                    Debug.LogError("[UISettings] Removed " + obj.name + " from " + name + " as it has no Screen Controller attached!");
                    screenToRegister.Remove(obj);
                }
            }
        }
    }
}

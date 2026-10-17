using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.UIFramework.Window
{
    /// <summary>
    /// 这是一个“辅助层级,以便显示优先级更高的窗口（也就是最前方的window，且要有黑色半透明背景）。
    /// 默认情况下，它包含任何标记为弹出窗口的窗口。它有WindowUILyer控制”
    /// </summary>
    public class WindowParaLayer:MonoBehaviour
    {
        [SerializeField]
        private GameObject darkenBgObject = null;
        private List<GameObject> containedScreens = new List<GameObject>();

        public void AddScreen(Transform screenRectTransform)
        {
            screenRectTransform.SetParent(transform,false);
            containedScreens.Add(screenRectTransform.gameObject);
        }

        /// <summary>
        /// Z：更新黑色背景(当containedScreens中的界面中有为true的则令darkenBgObject激活)
        /// </summary>
        public void RefreshDarken()
        {
            for (int i=0;i<containedScreens.Count;i++)
            {
                if (containedScreens[i]!=null)
                {
                    if (containedScreens[i].activeSelf)
                    {
                        darkenBgObject.SetActive(true);
                        return;
                    }
                }
            }
            darkenBgObject.SetActive(false);
        }

        public void DarkenBG()
        {
            darkenBgObject.SetActive(true);
            darkenBgObject.transform.SetAsLastSibling();
        }
    }
}

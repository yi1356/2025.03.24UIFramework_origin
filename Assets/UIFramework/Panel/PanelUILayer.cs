using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.UIFramework.Panel
{
    /// <summary>
    /// 这个Layer层是控制面板的(能让其显示在不同节点下)
    /// 面板是界面的一种，没有历史记录，没有队列
    /// 就是简单的显示在界面中
    /// 比如说体力槽，小地图这种常驻的
    /// </summary>
    public class PanelUILayer : UILayer<IPanelController>
    {
        [SerializeField]
        [Tooltip("优先级并行层的设置。注册到此层的面板将根据其优先级重新归属到不同的并行层对象")]
        private PanelPriorityLayerList priorityLayers = null;

        public override void HideScreen(IPanelController screen)
        {
            screen.Hide();
        }

        public bool IsPanelVisible(string panelId)
        {
            IPanelController panel;
            if(registeredScreen.TryGetValue(panelId,out panel))
            {
                return panel.IsVisible;
            }
            return false;
        }

        public override void ReparentScreen(IScreenController controller, Transform screeTransform)
        {
            var ctl = controller as IPanelController;
            if (ctl != null)
            {
                ReparentToParaLayer(ctl.priority, screeTransform);
            }
        }

        public override void ShowScreen(IPanelController screen)
        {
            screen.Show();
        }

        public override void ShowScreen<TProps>(IPanelController screen, TProps properties)
        {
            screen.Show(properties);
        }

        private void ReparentToParaLayer(PanelPriority priority, Transform screeTransform)
        {
            Transform trans;
            if(!priorityLayers.ParaLayerLookUp.TryGetValue(priority,out trans))
            {
                trans = this.transform;
            }
            
            screeTransform.SetParent(trans, false) ;

        }
    }
}

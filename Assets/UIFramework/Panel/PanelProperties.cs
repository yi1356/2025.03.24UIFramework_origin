using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.UIFramework.Panel
{
    [Serializable]
    public class PanelProperties : IPanelProperties
    {
        [SerializeField]
        [Tooltip("面板根据其优先级进入不同的副层级，可以在面板层级中，设置副层级")]
        private PanelPriority priority;
        public PanelPriority Priority 
        {
            get { return priority; }
            set { priority = value; }
        }
    }
}

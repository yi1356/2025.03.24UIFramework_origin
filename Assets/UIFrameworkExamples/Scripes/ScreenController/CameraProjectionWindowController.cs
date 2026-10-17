using Assets.UIFramework.Window;
using Assets.UIFrameworkExamples.Scripes.Widgets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.UIFrameworkExamples.Scripes.ScreenController
{
    [Serializable]
    public class CameraProjectionWindowProperties:WindowProperties
    {
        public readonly Camera WorldCamera;
        public readonly Transform TransformToFollow;

        public CameraProjectionWindowProperties(Camera worldCamera, Transform toFollow)
        {
            WorldCamera = worldCamera;
            TransformToFollow = toFollow;
        }
    }
    public class CameraProjectionWindowController:AWindowController<CameraProjectionWindowProperties>
    {
        [SerializeField]
        private UIFollwComponent followTemplate = null;

        private List<UIFollwComponent> allElements = new List<UIFollwComponent>();

        protected override void OnPropertiesSet()
        {
            CreateNewLabel(Properties.TransformToFollow,"看这里",null);
            Properties.TransformToFollow.parent.gameObject.SetActive(true);
        }

        protected override void WhileHiding()
        {
            foreach (var element in allElements)
            {
                Destroy(element.gameObject);
            }
            allElements.Clear();
            Properties.TransformToFollow.parent.gameObject.SetActive(false);
        }

        private void LateUpdate()
        {
            for (int i=0;i<allElements.Count;i++)
            {
                allElements[i].UpdatePosition(Properties.WorldCamera);
            }
        }

        private void CreateNewLabel(Transform target, string label, Sprite icon)
        {
            var followConponent = Instantiate(followTemplate,followTemplate.transform.parent,false);
            followConponent.labelDestroyed += OnLabelDestroyed;
            followConponent.gameObject.SetActive(true);
            followConponent.SetFollow(target);
            followConponent.SetText(label);

            if (icon != null)
            {
                followConponent.SetIcon(icon);
            }

            allElements.Add(followConponent);
            
        }

        private void OnLabelDestroyed(UIFollwComponent destroyedLabel)
        {
            allElements.Remove(destroyedLabel);
        }
    }

   
}

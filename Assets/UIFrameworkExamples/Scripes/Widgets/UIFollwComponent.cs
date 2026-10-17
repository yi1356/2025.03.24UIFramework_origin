using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.UIFrameworkExamples.Scripes.Widgets
{
    /// <summary>
    /// UI跟随组件，注意：仅在模板UI元素锚定到左下角时才有效
    /// </summary>
    public class UIFollwComponent:MonoBehaviour
    {
        [SerializeField] private Text label = null;
        [SerializeField] private Image icon = null;
        [SerializeField] private bool clampAtBorders = true;
        [SerializeField] private bool rotateWhenClamped = true;
        [SerializeField] private RectTransform rotatingElement = null;


        public event Action<UIFollwComponent> labelDestroyed;
        private Transform currentFollow;
        private RectTransform rectTransform;
        private CanvasScaler parentScaler;
        private RectTransform mainCanvasRectTransfoem;

        public  static  float ScaleFactor;

        public void SetFollow(Transform toFollow)
        {
            currentFollow = toFollow;
        }

        public void SetText(string label)
        {
            this.label.text = label;
        }

        public void SetIcon(Sprite icon)
        {
            this.icon.sprite = icon;
        }

        private void Start()
        {
            mainCanvasRectTransfoem = transform.root as RectTransform;//总节点
            rectTransform = transform as RectTransform;
            parentScaler = mainCanvasRectTransfoem.GetComponent<CanvasScaler>();

            if (rotatingElement==null)
            {
                rotatingElement = rectTransform;
            }

            ScaleFactor =Screen.width / parentScaler.referenceResolution.x * (1 - parentScaler.matchWidthOrHeight) + Screen.height / parentScaler.referenceResolution.y * parentScaler.matchWidthOrHeight;
        }

        internal void UpdatePosition(Camera cam)//********************************
        {
            if(currentFollow != null)
            {
                var onScreenPosition = GetAnchoredPosition(cam,currentFollow.transform,parentScaler,rectTransform.rect);
                if (!clampAtBorders)
                {
                    rectTransform.anchoredPosition = onScreenPosition;
                    return;
                }

                var clampedPosition = GetClampedOnScreenPosition(onScreenPosition,rectTransform.rect,mainCanvasRectTransfoem);
                rectTransform.anchoredPosition = clampedPosition;

                if (!rotateWhenClamped)
                {
                    return;
                }

                if (onScreenPosition != clampedPosition)
                {
                    var delta = clampedPosition - onScreenPosition;
                    var angle = Mathf.Atan2(delta.y,delta.x) * Mathf.Rad2Deg;//
                    rotatingElement.localRotation = Quaternion.AngleAxis(angle-98f,Vector3.forward);//
                }
                else
                {
                    rotatingElement.localRotation = Quaternion.identity;
                }
            }
            
        }

        /// <summary>
        /// 在ScreenSpace-Camera 下的UI, 给定WorldSpace 变换计算锚定位置
        /// </summary>
        /// <param name="viewingCamera">世界空间相机(用于渲染场景的相机)</param>
        /// <param name="followTransfoem">要跟随的物体</param>
        /// <param name="canvasScaler">画布缩放器</param>
        /// <param name="rect">将跟随变换的UI元素的矩形区域</param>
        /// <returns></returns>
        public static Vector2 GetAnchoredPosition(Camera viewingCamera, Transform followTransfoem, CanvasScaler canvasScaler, Rect followElementRect)
        {
            //坐标转换
            var relaivePosition = viewingCamera.transform.InverseTransformPoint(followTransfoem.position);// 可将世界坐标系中的一个点(位置)转换为特定 Transform 对象的局部坐标系中的点，这里指将followPosition转化为相对与viewingCamera的位置
            relaivePosition.z = Mathf.Max(relaivePosition.z,1f);//防止 z坐标值过小甚至为负数，这可能会引发后续计算出现异常
            var viewportPos = viewingCamera.WorldToScreenPoint(viewingCamera.transform.TransformPoint(relaivePosition)); //将世界坐标位置转换为屏幕坐标位置

            //TODU:????????????????????计算出最终的锚点位置(因为采取了ScreenSpace_Camera的方式，所以要设置AnchoredPosition)这里非常不理解
            //return new Vector2(viewportPos.x+canvasScaler.referenceResolution.x-followElementRect.size.x/2f,
            //    viewportPos.y+canvasScaler.referenceResolution.y-followElementRect.size.y - followElementRect.size.y/2f );

            return new Vector2(viewportPos.x / ScaleFactor - followElementRect.size.x / 2f,
                viewportPos.y/ ScaleFactor - followElementRect.size.y - followElementRect.size.y / 2f);
            


        }

        /// <summary>  
        /// 限制Screenspace-Camera UI在屏幕上的位置  
        /// </summary>  
        /// <param name="onScreenPosition">UI元素的当前屏幕位置</param>  
        /// <param name="followElementRect">跟随世界空间对象的矩形区域</param>  
        /// <param name="mainCanvasRectTransform">此UI主画布的RectTransform</param>  
        /// <returns></returns>
        public static Vector2 GetClampedOnScreenPosition(Vector2 onScreenPosition, Rect followElementRect, RectTransform mainCanvasRectTransform)
        {
            return new Vector2(Mathf.Clamp(onScreenPosition.x, 0f, mainCanvasRectTransform.sizeDelta.x - followElementRect.size.x),
                               Mathf.Clamp(onScreenPosition.y, 0f, mainCanvasRectTransform.sizeDelta.y - followElementRect.size.y));
        }


        private void OnDestroy()
        {
            if (labelDestroyed!=null)
            {
                labelDestroyed(this);
            }
        }


    }
}

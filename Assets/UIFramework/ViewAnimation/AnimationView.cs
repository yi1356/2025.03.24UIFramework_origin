using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.UIFramework.ViewAnimation
{
    /// <summary>
    /// 具体的动画实现，这里是封装的AnimationClip,自己封装DOTwwen之类的都行，这个随意
    /// </summary>
    public class AnimationView:AniComponent
    {
        [SerializeField] private AnimationClip clip = null;
        [SerializeField] private bool playReverse = false;//Z：控制动画播放方向（正播/倒播） 

        private Action previouseCallbackWhenFinished;  //Z：缓存 “上一次动画结束时的回调函数”，避免多次动画叠加时回调丢失，确保上一次动画的回调能正确执行？？？

        public override void Animate(Transform target, Action callWhenFinished)
        {
            FinishPrevious();
            var targetAnimation = target.GetComponent<Animation>();
            if (targetAnimation==null)
            {
                Debug.LogError("[LegacyAnimationScreenTransition] No Animation component in " + target);
                if (callWhenFinished!=null)
                {
                    callWhenFinished();
                }
                return;
            }

            targetAnimation.clip = clip;
            StartCoroutine(PlayAnimationRoutine(targetAnimation,callWhenFinished));
        }

        
        private IEnumerator  PlayAnimationRoutine(Animation targetAnimation, Action callWhenFinished)
        {
            previouseCallbackWhenFinished = callWhenFinished;
            //借助 AnimationState，能够对单个动画剪辑的播放速度、时间、权重等参数加以控制
            foreach (AnimationState state in targetAnimation)
            {
                state.time = playReverse ? state.clip.length : 0f;
                state.speed = playReverse ? -1f : 1f;
            }

            targetAnimation.Play(PlayMode.StopAll);
            yield return new WaitForSeconds(targetAnimation.clip.length);
            FinishPrevious();
        }

        private void FinishPrevious()
        {
            if (previouseCallbackWhenFinished!=null)
            {
                previouseCallbackWhenFinished();
                previouseCallbackWhenFinished = null;
            }
            StopAllCoroutines();
        }
    }
}

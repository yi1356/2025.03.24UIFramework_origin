using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Utils;

namespace Assets.UIFrameworkExamples.Scripes.Extras
{
    public class PlayerDataUpdatedSignal : ASignal<List<PlayerDateEntry>> { }
    /// <summary>
    /// 单条的用户数据
    /// </summary>
    [Serializable]
    public class PlayerDateEntry
    {
        public string LevelName;
        [Range(0, 3)]
        public int Stars;
    }

    /// <summary>
    /// ScripeObject用户数据，存储了用户数据列表
    /// </summary>
    [CreateAssetMenu(fileName="PlayerData",menuName="UI/Fake Player Data")]
    public class FakePlayerData : ScriptableObject
    {
        [SerializeField]
        private List<PlayerDateEntry> levelProgress = null;

        public List<PlayerDateEntry> LevelProgress
        {
            get { return levelProgress; }
        }

        /// <summary>
        /// 这个方法是由Unity编辑器在MonoBehaviour 和 ScriptableObjects中调用的
        /// 每当检查器中的值发生变化时就会触发,在这里，我使用它来传播实例中的更改，
        /// 但在实践中，你可以通过向屏幕传递一个可观察变量来实现相同的行为
        /// 该变量通过其属性或其他形式的数据绑定到控制器
        /// </summary>
        private void OnValidate()
        {
            Signals.Get<PlayerDataUpdatedSignal>().Dispatch(levelProgress);
        }

    }
}

using Assets.UIFramework.Window;
using Assets.UIFrameworkExamples.Scripes.Extras;
using Assets.UIFrameworkExamples.Scripes.Widgets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Utils;

namespace Assets.UIFrameworkExamples.Scripes.ScreenController
{
    /// <summary>
    /// playerWindow属性类
    /// </summary>
    [Serializable]
    public class PlayerWindowProperties : WindowProperties
    {
        public readonly List<PlayerDateEntry> PlayerData;
        public PlayerWindowProperties(List<PlayerDateEntry> data)
        {
            PlayerData = data;
        }

    }
    public class PlayerWindowController: AWindowController<PlayerWindowProperties>
    {
        [SerializeField]
        private LevelProgressComponent templateLevelEntry = null;

        private List<LevelProgressComponent> currentLevels = new List<LevelProgressComponent>();

        protected override void AddListeners()
        {
            Signals.Get<PlayerDataUpdatedSignal>().AddListener(OnDataUpdated);
        }

        protected override void RemoveListeners()
        {
            Signals.Get<PlayerDataUpdatedSignal>().RemoveListener(OnDataUpdated);
        }

        protected override void OnPropertiesSet()
        {
            OnDataUpdated(Properties.PlayerData);
        }

        private void OnDataUpdated(List<PlayerDateEntry> data)
        {
            VerifyElementCount(data.Count);
            RefreshElementData(data);
        }

       
        private void VerifyElementCount(int levelCount)
        {
            if (currentLevels.Count == levelCount)
            {
                return;
            }

            if (currentLevels.Count < levelCount)
            {
                while (currentLevels.Count < levelCount)
                {
                    var newLevel = Instantiate(templateLevelEntry,
                        templateLevelEntry.transform.parent,
                        false);
                    newLevel.gameObject.SetActive(true);
                    currentLevels.Add(newLevel);
                }
            }
            else
            {
                while (currentLevels.Count > levelCount)
                {
                    var levelToRemove = currentLevels[currentLevels.Count - 1];
                    currentLevels.Remove(levelToRemove);
                    Destroy(levelToRemove.gameObject);
                }
            }
        }

        private void RefreshElementData(List<PlayerDateEntry> playerLevelProgress)
        {
            for (int i = 0; i < currentLevels.Count; i++)
            {
                currentLevels[i].SetData(playerLevelProgress[i], i);
            }
        }

    }
}

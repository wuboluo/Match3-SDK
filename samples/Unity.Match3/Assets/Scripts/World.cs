using System.Text;
using UnityEngine;
using UnityEngine.U2D;

namespace Match3
{
    public class World : MonoSingleton<World>
    {
        public int rowCount = 5;
        public int columnCount = 7;
        public float tileSize = 0.6f;

        public SpriteAtlas artAtlas;
        public GameObject itemPrefab;
        public CanvasInputSystem inputSystem;

        public Game_RootComponent Root { get; private set; }

        protected override void OnAwake()
        {
            Root = new Game_RootComponent();
            {
                Root.AddComponent(New_Game_ItemPoolComponent());
                Root.AddComponent(New_Game_BoardComponent());
                Root.AddComponent(New_Game_SwapComponent());
                Root.AddComponent(New_Game_SolveComponent());
                Root.AddComponent(New_Game_FillComponent());
                Root.AddComponent(New_Game_PointerComponent());
                Root.AddComponent(New_Game_JobComponent());
            }
        }

        /// 道具池组件
        private Game_ItemPoolComponent New_Game_ItemPoolComponent()
        {
            return new Game_ItemPoolComponent(itemPrefab, transform);
        }

        /// 棋盘组件
        private Game_BoardComponent New_Game_BoardComponent()
        {
            return new Game_BoardComponent(rowCount, columnCount, tileSize);
        }

        /// 消除组件
        private Game_SolveComponent New_Game_SolveComponent()
        {
            return new Game_SolveComponent(GetSequenceDetectors(), GetSpecialItemDetectors());

            // 线性消除器
            ILinearDetect[] GetSequenceDetectors()
            {
                return new ILinearDetect[]
                {
                    new GameVerticalLinearDetectComponent(),
                    new GameHorizontalLinearDetectComponent()
                };
            }

            // 特殊消除器
            ISpecialDetect[] GetSpecialItemDetectors()
            {
                return null;
            }
        }

        /// 填充策略组件
        private Game_FillComponent New_Game_FillComponent()
        {
            return new Game_FillComponent();
        }

        /// 拖拽组件
        private Game_PointerComponent New_Game_PointerComponent()
        {
            return new Game_PointerComponent();
        }

        /// Job执行组件
        private Game_JobComponent New_Game_JobComponent()
        {
            return new Game_JobComponent();
        }

        /// 交换组件
        private Game_SwapComponent New_Game_SwapComponent()
        {
            return new Game_SwapComponent();
        }

        public void SolveSequenceDescription(LinearSolveSequenceData sequenceData)
        {
            var sb = new StringBuilder();
            var detectorType = sequenceData.SequenceDetectorType.Name.Contains("Ver") ? "纵向" : "横向";

            sb.Append($"棋子种类：<color=yellow>{sequenceData.SolvedSlotComponents[0].ItemComponent.ContentId}</color>");
            sb.Append("  |  ");
            sb.Append($"方向：<color=yellow>{detectorType}</color>");
            sb.Append("  |  ");
            sb.Append($"数量：<color=yellow>{sequenceData.SolvedSlotComponents.Count}</color>");

            Debug.Log(sb.ToString());
        }
    }
}
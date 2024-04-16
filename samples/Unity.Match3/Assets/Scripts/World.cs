namespace Match3
{
    public class World : MonoSingleton<World>
    {
        public GameBoardData data;
        public CanvasInputSystem inputSystem;

        public Game_RootComponent Root { get; private set; }

        protected override void OnAwake()
        {
            Root = new Game_RootComponent();
            {
                Root.AddComponent(New_Game_ItemPoolComponent());
                Root.AddComponent(New_Game_ItemGenerateComponent());
                Root.AddComponent(New_Game_BoardComponent());
                Root.AddComponent(New_Game_SolveComponent());
                Root.AddComponent(New_Game_AnimatedItemSwapperComponent());
                Root.AddComponent(New_Game_RiseFillStrategyComponent());
            }
        }

        // /// 交换位置操作逻辑
        // public UnityGame GetUnityGame()
        // {
        //     var gameConfig = new GameConfig
        //     {
        //         GameBoardData = data,
        //         ItemSwapperComponent = new Game_AnimatedItemSwapperComponent(),
        //         GameSolveComponent = new Game_SolveComponent(GetSequenceDetectors(), GetSpecialItemDetectors())
        //     };
        //
        //     return new UnityGame(inputSystem, gameConfig);
        // }

        /// 道具池组件
        private Game_ItemPoolComponent New_Game_ItemPoolComponent()
        {
            return new Game_ItemPoolComponent(data.tileModels, data.transform);
        }

        /// 道具生成组件
        private Game_ItemGenerateComponent New_Game_ItemGenerateComponent()
        {
            return new Game_ItemGenerateComponent(data.itemPrefab, data.itemsPool, data.artAtlas);
        }

        /// 棋盘组件
        private Game_BoardComponent New_Game_BoardComponent()
        {
            return new Game_BoardComponent(data.rowCount, data.columnCount, data.tileSize);
        }

        /// 消除组件
        private Game_SolveComponent New_Game_SolveComponent()
        {
            return new Game_SolveComponent(GetSequenceDetectors(), GetSpecialItemDetectors());

            // 线性消除器
            ILineDetect[] GetSequenceDetectors()
            {
                return new ILineDetect[]
                {
                    new Game_VerticalLineDetectComponent(),
                    new Game_HorizontalLineDetectComponent()
                };
            }

            // 特殊消除器
            ISpecialItemDetector[] GetSpecialItemDetectors()
            {
                return null;
            }
        }

        /// 道具交换动画组件
        private Game_AnimatedItemSwapperComponent New_Game_AnimatedItemSwapperComponent()
        {
            return new Game_AnimatedItemSwapperComponent();
        }

        /// 填充策略组件
        private Game_RiseFillStrategyComponent New_Game_RiseFillStrategyComponent()
        {
            return new Game_RiseFillStrategyComponent();
        }
    }
}
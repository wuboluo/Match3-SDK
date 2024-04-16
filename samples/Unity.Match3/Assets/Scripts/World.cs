namespace Match3
{
    public class World : MonoSingleton<World>
    {
        public GameBoardData boardData;
        public CanvasInputSystem inputSystem;

        public Game_RootComponent Root { get; private set; }

        protected override void OnAwake()
        {
            // 组件
            Root = new Game_RootComponent();

            var itemPoolComponent = Root.AddComponent(new Game_ItemPoolComponent());
            var itemGenerateComponent = Root.AddComponent(new Game_ItemGenerateComponent());
            var boardRenderComponent = Root.AddComponent(new Game_BoardRenderComponent());

            itemPoolComponent.Init(boardData.tileModels, boardData.transform);
            itemGenerateComponent.Init(boardData.itemPrefab, boardData.itemsPool, boardData.artAtlas);
            boardRenderComponent.Init(boardData.rowCount, boardData.columnCount, boardData.tileSize);
        }

        /// 交换位置操作逻辑
        public UnityGame GetUnityGame()
        {
            var gameConfig = new GameConfig
            {
                GameBoardData = boardData,
                ItemSwapper = new AnimatedItemSwapper(),
                GameBoardSolver = new GameBoardSolver(GetSequenceDetectors(), GetSpecialItemDetectors())
            };

            return new UnityGame(inputSystem, gameConfig);
        }


        /// 线性消除器
        private static ILineDetect[] GetSequenceDetectors()
        {
            return new ILineDetect[]
            {
                new Game_VerticalLineDetectComponent(),
                new Game_HorizontalLineDetectComponent()
            };
        }

        /// 特殊消除器
        private static ISpecialItemDetector[] GetSpecialItemDetectors()
        {
            return null;
        }
    }
}
namespace Match3
{
    public class GameConfig
    {
        /// 棋盘绘制器
        public GameBoardData GameBoardData { get; set; }

        /// 交换动画
        public AnimatedItemSwapper ItemSwapper { get; set; }

        /// 消除解决方案
        public GameBoardSolver GameBoardSolver { get; set; }
    }
}
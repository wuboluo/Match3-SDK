using UnityEngine;
using UnityEngine.UI;

namespace Match3
{
    public class Init : MonoBehaviour
    {
        public Button startBtn;

        private void Start()
        {
            startBtn.onClick.AddListener(StartGame);
        }

        private static void StartGame()
        {
            var fillComponent = World.Instance.Root.GetComponent<Game_RiseFillStrategyComponent>();
            var jobComponent = World.Instance.Root.GetComponent<Game_JobComponent>();

            // 填充棋盘
            var boardFillJobs = fillComponent.GetBoardFillJobs();
            jobComponent.Execute(boardFillJobs);

            Debug.Log("游戏开始");
        }
    }
}
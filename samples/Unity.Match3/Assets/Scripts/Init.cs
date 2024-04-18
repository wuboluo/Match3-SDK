using UnityEngine;

namespace Match3
{
    public class Init : MonoBehaviour
    {
        private void Start()
        {
            StartGame();
        }

        private static void StartGame()
        {
            var fillComponent = World.Instance.Root.GetComponent<Game_FillComponent>();
            var jobComponent = World.Instance.Root.GetComponent<Game_JobComponent>();

            // 填充棋盘Job
            var boardFillJobs = fillComponent.GetBoardFillJobs();
            jobComponent.Execute(boardFillJobs);

            Debug.Log("游戏开始");
        }
    }
}
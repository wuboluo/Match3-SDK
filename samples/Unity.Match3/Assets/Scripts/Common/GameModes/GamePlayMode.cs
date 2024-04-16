using System;
using Cysharp.Threading.Tasks;

public class GamePlayMode
{
    private readonly GameUiCanvas _gameUiCanvas;
    private readonly RiseFillStrategy _riseFillStrategy;
    private readonly UnityGame _unityGame;

    public GamePlayMode(AppContext appContext)
    {
        _unityGame = appContext.Resolve<UnityGame>();
        _gameUiCanvas = appContext.Resolve<GameUiCanvas>();
        _riseFillStrategy = appContext.Resolve<RiseFillStrategy>();
    }

    public void Deactivate()
    {
        _unityGame.LevelGoalAchieved -= OnLevelGoalAchieved;
        _gameUiCanvas.StrategyChanged -= OnStrategyChanged;

        _unityGame.StopAsync().Forget();
        _gameUiCanvas.ShowMessage("Game finished.");
    }

    public event EventHandler Finished
    {
        add => _unityGame.Finished += value;
        remove => _unityGame.Finished -= value;
    }

    public void Activate()
    {
        _unityGame.LevelGoalAchieved += OnLevelGoalAchieved;
        _gameUiCanvas.StrategyChanged += OnStrategyChanged;

        // 设置填充策略
        _unityGame.SetGameBoardFillStrategy(GetSelectedFillStrategy());
        // 开始游戏 生成格子
        _unityGame.StartAsync().Forget();

        _gameUiCanvas.ShowMessage("Game started.");
    }

    private void OnLevelGoalAchieved(object sender, LevelGoal levelGoal)
    {
        _gameUiCanvas.RegisterAchievedGoal(levelGoal);
    }

    private void OnStrategyChanged(object sender, int index)
    {
        _unityGame.SetGameBoardFillStrategy(GetFillStrategy());
    }

    /// 选择的填充策略
    private RiseFillStrategy GetSelectedFillStrategy()
    {
        return GetFillStrategy();
    }

    private RiseFillStrategy GetFillStrategy()
    {
        return _riseFillStrategy;
    }
}
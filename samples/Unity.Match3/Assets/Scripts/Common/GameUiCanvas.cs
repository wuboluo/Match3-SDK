using System;
using System.Linq;
using UnityEngine;

public class GameUiCanvas : MonoBehaviour
{
    [SerializeField] private App _app;
    [SerializeField] private AppContext _appContext;
    [SerializeField] private InteractableDropdown _iconsSetDropdown;
    [SerializeField] private InteractableDropdown _fillStrategyDropdown;
    [SerializeField] private InteractableButton _startGameButton;

    public int SelectedIconsSetIndex => _iconsSetDropdown.SelectedIndex;
    public int SelectedFillStrategyIndex => _fillStrategyDropdown.SelectedIndex;

    private void Start()
    {
        _iconsSetDropdown.AddItems(_appContext.Resolve<IconsSetModel[]>().Select(iconsSet => iconsSet.Name));
        // _fillStrategyDropdown.AddItems(_appContext.Resolve<RiseFillStrategy>().Select(strategy => strategy.Name));
    }

    private void OnEnable()
    {
        _startGameButton.Click += OnStartGameButtonClick;
        _fillStrategyDropdown.IndexChanged += OnFillStrategyDropdownIndexChanged;
    }

    private void OnDisable()
    {
        _startGameButton.Click -= OnStartGameButtonClick;
        _fillStrategyDropdown.IndexChanged -= OnFillStrategyDropdownIndexChanged;
    }

    public event EventHandler StartGameClick;
    public event EventHandler<int> StrategyChanged;

    public void ShowMessage(string message)
    {
        Debug.Log(message);
    }

    /// 目标完成时（目标内容：单次消除一整行）
    public void RegisterAchievedGoal(LevelGoal achievedGoal)
    {
        ShowMessage($"目标 {achievedGoal.GetType().Name} 已完成");
    }

    private void OnStartGameButtonClick()
    {
        Debug.Log("游戏开始");
        _app.drawGameBoardMode.Activate();
        _app.gameInitMode.Activate();
        _app.gamePlayMode.Activate();
        // StartGameClick?.Invoke(this, EventArgs.Empty);
    }

    private void OnFillStrategyDropdownIndexChanged(int index)
    {
        StrategyChanged?.Invoke(this, index);
    }
}
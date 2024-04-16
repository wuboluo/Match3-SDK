using System;

public class GameResetMode : IGameMode
{
    private readonly UnityGameBoardRenderer _gameBoardRenderer;
    private readonly UnityItemGenerator _itemsPool;
    private readonly UnityGame _unityGame;

    public GameResetMode(AppContext appContext)
    {
        _unityGame = appContext.Resolve<UnityGame>();
        _itemsPool = appContext.Resolve<UnityItemGenerator>();
        _gameBoardRenderer = appContext.Resolve<UnityGameBoardRenderer>();
    }

    public event EventHandler Finished;

    public void Activate()
    {
        _itemsPool.ReturnAllItems(_unityGame.GetGridSlots());
        _gameBoardRenderer.ResetGridTiles();
        _unityGame.ResetGameBoard();

        Finished?.Invoke(this, EventArgs.Empty);
    }
}
using System;
using System.Collections.Generic;
using UnityEngine;

public class AppContext : MonoBehaviour //, IAppContext
{
    [SerializeField] private GameUiCanvas _gameUiCanvas;

    [SerializeField] private CanvasInputSystem _inputSystem;

    [SerializeField] private UnityGameBoardRenderer _gameBoardRenderer;

    [Space] [SerializeField] private GameObject _itemPrefab;

    [Space] [SerializeField] private IconsSetModel[] _iconSets;

    private Dictionary<Type, object> _registeredTypes;

    private void Awake()
    {
        Construct();
    }

    public T Resolve<T>()
    {
        return (T)_registeredTypes[typeof(T)];
    }

    public void Construct()
    {
        _registeredTypes = new Dictionary<Type, object>();

        RegisterInstance(_inputSystem);
        RegisterInstance(_iconSets);
        RegisterInstance(_gameUiCanvas);
        RegisterInstance(_gameBoardRenderer);
        RegisterInstance(GetUnityGame());
        RegisterInstance(GetItemGenerator());
        RegisterInstance(GetBoardFillStrategies());

        // RegisterInstance(GetBoardFillStrategies());
    }

    private void RegisterInstance<T>(T instance)
    {
        _registeredTypes.Add(typeof(T), instance);
    }

    private UnityGame GetUnityGame()
    {
        var gameConfig = new GameConfig
        {
            GameBoardDataProvider = _gameBoardRenderer,
            ItemSwapper = new AnimatedItemSwapper(),
            GameBoardSolver = GetGameBoardSolver()
        };

        return new UnityGame(_inputSystem, _gameBoardRenderer, gameConfig);
    }

    private UnityItemGenerator GetItemGenerator()
    {
        return new UnityItemGenerator(_itemPrefab, new GameObject("ItemsPool").transform);
    }

    private GameBoardSolver GetGameBoardSolver()
    {
        return new GameBoardSolver(GetSequenceDetectors(), GetSpecialItemDetectors());
    }

    private LineDetector[] GetSequenceDetectors()
    {
        return new LineDetector[]
        {
            new VerticalLineDetector(),
            new HorizontalLineDetector()
        };
    }

    private ISpecialItemDetector[] GetSpecialItemDetectors()
    {
        return null;
        // return new ISpecialItemDetector[]
        // {
        //     new StoneItemDetector(),
        //     new IceItemDetector()
        // };
    }

    // private static ISolvedSequencesConsumer[] GetSolvedSequencesConsumers()
    // {
    //     return new ISolvedSequencesConsumer[]
    //     {
    //         new GameScoreBoard()
    //     };
    // }

    private RiseFillStrategy GetBoardFillStrategies()
    {
        return new RiseFillStrategy(this);
    }
}
﻿using Match3.App;
using Match3.App.Interfaces;
using Match3.Template;
using Match3.Template.Interfaces;
using Match3.Template.SequenceDetectors;
using Microsoft.Extensions.DependencyInjection;
using Terminal.Match3.FillStrategies;
using Terminal.Match3.Interfaces;
using Terminal.Match3.TileGroupDetectors;

namespace Terminal.Match3
{
    public static class DependencyInjection
    {
        public static void AddInfrastructure(this IServiceCollection services)
        {
            var gameBoardRenderer = new TerminalGameBoardRenderer();
            services.AddSingleton<IGameBoardRenderer>(gameBoardRenderer);
            services.AddSingleton<ITerminalGameBoardRenderer>(gameBoardRenderer);

            var gameConfig = GetGameConfig(gameBoardRenderer);
            services.AddSingleton<GameConfig<ITerminalGridSlot>>(gameConfig);

            var itemGenerator = new TerminalItemGenerator();
            services.AddSingleton<IItemGenerator>(itemGenerator);
            services.AddSingleton<IItemsPool<ITerminalItem>>(itemGenerator);

            services.AddSingleton<TerminalGame>();
            services.AddSingleton<ITerminalInputSystem, TerminalInputSystem>();
            services.AddSingleton<IBoardFillStrategy<ITerminalGridSlot>, SimpleFillStrategy>();
        }

        private static GameConfig<ITerminalGridSlot> GetGameConfig(TerminalGameBoardRenderer gameBoardRenderer)
        {
            return new GameConfig<ITerminalGridSlot>
            {
                GameBoardDataProvider = gameBoardRenderer,
                LevelGoalsProvider = new LevelGoalsProvider(),
                ItemSwapper = new TerminalItemSwapper(gameBoardRenderer),
                GameBoardSolver = GetGameBoardSolver(),
                SolvedSequencesConsumers = GetSolvedSequencesConsumers(gameBoardRenderer)
            };
        }

        private static IGameBoardSolver<ITerminalGridSlot> GetGameBoardSolver()
        {
            return new GameBoardSolver<ITerminalGridSlot>(new ISequenceDetector<ITerminalGridSlot>[]
            {
                new VerticalLineDetector<ITerminalGridSlot>(),
                new HorizontalLineDetector<ITerminalGridSlot>()
            });
        }

        private static ISolvedSequencesConsumer<ITerminalGridSlot>[] GetSolvedSequencesConsumers(
            ITerminalGameBoardRenderer gameBoardRenderer)
        {
            return new ISolvedSequencesConsumer<ITerminalGridSlot>[]
            {
                new TileGroupDetector(gameBoardRenderer)
            };
        }
    }
}
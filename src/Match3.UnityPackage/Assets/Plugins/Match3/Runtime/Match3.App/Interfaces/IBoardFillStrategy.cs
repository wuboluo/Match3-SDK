using System.Collections.Generic;
using Match3.Core.Interfaces;

namespace Match3.App.Interfaces
{
    public interface IBoardFillStrategy<TGridSlot> where TGridSlot : IGridSlot
    {
        string Name { get; }

        /// 填充Jobs
        IEnumerable<IJob> GetFillJobs(IGameBoard<TGridSlot> gameBoard);
        
        /// 消除Jobs
        IEnumerable<IJob> GetSolveJobs(IGameBoard<TGridSlot> gameBoard, SolvedData<TGridSlot> solvedData);
    }
}
using System;

namespace Match3
{
    public interface IGameMode
    {
        event EventHandler Finished;
        void Activate();
    }
}
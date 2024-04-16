using System;

public interface IGameMode
{
    event EventHandler Finished;
    void Activate();
}
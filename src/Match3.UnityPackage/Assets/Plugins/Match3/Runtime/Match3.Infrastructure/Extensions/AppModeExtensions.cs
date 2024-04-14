using System;
using Match3.Infrastructure.Interfaces;

namespace Match3.Infrastructure.Extensions
{
    public static class AppModeExtensions
    {
        public static void Deactivate(this IGameMode gameMode)
        {
            if (gameMode is IDeactivatable deactivation)
            {
                deactivation.Deactivate();
            }
        }

        public static void Dispose(this IGameMode gameMode)
        {
            if (gameMode is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
    }
}
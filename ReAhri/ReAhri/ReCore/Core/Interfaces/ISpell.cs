﻿namespace ReAhri.ReCore.Core
{
    interface ISpell
    {
        void Execute();
        bool ShouldGetExecuted();
        void OnDraw();
        void OnEndScene();
    }
}

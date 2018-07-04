using System;
using System.Collections.Generic;
using System.Text;

namespace BryanPorter.IntervalTrainer.Shared.Interfaces
{
    public enum State
    {
        Created,
        Started,
        Restarted,
        Resumed,
        Paused,
        Stopped,
        Destroyed
    }

    public interface IPresenter
    {
        void Notify(State state);
    }
}

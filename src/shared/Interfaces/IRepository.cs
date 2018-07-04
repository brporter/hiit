namespace BryanPorter.IntervalTrainer.Shared.Interfaces
{
    using System;
    using System.Collections.Generic;
    using BryanPorter.IntervalTrainer.Shared.Models;

    public interface IRepository
        : IRefreshable
    {
        Routine GetRoutine(Guid routineId);
        Routine[] GetRoutines();
        void SaveRoutine(Routine routine);
        void DeleteRoutine(Routine routine);
    }
}
namespace BryanPorter.IntervalTrainer.Shared.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    using BryanPorter.IntervalTrainer.Shared.Models;


    /// <summary>
    /// Interface for views that present a list of routines. Routines may be edited and removed.
    /// </summary>
    public interface IRoutineListView
    {
        void BindData(Routine[] routines);
        void NavigateToRoutineEditor(Guid routineId);
        void NavigateToRoutineExecutor(Guid routineId);
        void RefreshDataBindings(Routine[] routines);
    }
}

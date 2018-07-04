namespace BryanPorter.IntervalTrainer.Shared.Interfaces
{
    using BryanPorter.IntervalTrainer.Shared;
    using BryanPorter.IntervalTrainer.Shared.Models;

    public interface IRoutineListPresenter
        : IPresenter
    {
        void OnReadyForDataBinding();
        void OnItemSelected(Routine selectedItem);
        void OnItemSelectedForDeletion(Routine selectedItem);
        void OnItemSelectedForExecution(Routine selectedItem);
    }
}
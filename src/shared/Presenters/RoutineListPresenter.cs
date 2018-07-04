namespace BryanPorter.IntervalTrainer.Shared.Presenters
{
    using System;
    using BryanPorter.IntervalTrainer.Shared.Interfaces;
    using BryanPorter.IntervalTrainer.Shared.Models;

    public class RoutineListPresenter
        : IRoutineListPresenter
    {
        readonly IRoutineListView _view;
        readonly IRepository _repository;
        bool _dataRefreshNeeded;

        public RoutineListPresenter(IRoutineListView view, IRepository repository)
        {
            _view = view;
            _repository = repository;
        }

        public void Notify(State state)
        {
            if (state == State.Paused)
            {
                _dataRefreshNeeded = true;
            }

            if (state == State.Resumed 
                && _dataRefreshNeeded)
            {
                _dataRefreshNeeded = false;
                _view.RefreshDataBindings(_repository.GetRoutines());
            }
        }

        public void OnItemSelected(Routine selectedItem)
        {
            if (selectedItem == null)
            {
                _view.NavigateToRoutineEditor(Guid.Empty);
            }
            else
            {
                _view.NavigateToRoutineEditor(selectedItem.RoutineId);
            }
        }

        public void OnItemSelectedForDeletion(Routine selectedItem)
        {
            _repository.DeleteRoutine(selectedItem);
            _view.RefreshDataBindings(_repository.GetRoutines());
        }

        public void OnItemSelectedForExecution(Routine selectedItem)
        {
            throw new System.NotImplementedException();
        }

        public void OnReadyForDataBinding()
        {
            _view.BindData(_repository.GetRoutines());
        }
    }
}

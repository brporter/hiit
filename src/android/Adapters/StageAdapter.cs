

namespace BryanPorter.IntervalTrainer.Droid.Adapters
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using Android.App;
    using Android.Content;
    using Android.OS;
    using Android.Runtime;
    using Android.Views;
    using Android.Widget;

    using BryanPorter.IntervalTrainer.Shared.Interfaces;
    using BryanPorter.IntervalTrainer.Shared.Models;

    public class StageAdapter
        : BaseAdapter<Stage>, IRefreshable
    {
        readonly IRepository _repository;
        Routine _routine;

        public StageAdapter(IRepository repository, Routine routine)
        {
            _repository = repository;
            _routine = routine;
        }

        public override Stage this[int position] => _routine.Stages[position];

        public override int Count => _routine.Stages != null ? _routine.Stages.Count : 0;

        public override long GetItemId(int position) => _routine.Stages[position].GetHashCode();

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            throw new NotImplementedException();
        }

        public void Refresh()
        {
            _routine = _repository.GetRoutine(_routine.RoutineId);
            this.NotifyDataSetChanged();
        }
    }
}
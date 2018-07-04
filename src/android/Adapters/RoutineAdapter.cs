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

    public class RoutineAdapter
        : BaseAdapter<Routine>
    {
        Routine[] _routines;
        readonly Context _context;

        public RoutineAdapter(Context context, Routine[] routines)
        {
            _context = context;
            _routines = routines;
        }

        public override Routine this[int position] => _routines[position];

        public override int Count => _routines.Length;

        public override long GetItemId(int position)
        {
            return this[position].GetHashCode();
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            if (convertView == null)
            {
                convertView = LayoutInflater.From(_context).Inflate(Resource.Layout.routine_item_layout, null);
            }

            var routineView = convertView.FindViewById<TextView>(Resource.Id.routineName);
            var stageDescription = convertView.FindViewById<TextView>(Resource.Id.stageDescription);

            var routine = this[position];

            routineView.Text = routine.Name;
            stageDescription.Text = string.Format("{0} stages, {1} total minutes.",
                routine.Stages != null ? routine.Stages.Count : 0,
                routine.Stages != null ? routine.Stages.Sum(s => (s.WorkTime.TotalMinutes + s.RestTime.TotalMinutes) * s.RepeatCount) : 0
            );

            return convertView;
        }

        public virtual void Refresh(Routine[] routines)
        {
            _routines = routines;
            this.NotifyDataSetChanged();
        }
    }
}
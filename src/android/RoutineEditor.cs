namespace BryanPorter.IntervalTrainer.Droid
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

    [Activity(Label = "RoutineEditor")]
    public class RoutineEditor 
        : Activity
    {
        Routine _routine;

        EditText _routineNameEditor;
        Button _saveRoutineButton;

        public RoutineEditor()
        {
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_routineeditor);

            _routineNameEditor = FindViewById<EditText>(Resource.Id.routineNameEditor);
            _saveRoutineButton = FindViewById<Button>(Resource.Id.saveRoutineButton);

            var routineId = Intent.GetByteArrayExtra("routineid");

            //if (routineId != null)
            //{
            //    _routine = UnitOfWork.Repository.GetRoutine(new Guid(routineId));

            //    _routineNameEditor.Text = _routine.Name;
            //}
            //else
            //{
            //    _routine = new Routine()
            //    {
            //        RoutineId = Guid.NewGuid(),
            //        Stages = new List<Stage>()
            //    };
            //}

            //_saveRoutineButton.Click += (s, e) =>
            //{
            //    _routine.Name = _routineNameEditor.Text;

            //    UnitOfWork.Repository.SaveRoutine(_routine);
            //    Finish();
            //};
        }
    }
}
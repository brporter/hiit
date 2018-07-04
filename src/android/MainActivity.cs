namespace BryanPorter.IntervalTrainer.Droid
{
    using Android.App;
    using Android.Widget;
    using Android.OS;
    using Android.Support.V7.App;

    using BryanPorter.IntervalTrainer.Droid.Adapters;
    using BryanPorter.IntervalTrainer.Shared.Interfaces;
    using BryanPorter.IntervalTrainer.Shared.Models;
    using BryanPorter.IntervalTrainer.Shared.Presenters;
    using BryanPorter.IntervalTrainer.Shared;
    using System.Collections.Generic;
    using System.Linq;
    using Android.Content;
    using Android.Runtime;
    using System;

    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity 
        : AppCompatActivity, IRoutineListView
    {
        readonly IRoutineListPresenter _presenter;

        RoutineAdapter _adapter;
        ListView _routineListView;
        Button _newRoutineButton;
        

        public MainActivity()
        {
            // TODO: DI container. Holy crap.
            _presenter = new RoutineListPresenter(this, new Repository(new FileSystemStorage(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal)))); 
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            _routineListView = FindViewById<ListView>(Resource.Id.routineListView);
            _newRoutineButton = FindViewById<Button>(Resource.Id.newRoutineButton);

            _newRoutineButton.Click += (s, e) =>
            {
                _presenter.OnItemSelected(null);
            };

            _routineListView.ItemClick += (s, e) =>
            {
                _presenter.OnItemSelected(_adapter[e.Position]);
            };

            _presenter.OnReadyForDataBinding();
        }

        protected override void OnPause()
        {
            _presenter.Notify(State.Paused);

            base.OnPause();
        }

        protected override void OnResume()
        {
            _presenter.Notify(State.Resumed);

            base.OnResume();
        }

        public void BindData(Routine[] routines)
        {
            _adapter = null;
            RefreshDataBindings(routines);
        }

        public void NavigateToRoutineEditor(Guid routineId)
        {
            var intent = new Intent(this, typeof(RoutineEditor));
            intent.PutExtra("routineid", routineId.ToByteArray());

            StartActivityForResult(intent, 0);
        }

        public void NavigateToRoutineExecutor(Guid routineId)
        {
            throw new NotImplementedException();
        }

        public void RefreshDataBindings(Routine[] routines)
        {
            if (_adapter == null)
            {
                _adapter = new RoutineAdapter(this, routines);
                _routineListView.Adapter = _adapter;
            }
            else
            {
                _adapter.Refresh(routines);
            }
        }
    }
}


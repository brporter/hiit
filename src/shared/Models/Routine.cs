using System;
using System.Collections.Generic;
using System.Text;

namespace BryanPorter.IntervalTrainer.Shared.Models
{
    public enum LevelOfEffort
    {
        Light,
        Moderate,
        Hard,
        VeryHard
    }

    [Serializable]
    public class Routine
    {
        public Guid RoutineId { get; set; }
        public string Name { get; set; }
        public IList<Stage> Stages { get; set; }
    }

    [Serializable]
    public class Stage
    {
        public Guid StageId { get; set; }
        public LevelOfEffort Effort { get; set; }
        public TimeSpan WorkTime { get; set; }
        public TimeSpan RestTime { get; set; }
        public int RepeatCount { get; set; }
        public Guid RoutineId { get; set; }
    }
}

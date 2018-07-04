namespace BryanPorter.IntervalTrainer.Shared.Tests.Integration
{
    using System;
    using System.Collections.Generic;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using BryanPorter.IntervalTrainer.Shared.Interfaces;
    using BryanPorter.IntervalTrainer.Shared.Models;
    using BryanPorter.IntervalTrainer.Shared;

    [TestClass]
    public class FileSystemTests
    {
        [TestMethod]
        public void Test_RoutineSaveAndLoad()
        {
            IStorage fileSystem = new BryanPorter.IntervalTrainer.Shared.FileSystemStorage(".");
            IRepository repo = new BryanPorter.IntervalTrainer.Shared.Repository(fileSystem);

            var routine = new Routine()
            {
                Name = "Test Routine #1",
                RoutineId = Guid.NewGuid(),
                Stages = new Stage[]
                {
                    new Stage()
                    {
                        Effort = LevelOfEffort.Hard,
                        RepeatCount = 5,
                        RestTime = TimeSpan.FromMinutes(2),
                        WorkTime = TimeSpan.FromMinutes(2),
                        StageId = Guid.NewGuid()
                    }
                }
            };

            foreach (var stage in routine.Stages)
                stage.RoutineId = routine.RoutineId;

            var result = repo.GetRoutine(routine.RoutineId);

            Assert.IsNull(result);

            repo.SaveRoutine(routine);

            result = repo.GetRoutine(routine.RoutineId);

            Assert.IsNotNull(result);
            Assert.AreEqual(routine, result); // should be the same reference if the cache worked

            var routines = repo.GetRoutines();

            int count = 0;

            foreach (var r in routines)
            {
                count++;

                Assert.IsTrue(count == 1);
                Assert.AreEqual(routine, r); // should be the same reference if the cache worked
            }

            IRepository secondRepo = new BryanPorter.IntervalTrainer.Shared.Repository(fileSystem);

            var secondRoutines = secondRepo.GetRoutines();
            var secondRoutine = default(Routine);

            foreach (var sr in secondRoutines)
            {
                Assert.IsTrue(sr.RoutineId == result.RoutineId);
                Assert.AreNotEqual(sr, result);

                secondRoutine = sr;
                break;
            }

            secondRoutine.Name = "Fizzle";

            secondRepo.SaveRoutine(secondRoutine);
            var newResult = repo.GetRoutine(result.RoutineId);

            bool exceptionThrown = false;

            try
            {
                repo.SaveRoutine(result);
            }
            catch (Exception e)
            {
                exceptionThrown = true;
                Assert.IsInstanceOfType(e, typeof(InvalidOperationException));
            }

            Assert.IsTrue(exceptionThrown);
        }
    }
}

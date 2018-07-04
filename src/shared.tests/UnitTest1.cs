namespace BryanPorter.IntervalTrainer.Tests
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.Runtime.Serialization.Formatters.Binary;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    using BryanPorter.IntervalTrainer.Shared;
    using BryanPorter.IntervalTrainer.Shared.Interfaces;
    using BryanPorter.IntervalTrainer.Shared.Models;

    [TestClass]
    public class RepositoryTests
    {
        static readonly BinaryFormatter _formatter = new BinaryFormatter();

        private static Mock<IStorage> ConfigureStorageMock(out Guid routineId)
        {
            routineId = Guid.NewGuid();
            var localRoutineId = routineId;

            var routine = new Routine()
            {
                RoutineId = routineId,
                Name = "Test Routine",
                Stages = new List<Stage>()
                {
                    new Stage()
                    {
                        Effort = LevelOfEffort.Hard,
                        WorkTime = TimeSpan.FromMinutes(1),
                        RestTime = TimeSpan.FromMinutes(2),
                        RepeatCount = 3,
                        RoutineId = routineId,
                        StageId = Guid.NewGuid()
                    },
                    new Stage()
                    {
                        Effort = LevelOfEffort.Light,
                        WorkTime = TimeSpan.FromMinutes(4),
                        RestTime = TimeSpan.FromMinutes(5),
                        RepeatCount = 6,
                        RoutineId = routineId,
                        StageId = Guid.NewGuid()
                    },
                }
            };

            var storageMock = new Mock<IStorage>();
            storageMock.Setup(m => m.GetStorageIdentifiers()).Returns(() => new[] { localRoutineId });
            storageMock.Setup(m => m.GetLastWriteDateTimeUtc(It.IsAny<Guid>())).Returns(() => DateTime.MinValue);
            storageMock.Setup(m => m.GetReadableStream(It.IsAny<Guid>())).Returns(() =>
            {
                var memoryStream = new System.IO.MemoryStream();
                _formatter.Serialize(memoryStream, routine);

                memoryStream.Seek(0, System.IO.SeekOrigin.Begin);
                return memoryStream;
            });

            storageMock.Setup(m => m.GetWriteableStream(It.IsAny<Guid>())).Returns(() =>
            {
                var memoryStream = new System.IO.MemoryStream();

                return memoryStream;
            });

            storageMock.Setup(m => m.RemoveStorage(It.IsAny<Guid>())).Callback(() =>
            { });

            storageMock.Setup(m => m.CheckStorageIdentifierExists(It.IsAny<Guid>())).Returns(true);

            return storageMock;
        }

        [TestMethod]
        public void Test_GetRoutine()
        {
            // Arrange
            var routineId = default(Guid);
            var storageMock = ConfigureStorageMock(out routineId);

            var repository = new Repository(storageMock.Object);


            // Act
            var routineFirstLoad = repository.GetRoutine(routineId);
            var routineSecondLoad = repository.GetRoutine(routineId);

            // Assert
            Assert.AreEqual(routineFirstLoad.RoutineId, routineId);
            Assert.AreEqual(routineFirstLoad, routineSecondLoad); // did caching do its job?

            // Since second load should have come from cache, GetReadableStream should not have been called more than once
            storageMock.Verify(m => m.GetReadableStream(It.IsAny<Guid>()), Times.Once());

            // Since first load would not have resulted in cache invalidation check, last write time should not have been called more than once - for second load
            storageMock.Verify(m => m.GetLastWriteDateTimeUtc(It.IsAny<Guid>()), Times.Once());
        }

        [TestMethod]
        public void Test_GetRoutines()
        {
            // Arrange
            var routineId = default(Guid);
            var storageMock = ConfigureStorageMock(out routineId);

            var repository = new Repository(storageMock.Object);


            // Act
            var routines = repository.GetRoutines().ToArray();
            var specificRoutine = repository.GetRoutine(routineId);

            // Assert
            Assert.IsTrue(routines.Count() == 1);
            Assert.AreEqual(routines.First().RoutineId, routineId);
            Assert.AreEqual(routines.First(), specificRoutine); // did caching do its job?

            // storage id enumeration should only happen once per GetRoutines() call
            storageMock.Verify(m => m.GetStorageIdentifiers(), Times.Once());

            // get readable stream should have happened for the first GetRoutines(), cache should have satisfied second call to GetRoutine()
            storageMock.Verify(m => m.GetReadableStream(It.IsAny<Guid>()), Times.Once());

            // Since first load would not have resulted in cache invalidation check, last write time should not have been called more than once - for second load
            storageMock.Verify(m => m.GetLastWriteDateTimeUtc(It.IsAny<Guid>()), Times.Once());
        }

        [TestMethod]
        public void Test_SaveRoutine()
        {
            var routineId = default(Guid);
            var storageMock = ConfigureStorageMock(out routineId);
            var repository = new Repository(storageMock.Object);

            // act

            var routine = new Routine()
            {
                RoutineId = Guid.NewGuid(),
                Name = "Foo",
                Stages = new Stage[] { }
            };

            repository.SaveRoutine(routine);

            var specificRoutine = repository.GetRoutine(routine.RoutineId);

            Assert.AreEqual(routine.RoutineId, specificRoutine.RoutineId);
            Assert.AreEqual(routine, specificRoutine);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Test_DeleteRoutine()
        {
            var routineId = default(Guid);
            var storageMock = ConfigureStorageMock(out routineId);
            var repository = new Repository(storageMock.Object);

            // act
            var routine = repository.GetRoutine(routineId);
            repository.DeleteRoutine(routine);

            // should throw
            repository.SaveRoutine(routine);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Test_InvalidatedReference()
        {
            var routineId = default(Guid);
            var storageMock = ConfigureStorageMock(out routineId);
            var repository = new Repository(storageMock.Object);

            // act
            var routine = repository.GetRoutine(routineId);

            storageMock.Setup(m => m.GetLastWriteDateTimeUtc(It.IsAny<Guid>())).Returns(DateTime.UtcNow.AddHours(1));

            var secondRoutineLoad = repository.GetRoutine(routineId);

            Assert.AreNotEqual(routine, secondRoutineLoad); // caching should have invalidated the second load, and made a new object

            // should throw
            repository.SaveRoutine(routine);
        }
    }
}

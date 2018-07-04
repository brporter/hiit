namespace BryanPorter.IntervalTrainer.Shared.Interfaces
{
    using System;
    using System.IO;
    using System.Collections.Generic;
    using System.Text;

    public interface IStorage
    {
        IEnumerable<Guid> GetStorageIdentifiers();
        bool CheckStorageIdentifierExists(Guid id);

        DateTime GetLastWriteDateTimeUtc(Guid id);

        Stream GetWriteableStream(Guid id);

        Stream GetReadableStream(Guid id);

        void RemoveStorage(Guid id);
    }
}

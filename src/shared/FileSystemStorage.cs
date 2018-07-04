namespace BryanPorter.IntervalTrainer.Shared
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;

    using BryanPorter.IntervalTrainer.Shared.Interfaces;

    public class FileSystemStorage
        : IStorage
    {
        const string FileExtension = ".routine";
        readonly string _storagePath;

        public FileSystemStorage(string storagePath)
        {
            if (!Directory.Exists(storagePath))
                throw new ArgumentOutOfRangeException("storagePath");

            _storagePath = storagePath;
        }

        public bool CheckStorageIdentifierExists(Guid id)
        {
            return File.Exists(
                CalculateFilePath(id)
            );
        }

        public IEnumerable<Guid> GetStorageIdentifiers()
        {
            var di = new DirectoryInfo(_storagePath);
            var id = default(Guid);

            foreach (var fi in di.GetFiles())
            {
                if (Guid.TryParse(fi.Name.Replace(FileExtension, string.Empty), out id))
                    yield return id;
            }
        }

        public DateTime GetLastWriteDateTimeUtc(Guid id)
        {
            var filePath = CalculateFilePath(id);

            if (File.Exists(filePath))
            {
                return File.GetLastWriteTimeUtc(filePath);
            }

            return DateTime.MinValue;
        }

        public Stream GetReadableStream(Guid id)
        {
            return File.OpenRead(CalculateFilePath(id));
        }

        public Stream GetWriteableStream(Guid id)
        {
            return File.OpenWrite(CalculateFilePath(id));
        }
        public void RemoveStorage(Guid id)
        {
            var filePath = CalculateFilePath(id);

            if (File.Exists(filePath))
                File.Delete(filePath);
        }

        private string CalculateFilePath(Guid id)
        {
            return Path.Combine(
                    _storagePath,
                    string.Format("{0}{1}", id.ToString(), FileExtension)
                );
        }
    }
}

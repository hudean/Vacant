using System;
using System.Collections.Concurrent;

namespace Vacant.Domain.Uow
{
    public class UnitOfWorkManager : IUnitOfWorkManager, IDisposable
    {
        private readonly ConcurrentDictionary<string, IUnitOfWork> _unitOfWorks;

        public UnitOfWorkManager()
        {
            _unitOfWorks = new ConcurrentDictionary<string, IUnitOfWork>();
        }

        public IUnitOfWork this[string dataBaseName]
        {
            get
            {
                if (!TryGetUnitOfWork(dataBaseName, out var unitOfWork))
                    throw new ArgumentException(dataBaseName);
                return unitOfWork;
            }
        }

        public void Dispose()
        {
            _unitOfWorks.Clear();
        }

        public void TryAddUnitOfWork(string dataBaseName, IUnitOfWork unitOfWork)
        {
            _unitOfWorks.TryAdd(dataBaseName, unitOfWork);
        }

        public bool TryGetUnitOfWork(string dataBaseName, out IUnitOfWork unitOfWork)
        {
            return _unitOfWorks.TryGetValue(dataBaseName, out unitOfWork);
        }
    }
}

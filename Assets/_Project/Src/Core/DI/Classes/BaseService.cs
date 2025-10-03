using System;
using UniRx;

namespace _Project.Src.Core.DI.Classes
{
    public abstract class BaseService : IDisposable
    {
        private readonly CompositeDisposable _disposables = new();

        protected BaseService()
        {
        }


        public IDisposable AddTo(IDisposable d)
        {
            return d.AddTo(_disposables);
        }

        public virtual void Dispose()
        {
            _disposables?.Dispose();
        }
    }

    public static class DiExtension
    {
        public static IDisposable AddTo(this IDisposable disposable, BaseService service)
        {
            return service.AddTo(disposable);
        }
    }
}
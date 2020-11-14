using System;
using System.Threading.Tasks;

namespace Threading.Extensions
{
    public interface IActor : IDisposable
    {
        void Enqueue(Func<Task> work);
    }
}
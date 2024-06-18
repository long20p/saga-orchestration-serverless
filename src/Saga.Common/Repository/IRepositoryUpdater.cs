using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saga.Common.Repository
{
    public interface IRepositoryUpdater<T>
    {
        Task<T> Update(string id, T latest);
    }
}

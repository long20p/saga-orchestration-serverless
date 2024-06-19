using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saga.Common.Repository
{
    public interface IRepositoryClient<T>
    {
        Task<T> GetAsync(string id);

        Task<T> UpdateAsync(string id, T latest);
    }
}

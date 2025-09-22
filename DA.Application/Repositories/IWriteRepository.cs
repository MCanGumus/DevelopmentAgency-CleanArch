using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using DA.Domain.Entities;
using System.Linq.Expressions;

namespace DA.Application.Repositories
{
    public interface IWriteRepository<T> : IRepository<T> where T : BaseEntity
    {
        Task<bool> AddAsync(T model);
        Task<bool> AddRangeAsync(List<T> models);
        bool Remove(T model);
        bool AddRange(List<T> models);
        Task<bool> RemoveAsync(Guid id);
        bool RemoveRange(List<T> models);
        bool Update(T model);
        bool UpdateRange(List<T> models);

        void Save();
    }
}

using Microsoft.EntityFrameworkCore;
using DA.Application.Repositories;
using DA.Domain.Entities;
using DA.Persistence.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DA.Persistence.Repositories
{
    public class ReadRepository<T> : IReadRepository<T> where T : BaseEntity
    {
        private readonly DAContext _context;

        public ReadRepository(DAContext context)
        {
            _context = context;
        }

        public DbSet<T> Table => _context.Set<T>();

        public IQueryable<T> GetAll(bool tracking = true)
        {
            var query = Table.AsQueryable().AsNoTracking();
            if (!tracking)
            {
                query = query.AsNoTracking();
            }
            return query;
        }

        public IQueryable<T> GetWhere(Expression<Func<T, bool>> method, bool tracking = true)
        {
            var query = Table.Where(method).AsNoTracking();
            if (!tracking)
            {
                query = query.AsNoTracking();
            }
            return query;
        }

        public async Task<T> GetSingleAsync(Expression<Func<T, bool>> method, bool tracking = true)
        {
            var query = Table.AsQueryable().AsNoTracking();
            if (!tracking)
            {
                query = query.AsNoTracking();
            }
            return await query.FirstOrDefaultAsync(method);
        }

        public async Task<T> GetByIdAsync(Guid id, bool tracking = true)
        {
            var query = Table.AsQueryable().AsNoTracking();
            if (!tracking)
            {
                query = query.AsNoTracking();
            }
            return await Table.FirstOrDefaultAsync(model => model.Id == id);
        }

        public T GetById(Guid id, bool tracking = true)
        {
            var query = Table.AsQueryable().AsNoTracking();
            if (!tracking)
            {
                query = query.AsNoTracking();
            }
            return Table.Where(model => model.Id == id).AsNoTracking().SingleOrDefault();
        }
    }
}

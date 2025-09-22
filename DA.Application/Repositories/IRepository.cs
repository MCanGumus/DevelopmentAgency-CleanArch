using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using DA.Domain.Entities;
using System.Linq.Expressions;

namespace DA.Application.Repositories
{

    public interface IRepository<T> where T : BaseEntity
    {
        DbSet<T> Table { get; }
    }
   
}
using DA.Domain.Entities;

namespace DA.Application.Abstractions
{
    public interface IBaseService<TEntity, TUpdateEntityDto, TSaveEntityDto, TEntityDto>
        where TEntity : BaseEntity
        where TUpdateEntityDto : class, new()
        where TSaveEntityDto : class, new()
        where TEntityDto : class, new()
    {
        IEnumerable<TEntityDto> GetAll();
        Task<TEntityDto> GetByIdAsync(Guid id);
        TEntityDto GetById(Guid id);
        TEntity GetEntityById(Guid id);
        Task<bool> InsertAsync(TSaveEntityDto SaveDto);
        Task<TEntity> Insert(TSaveEntityDto SaveDto);
        Task<TEntity> InsertAsyncReturnedEntity(TSaveEntityDto SaveDto);
        void Update(TUpdateEntityDto updateDto);
        void UpdateEntity(TEntity entity);
        Task DeleteAsync(Guid id);
        void Delete(Guid id);
    }
}

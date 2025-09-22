using AutoMapper;
using Microsoft.EntityFrameworkCore;
using DA.Application.Abstractions;
using DA.Application.Repositories;
using DA.Domain.Entities;
using System.Reflection;
using System.Globalization;

namespace DA.Persistence.Services
{
    public class BaseService<TEntity, TUpdateEntityDto, TSaveEntityDto, TEntityDto> : IBaseService<TEntity, TUpdateEntityDto, TSaveEntityDto, TEntityDto>
        where TEntity : BaseEntity
        where TUpdateEntityDto : class, new()
        where TSaveEntityDto : class, new()
        where TEntityDto : class, new()


    {
        private readonly IReadRepository<TEntity> _readRepository;
        private readonly IWriteRepository<TEntity> _writeRepository;
        private readonly IMapper _mapper;

        public BaseService(IReadRepository<TEntity> readRepository, IWriteRepository<TEntity> writeRepository, IMapper mapper)
        {
            _readRepository = readRepository;
            _writeRepository = writeRepository;
            _mapper = mapper;
        }


        public async Task DeleteAsync(Guid id)
        {
            TEntity entity = await _readRepository.GetByIdAsync(id);
            entity.DataType = DA.Domain.Enums.EnumDataType.Deleted;

            _writeRepository.Update(entity);
            _writeRepository.Save();
        }

        public void Delete(Guid id)
        {
            TEntity entity = _readRepository.GetById(id);
            entity.DataType = DA.Domain.Enums.EnumDataType.Deleted;

            _writeRepository.Update(entity);
            _writeRepository.Save();
        }

        public IEnumerable<TEntityDto> GetAll()
        {
            var entity = _readRepository.GetAll().Where(x => x.DataType == DA.Domain.Enums.EnumDataType.New).ToList();
            IEnumerable<TEntityDto> entityDto = _mapper.Map<IEnumerable<TEntity>, IEnumerable<TEntityDto>>(entity);

            return entityDto;
        }

        public async Task<TEntityDto> GetByIdAsync(Guid id)
        {
            var entity = await _readRepository.GetByIdAsync(id);
            var entityDto = _mapper.Map<TEntityDto>(entity);

            return entityDto;
        }

        public TEntityDto GetById(Guid id)
        {
            var entity =  _readRepository.GetWhere(x => x.Id == id).AsNoTracking().SingleOrDefault();
            var entityDto = _mapper.Map<TEntityDto>(entity);

            return entityDto;
        }

        public TEntity GetEntityById(Guid id)
        {
            var entity = _readRepository.GetWhere(x => x.Id == id).AsNoTracking().SingleOrDefault();

            return entity;
        }

        public async Task<bool> InsertAsync(TSaveEntityDto SaveDto)
        {

            var entity = _mapper.Map<TEntity>(SaveDto);
            
            entity.DataType = DA.Domain.Enums.EnumDataType.New;
            entity.RecordDate = DateTime.Now;

            var isAdded = await _writeRepository.AddAsync(entity);
            if (isAdded)
            {
                _writeRepository.Save();

                return true;
            }

            return false;
        }

        public async Task<TEntity> Insert(TSaveEntityDto SaveDto)
        {
            var entity = _mapper.Map<TEntity>(SaveDto);

            entity.DataType = DA.Domain.Enums.EnumDataType.New;
            entity.RecordDate = DateTime.Now;

            var isAdded = await _writeRepository.AddAsync(entity);
            if (isAdded)
            {
                _writeRepository.Save();

                return entity;
            }

            return null;
        }

        public void Update(TUpdateEntityDto updateDto)
        {
            var entity = _mapper.Map<TEntity>(updateDto);

            _writeRepository.Update(entity);
            _writeRepository.Save();
        }

        public void UpdateEntity(TEntity entity)
        {
            _writeRepository.Update(entity);
            _writeRepository.Save();
        }

        public async Task<TEntity> InsertAsyncReturnedEntity(TSaveEntityDto SaveDto)
        {

            var entity = _mapper.Map<TEntity>(SaveDto);

            entity.DataType = DA.Domain.Enums.EnumDataType.New;
            entity.RecordDate = DateTime.Now;

            var isAdded = await _writeRepository.AddAsync(entity);
            if (isAdded)
            {
                _writeRepository.Save();

                return entity;
            }

            return entity;
        }


    }
}

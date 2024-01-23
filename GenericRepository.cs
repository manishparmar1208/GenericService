using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Entities.Auditing;
using Abp.EntityFrameworkCore;
using Abp.Linq.Extensions;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Ornacore.Common.Dto;
using Ornacore.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Ornacore.Common.Services
{
    public class GenericRepository<TEntity, TResponseDTO, TPagedDto> : ApplicationService, IGenericRepository<TEntity, TResponseDTO, TPagedDto>
    where TEntity : FullAuditedEntity<int>
    where TPagedDto : PageAndSortedInputDto
    {
        private IDbContextProvider<OrnacoreDbContext> _dbContextProvider;
        private DbSet<TEntity> entities;

        public GenericRepository(IDbContextProvider<OrnacoreDbContext> dbContextProvider)
        {
            _dbContextProvider = dbContextProvider;
        }

        //protected async Task<PagedResultDto<TResponseDTO>> GetAllAfterFilter(IQueryable<TEntity> query, TPagedDto input)
        //{
        //    PagedResultDto<TResponseDTO> result = new PagedResultDto<TResponseDTO>();
        //    query = query.OrderBy(input.Sorting);
        //    result.TotalCount = await query.CountAsync();
        //    var list = await query.PageBy(input).ToListAsync();
        //    result.Items = ObjectMapper.Map<List<TResponseDTO>>(list);
        //    return result;
        //}

        protected async Task<Boolean> Create(TEntity entity)
        {
            var db = await _dbContextProvider.GetDbContextAsync();
            await db.AddAsync(entity);
            return true;
        }
        
        protected async Task<Boolean> Update(TEntity entity)
        {
            var db = _dbContextProvider.GetDbContext();
            entities = db.Set<TEntity>();
            var exist = await entities.Where(x => x.Id == entity.Id).AsNoTracking().FirstOrDefaultAsync();
            if (exist != null)
            {
                entity.CreationTime = exist.CreationTime;
                entity.CreatorUserId = exist.CreatorUserId;
                entities.Update(entity);
                await db.SaveChangesAsync();
                return true;
            }
            return false;
        }

        protected async Task<TResponseDTO> GetByIdGenric(int id)
        {
            var db = _dbContextProvider.GetDbContext();
            entities = db.Set<TEntity>();
            var record = await entities.Where(x => x.Id == id).FirstOrDefaultAsync();
            if (record == null)
                throw new UserFriendlyException("Record not found");
            return ObjectMapper.Map<TResponseDTO>(record);
        }

        protected async Task<PagedResultDto<TResponseDTO>> GetAllWithIncludeFilter(TPagedDto input, Expression<Func<TEntity, bool>> query = null, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null)
        {
            PagedResultDto<TResponseDTO> result = new PagedResultDto<TResponseDTO>();
            var db = await _dbContextProvider.GetDbContextAsync();
            entities = db.Set<TEntity>();
            IQueryable<TEntity> data;
            data = entities;
            if (query != null)
                data = entities.Where(query);
            if (include != null)
                data = include(data);
            result.TotalCount = await data.CountAsync();
            var list = await data.OrderBy(input.Sorting).PageBy(input).ToListAsync();
            result.Items = ObjectMapper.Map<List<TResponseDTO>>(list);

            return result;
        }

        protected async Task<TResponseDTO> GetByIdIncludeGenric(int id, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null)
        {
            var db = _dbContextProvider.GetDbContext();
            var result = db.Set<TEntity>().AsQueryable();
            if (include != null)
                result = include(result);
            var record = await result.Where(x => x.Id == id).FirstOrDefaultAsync();
            if (record == null)
                throw new UserFriendlyException("Record not found");
            return ObjectMapper.Map<TResponseDTO>(record);
        }

        protected async Task<Boolean> DeleteGenric(int id)
        {
            var db = _dbContextProvider.GetDbContext();
            entities = db.Set<TEntity>();
            var record = await entities.Where(x => x.Id == id).FirstOrDefaultAsync();
            if (record == null)
                throw new UserFriendlyException("Record not found");
            entities.Remove(record);
            return true;
        }

        protected async Task<List<TResponseDTO>> GetDropdownList(Expression<Func<TEntity, bool>> query = null, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null)
        {
            var db = _dbContextProvider.GetDbContext();
            entities = db.Set<TEntity>();
            IQueryable<TEntity> data;
            if (query != null)
                data = entities.Where(query);
            else
                data = entities;
            if (include != null)
                data = include(data);
            data = data.OrderByDescending(x => x.CreationTime);
            return ObjectMapper.Map<List<TResponseDTO>>(data); ;
        }

        //protected async Task<PagedResultDto<TResponseDTO>> GetAllWithFilter(TPagedDto input, Expression<Func<TEntity, bool>> query)
        //{
        //    PagedResultDto<TResponseDTO> result = new PagedResultDto<TResponseDTO>();
        //    var db = await _dbContextProvider.GetDbContextAsync();
        //    entities = db.Set<TEntity>();
        //    IQueryable<TEntity> data;
        //    data = entities.Where(query).OrderBy(input.Sorting);

        //    result.TotalCount = await data.CountAsync();
        //    var list = await data.PageBy(input).ToListAsync();
        //    result.Items = ObjectMapper.Map<List<TResponseDTO>>(list);
        //    return result;
        //}

        protected async Task<Boolean> DeleteIsActive(int id)
        {
            var db = _dbContextProvider.GetDbContext();
            entities = db.Set<TEntity>();
            var record = await entities.Where(x => x.Id == id).FirstOrDefaultAsync();
            if (record != null)
                //record.isActive = false;
                if (record == null)
                    throw new UserFriendlyException("Record not found");

            entities.Update(record);
            return true;
        }
    }
}

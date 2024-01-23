using Abp.Domain.Entities.Auditing;
using Ornacore.Common.Dto;

namespace Ornacore.Common.Services
{
    public interface IGenericRepository<TEntity, TResponseDTO, TPagedDto>
    where TEntity : FullAuditedEntity<int>
    where TPagedDto : PageAndSortedInputDto
    {
        //Task<TResponseDTO> GetByIdGenric(int id);

        //Task<Boolean> Create(TEntity entity);

        //Task<Boolean> Update(TEntity entity);

        //Task<Boolean> DeleteGenric(int id);

        //Task<PagedResultDto<TResponseDTO>> GetAllAfterFilter(IQueryable<TEntity> query, TPagedDto input);
    }
}

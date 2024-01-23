using Abp.Application.Services.Dto;

namespace Ornacore.Common.Dto
{
    public  class CommonPageResultRequestDto: PagedResultRequestDto
    {
        public string Filter { get; set; }
        public bool? IsActive { get; set; }
    }
}

namespace Ornacore.Common.Dto
{
    public  class PageAndSortedInputDto : CommonPageResultRequestDto
    {
        public string Sorting { get; set; }
        public int? Type { get; set; }

        public PageAndSortedInputDto()
        {
            Sorting = "CreationTime DESC";
            Type = 1;
        }
    }
}

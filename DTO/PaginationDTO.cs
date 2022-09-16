using System;
namespace Movies_API.DTO
{
    public class PaginationDTO
    {
        public int Page { get; set; } = 1;

        private int recordsPerPage { get; set; } = 10;

        private readonly int maxRecordPerPage = 50;

        public int RecordsPerPage
        {
            get
            {
                return recordsPerPage;
            }
            set
            {
                recordsPerPage = (value > maxRecordPerPage) ? maxRecordPerPage : value;
            }
        }
    }
}


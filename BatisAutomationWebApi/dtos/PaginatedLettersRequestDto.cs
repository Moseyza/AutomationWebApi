using System;

namespace BatisAutomationWebApi.dtos
{
    public class PaginatedLettersRequestDto
    {
        public Guid OwnerId { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
    }

    public class PaginatedLettersRequestDto1
    {
        public Guid OwnerId { get; set; }
        public string From { get; set; }
        public string To { get; set; }
    }
}
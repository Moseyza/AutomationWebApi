using System;

namespace BatisAutomationWebApi.dtos
{
    public class PaginatedLettersRequestDto
    {
        public Guid OwnerId { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
    }
}
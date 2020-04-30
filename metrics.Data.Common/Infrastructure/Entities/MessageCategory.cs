﻿using System.ComponentModel.DataAnnotations;

namespace metrics.Data.Common.Infrastructure.Entities
{
    public class MessageCategory : BaseEntity
    {
        [MaxLength(255)]
        public string Title { get; set; }
    }
}
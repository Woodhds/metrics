using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.Extensions.Logging;

namespace DAL.Entities
{
    public class Log
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string MachineName { get; set; }
        public LogLevel Level { get; set; }
        public string Message { get; set; }
        public string Exception { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
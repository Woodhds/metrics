using System;

namespace DAL.Entities
{
    public class ParseMessage
    {
        public int Id { get; set; }
        public int OwnerId { get; set; }
        public string Text { get; set; }

        public uint Date { get; set; }
    }
}
﻿using System.Collections.Generic;
using Base.Contracts;

namespace metrics.Broker.Events.Events
{
    public class RepostEvent
    {
        public int UserId { get; set; }
        
        public string Token { get; set; }
        
        public IEnumerable<VkRepostViewModel> Reposts { get; set; }
    }
}
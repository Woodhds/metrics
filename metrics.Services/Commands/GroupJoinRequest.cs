using System;
using System.Threading.Tasks;
using Base.Abstract;

namespace Base.Commands
{
    public class GroupJoinRequest : IRequest
    {
        public CommandType Type => CommandType.Join;
        public int Timeout { get; set; }

        public Task Handle()
        {
            throw new System.NotImplementedException();
        }
    }
}
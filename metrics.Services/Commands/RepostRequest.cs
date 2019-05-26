using System.Threading.Tasks;
using Base;
using Base.Abstract;

namespace metrics.Services.Commands
{
    public class RepostRequest : IRequest
    {
        public CommandType Type => CommandType.Repost;
        private int _timeout { get; set; }
        private int _ownerId { get; set; }
        private int _id { get; set; }
        public int Timeout => _timeout;


        public RepostRequest(int ownerId, int id) : this(ownerId, id, 0)
        {
        }

        public RepostRequest(int ownerId, int id, int timeout)
        {
            _timeout = timeout;
            _ownerId = ownerId;
            _id = id;
        }

        public Task Handle()
        {
            throw new System.NotImplementedException();
        }
    }
}
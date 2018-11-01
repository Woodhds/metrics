using System.Linq;

namespace DAL.Core
{
    public class SelectBuilder
    {
        private string[] _props;
        public SelectBuilder(string[] props)
        {
            _props = props;
        }

        public string Build()
        {
            var model = $"new ({ string.Join(",", _props.Select(z => $"it.{z}")) })";
            return model;
        }
    }
}
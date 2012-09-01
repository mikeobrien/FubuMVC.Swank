using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swank.Models;

namespace Swank
{
    public class SpecificationBuilder
    {
        private readonly Configuration _configuration;

        public SpecificationBuilder(Configuration configuration)
        {
            _configuration = configuration;
        }

        public Specification Build()
        {
            return new Specification
                {
                    dataTypes = new List<DataType>(),
                    modules = new List<Module>()
                };
        }
    }
}

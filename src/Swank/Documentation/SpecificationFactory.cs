using System;
using System.Collections.Generic;
using System.Linq;
using FubuMVC.Swank.Specification;

namespace FubuMVC.Swank.Documentation
{
    public class LazyConcurrent<T>
    {
        private static readonly object _mutex = new object();
        private static Lazy<T> _lazy; 

        public void Ensure(Func<T> factory)
        {
            if (_lazy != null) return;
            lock (_mutex)
            {
                if (_lazy != null) return;
                _lazy = new Lazy<T>(factory);
            }
        }

        public T Value
        {
            get { return _lazy.Value; }
        }
    }

    public class SpecificationFactory
    {
        private LazyConcurrent<SpecificationModel> _specification = 
            new LazyConcurrent<SpecificationModel>();
        public SpecificationFactory(SpecificationService specificationService)
        {
            LazyConcurrent<SpecificationModel>.Ensure(() => Map(specificationService));
        }

        public SpecificationModel Create()
        {
            return LazyConcurrent<SpecificationModel>.Value;
        }

        private static SpecificationModel Map(SpecificationService specificationService)
        {
            var specification = specificationService.Generate();
            return new SpecificationModel
            {
                Name = specification.Name,
                Comments = specification.Comments,
                Modules = new List<ModuleModel>(specification.Modules.Select(x => new ModuleModel
                {
                    
                }))
            };
        }
    }
}
using Microsoft.Practices.Unity;
using PipelineNet.MiddlewareResolver;
using System;

namespace Common.MessageProcessing
{
    public class UnityMiddlewareResolver : IMiddlewareResolver
    {
        private readonly IUnityContainer _container;


        public UnityMiddlewareResolver(IUnityContainer container)
        {
            _container = container;
        }


        // IMiddlewareResolver ////////////////////////////////////////////////////////////////////
        public Object Resolve(Type type)
        {
            return _container.Resolve(type);
        }
    }
}
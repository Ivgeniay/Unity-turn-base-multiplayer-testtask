
using UnityEngine;
using Zenject;
using System;

namespace client.Assets.Scripts.ViewLayer.DIContainer
{
    public class ZenjectServiceProvider : IServiceProvider
    {
        private readonly DiContainer di;
        public ZenjectServiceProvider(DiContainer di)
        {
            this.di = di;
            
        }

        public object GetService(Type serviceType)
        {
            try
            {
                return di.Resolve(serviceType);
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.Message);
                return null;
            }
        }
    }
}
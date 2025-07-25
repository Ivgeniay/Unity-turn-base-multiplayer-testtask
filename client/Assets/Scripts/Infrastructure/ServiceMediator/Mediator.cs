using client.Assets.Scripts.Domain.Interfaces.Mediator;
using System.Collections.Generic;
using System;

namespace client.Assets.Scripts.Infrastructure.ServiceMediator
{
    public class Mediator : IMediator
    {
        private readonly IServiceProvider _serviceProvider;

        public Mediator(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public TResponse Send<TResponse>(IRequest<TResponse> request)
        {
            var requestType = request.GetType();
            var responseType = typeof(TResponse);
            var handlerType = typeof(IRequestHandler<,>).MakeGenericType(requestType, responseType);
            
            var handler = _serviceProvider.GetService(handlerType);
            var handleMethod = handlerType.GetMethod("Handle");
            
            return (TResponse)handleMethod.Invoke(handler, new object[] { request });
        }

        public void Send(IRequest request)
        {
            var requestType = request.GetType();
            var handlerType = typeof(IRequestHandler<>).MakeGenericType(requestType);
            
            var handler = _serviceProvider.GetService(handlerType);
            var handleMethod = handlerType.GetMethod("Handle");
            
            handleMethod.Invoke(handler, new object[] { request });
        }

        public void Publish<TNotification>(TNotification notification) where TNotification : INotification
        {
            var notificationType = typeof(TNotification);
            var handlerType = typeof(INotificationHandler<>).MakeGenericType(notificationType);
            var enumerableHandlerType = typeof(IEnumerable<>).MakeGenericType(handlerType);
            
            var handlers = (IEnumerable<object>)_serviceProvider.GetService(enumerableHandlerType);
            
            if (handlers != null)
            {
                foreach (var handler in handlers)
                {
                    var handleMethod = handlerType.GetMethod("Handle");
                    handleMethod.Invoke(handler, new object[] { notification });
                }
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace client.Assets.Scripts.Domain.Interfaces.Mediator
{
    public interface IMediator
    {
        TResponse Send<TResponse>(IRequest<TResponse> request);
        void Send(IRequest request);
        void Publish<TNotification>(TNotification notification) where TNotification : INotification;
    }

    public interface IRequestHandler<in TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        TResponse Handle(TRequest request);
    }

    public interface IRequest<out TResponse>
    {
    }

    public interface IRequest
    {
    }


    public interface IRequestHandler<in TRequest> where TRequest : IRequest
    {
        void Handle(TRequest request);
    }

    public interface INotification
    {
    }

    public interface INotificationHandler<in TNotification> where TNotification : INotification
    {
        void Handle(TNotification notification);
    }
    
}
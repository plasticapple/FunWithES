//using System;
//using System.Collections.Generic;
//using System.Threading;

//namespace Common.messagebus
//{

//    public interface ICommandSender
//    {
//        void Send<T>(T command) where T : SiteCommand;

//    }
//    public interface IEventPublisher
//    {
//        void Publish<T>(T @event) where T : SiteCommand;
//    }

//    public class FakeBus : ICommandSender, IEventPublisher
//    {
//        private readonly Dictionary<Type, List<Action<SiteCommand>>> _routes = new Dictionary<Type, List<Action<SiteCommand>>>();

//        public void RegisterHandler<T>(Action<T> handler) where T : SiteCommand
//        {
//            List<Action<SiteCommand>> handlers;

//            if (!_routes.TryGetValue(typeof(T), out handlers))
//            {
//                handlers = new List<Action<SiteCommand>>();
//                _routes.Add(typeof(T), handlers);
//            }

//            handlers.Add((x => handler((T)x)));
//        }

//        public void Send<T>(T command) where T : SiteCommand
//        {
//            List<Action<SiteCommand>> handlers;

//            if (_routes.TryGetValue(typeof(T), out handlers))
//            {
//                if (handlers.Count != 1) throw new InvalidOperationException("cannot send to more than one handler");
//                handlers[0](command);
//            }
//            else
//            {
//                throw new InvalidOperationException("no handler registered");
//            }
//        }

//        public void Publish<T>(T @event) where T : SiteCommand
//        {
//            List<Action<SiteCommand>> handlers;

//            if (!_routes.TryGetValue(@event.GetType(), out handlers)) return;

//            foreach (var handler in handlers)
//            {
//                //dispatch on thread pool for added awesomeness
//                var handler1 = handler;
//                ThreadPool.QueueUserWorkItem(x => handler1(@event));
//            }
//        }
//    }
//}

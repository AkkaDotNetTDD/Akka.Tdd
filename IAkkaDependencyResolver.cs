using System;
using Akka.Actor;

namespace Akka.Tdd.Core
{
    public interface IAkkaDependencyResolver
    {
        Action<Type, bool> TypeRegistrer { set; get; }
        Action<ActorSystem> TypeIterationCompleted { set; get; }
    }
}
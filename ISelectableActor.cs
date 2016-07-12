using Akka.Actor;
using System;

namespace Akka.Tdd.Core
{
    public interface ISelectableActor
    {
        string ActorName { get; set; }

        Type Actortype { get; set; }

        ActorMetaData ActorMetaData { get; set; }
        ActorSystem ActorSystem { get; set; }
    }
}
using Akka.Actor;
using System;

namespace Akka.Tdd.Core
{
    public class SelectableActor : ISelectableActor
    {
        public string ActorName { set; get; }

        public Type Actortype { set; get; }
        public ActorMetaData ActorMetaData { get; set; }
        public ActorSystem ActorSystem { get; set; }
    }
}
using Akka.Actor;
using Akka.Configuration.Hocon;
using Akka.DI.Core;
using Akka.Routing;
using System;
using System.Configuration;

namespace Akka.Tdd.Core
{
    public static class SelectableActorExtensions
    {
        public static Props GetActorProps(this ISelectableActor selectableActor, ActorSystem system, ActorSetUpOptions options = null)
        {
            var props = system.DI().Props(selectableActor.Actortype);
            props = selectableActor.PrepareProps(options, props);
            return props;
        }

        public static Props GetActorProps(this ISelectableActor selectableActor, IActorContext actorContext, ActorSetUpOptions options = null)
        {
            var props = actorContext.DI().Props(selectableActor.Actortype);
            props = selectableActor.PrepareProps(options, props);
            return props;
        }

        public static ISelectableActor SetUp<T>(this ISelectableActor selectableActor, ActorSystem system, string actorName = null, ActorMetaData parentActorMetaData = null) where T : ActorBase
        {
            selectableActor.Actortype = typeof(T);
            selectableActor.ActorName = selectableActor.GetActorNameByType(actorName, selectableActor.Actortype);
            selectableActor.ActorMetaData = selectableActor.ActorMetaDataByName(selectableActor.ActorName, parentActorMetaData);
            selectableActor.ActorSystem = system;
            return selectableActor;
        }

        public static ISelectableActor SetUp(this ISelectableActor selectableActor, Type actorType, ActorSystem system, string actorName = null, ActorMetaData parentActorMetaData = null)
        {
            selectableActor.Actortype = actorType;
            selectableActor.ActorName = selectableActor.GetActorNameByType(actorName, selectableActor.Actortype);
            selectableActor.ActorMetaData = selectableActor.ActorMetaDataByName(selectableActor.ActorName, parentActorMetaData);
            selectableActor.ActorSystem = system;
            return selectableActor;
        }

        public static string GetActorNameByType(this ISelectableActor selectableActor, string actorName, Type actorType)
        {
            if (!typeof(ActorBase).IsAssignableFrom(actorType))
            {
                throw new Exception("Object supplied is not an actor");
            }
            var name = !string.IsNullOrEmpty(actorName)
                ? actorName
                : (!actorType.Name.Contains("`") ? actorType.Name : actorType.Name.Replace("`", "_"));

            return name;
        }

        public static ActorMetaData ActorMetaDataByName(this ISelectableActor selectableActor, string actorName, ActorMetaData parentActorMetaData = null)
        {
            return new ActorMetaData(actorName, parentActorMetaData ?? new ActorMetaData("user"));
        }

        public static ActorSelection Select(this ISelectableActor selectableActor, Type type, ActorMetaData parentActorMetaData, ActorSystem actorSystem)
        {
            var metaData = selectableActor.ActorMetaDataByName(selectableActor.GetActorNameByType(null, type), parentActorMetaData);
            return actorSystem.ActorSelection(metaData.Path);
        }

        public static ActorSelection Select(this ISelectableActor selectableActor, ActorMetaData metaData, ActorSystem actorSystem)
        {
            return actorSystem.ActorSelection(metaData.Path);
        }

        public static ActorSelection Select(this ISelectableActor selectableActor, IActorContext context = null)
        {
            return context == null ?
                selectableActor.ActorSystem.ActorSelection(selectableActor.ActorMetaData.Path) :
                context.ActorSelection(selectableActor.ActorMetaData.Path);
        }

        public static IActorRef Create(this ISelectableActor selectableActor, ActorSystem system, ActorSetUpOptions options = null)
        {
            var props = system.DI().Props(selectableActor.Actortype);
            props = selectableActor.PrepareProps(options, props);
            return system.ActorOf(props, selectableActor.ActorName);
        }

        public static IActorRef Create(this ISelectableActor selectableActor, IActorContext actorContext, ActorSetUpOptions options = null)
        {
            var props = actorContext.DI().Props(selectableActor.Actortype);
            props = selectableActor.PrepareProps(options, props);
            var actorRef = actorContext.ActorOf(props, name: selectableActor.ActorName);

            return actorRef;
        }

        public static Props PrepareProps(this ISelectableActor selectableActor, ActorSetUpOptions options, Props props = null)
        {
            if (props == null)
            {
                return null;
            }
            var section = (AkkaConfigurationSection)ConfigurationManager.GetSection("akka");
            if (section?.AkkaConfig?.GetConfig("akka.actor.deployment") != null)
            {
                props = props.WithRouter(FromConfig.Instance);
            }
            else
            {
                if (options?.RouterConfig != null)
                {
                    props = props.WithRouter(options.RouterConfig);
                }
            }
            if (options == null) return props;
            if (options.RouterConfig != null)
            {
                props = props.WithRouter(options.RouterConfig);
            }
            if (options.SupervisoryStrategy != null)
            {
                props = props.WithSupervisorStrategy(options.SupervisoryStrategy);
            }
            if (options.Dispatcher != null)
            {
                props = props.WithDispatcher(options.Dispatcher);
            }
            if (options.MailBox != null)
            {
                props = props.WithMailbox(options.MailBox);
            }
            return props;
        }

        public static string GetActorName(this ISelectableActor selectableActor)
        {
            return selectableActor.ActorName;
        }
    }
}
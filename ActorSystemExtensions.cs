using Akka.Actor;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Akka.Tdd.Core
{
    public static class ActorSystemExtensions
    {
        internal static ISelectableActor CreateActorSelector<T>(this ActorSystem actorSystem, ActorMetaData parentActorMetaData = null) where T : ActorBase
        {
            return new SelectableActor().SetUp<T>(actorSystem, null, parentActorMetaData);
        }

        /// <summary>
        /// Selects an existing action i.e an actor that has already been created
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="actorSystem"></param>
        /// <param name="context"></param>
        /// <param name="parentActorMetaData"></param>
        /// <returns></returns>
        public static ActorSelection LocateActor<T>(this ActorSystem actorSystem, ActorMetaData parentActorMetaData = null) where T : ActorBase
        {
            return actorSystem.LocateActor(typeof(T), parentActorMetaData);
        }

        public static IActorRef LocateActorRef<T>(this ActorSystem actorSystem, ActorMetaData parentActorMetaData = null, TimeSpan? resolutionTime = null) where T : ActorBase
        {
            try
            {
                return Task.Run(() => actorSystem.LocateActor(typeof(T), parentActorMetaData).ResolveOne(resolutionTime ?? TimeSpan.FromMinutes(1))).Result;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public static ActorSelection LocateActor<T>(this ActorSystem actorSystem, ActorSelection parentActorSelection) where T : ActorBase
        {
            return actorSystem.LocateActor(typeof(T), parentActorSelection);
        }

        public static ActorSelection LocateActor(this ActorSystem actorSystem, Type type, ActorMetaData parentActorMetaData = null)
        {
            return new SelectableActor().Select(type, parentActorMetaData ?? new ActorMetaData("user"), actorSystem);
        }

        public static ActorSelection LocateActor(this ActorSystem actorSystem, Type type, IActorRef parentActorRef)
        {
            return new SelectableActor().Select(type, parentActorRef != null ? parentActorRef.ToActorMetaData() : new ActorMetaData("user"), actorSystem);
        }

        public static ActorSelection LocateActor(this ActorSystem actorSystem, ActorMetaData actorMetaData)
        {
            return new SelectableActor().Select(actorMetaData, actorSystem);
        }

        public static ActorSelection LocateActor(this ActorSystem actorSystem, Type type, ActorSelection parentActorSelection)
        {
            return actorSystem.LocateActor(type, parentActorSelection?.ToActorMetaData());
        }

        public static ActorMetaData ToActorMetaData(this ActorSelection selection, TimeSpan? resolutionTime = null)
        {
            if (selection == null) throw new ArgumentNullException(nameof(selection));
            if (string.IsNullOrEmpty(selection.PathString))
            {
                throw new Exception("Invalid selection actor path");
            }
            try
            {
                var task = Task.Run(() => selection.ResolveOne(resolutionTime ?? TimeSpan.FromMinutes(1)));
                task.Wait();
                return task.Result.ToActorMetaData();
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public static ActorMetaData ToActorMetaData(this IActorRef actorRef)
        {
            if (actorRef == null) throw new ArgumentNullException(nameof(actorRef));
            if (string.IsNullOrEmpty(actorRef.Path.ToString()))
            {
                throw new Exception("Invalid parent actor path");
            }
            return new ActorMetaData(actorRef.Path.Name, new ActorMetaData(actorRef.Path.Parent.ToString()));
        }

        public static ActorSelection LocateActor<T, TP>(this ActorSystem actorSystem) where T : ActorBase where TP : ActorBase
        {
            var parentActorSelection = actorSystem.LocateActor(typeof(TP));

            if (string.IsNullOrEmpty(parentActorSelection.PathString))
            {
                throw new Exception("Invalid parent actor path");
            }

            var parentActorMetaData = new ActorMetaData(parentActorSelection.PathString.Split('/').Last());

            return actorSystem.CreateActorSelector<T>(parentActorMetaData).Select();
        }

        public static string GetActorName<T>(this ActorSystem actorSystem) where T : ActorBase
        {
            return actorSystem.CreateActorSelector<T>().ActorName;
        }

        public static IActorRef CreateActor<T>(this ActorSystem actorSystem, ActorSetUpOptions option = null, ActorMetaData parentActorMetaData = null) where T : ActorBase
        {
            return actorSystem.CreateActorSelector<T>(parentActorMetaData).Create(actorSystem, option);
        }

        public static IActorRef CreateActor<T>(this IActorContext actorContext, ActorSetUpOptions option = null, ActorMetaData parentActorMetaData = null) where T : ActorBase
        {
            return actorContext.System.CreateActorSelector<T>(parentActorMetaData).Create(actorContext, option);
        }

        public static string GetBackOffSupervisedName<TActorType>(this ActorSystem system) where TActorType : ActorBase
        {
            return system.GetActorName<TActorType>() + "Supervised";
        }

        public static Props GetActorProps<T>(this ActorSystem actorSystem, ActorSetUpOptions option = null, ActorMetaData parentActorMetaData = null) where T : ActorBase
        {
            return actorSystem.CreateActorSelector<T>(parentActorMetaData).GetActorProps(actorSystem, option);
        }

        public static string GetActorName<T>(this ActorSystem actorSystem, ActorMetaData parentActorMetaData) where T : ActorBase
        {
            return actorSystem.CreateActorSelector<T>(parentActorMetaData).GetActorName();
        }

        public static Props GetActorProps<T>(this IActorContext actorContext, ActorSetUpOptions option = null, ActorMetaData parentActorMetaData = null) where T : ActorBase
        {
            return actorContext.System.CreateActorSelector<T>(parentActorMetaData).GetActorProps(actorContext, option);
        }

        public static string GetActorName<T>(this IActorContext actorContext, ActorMetaData parentActorMetaData = null) where T : ActorBase
        {
            return actorContext.System.CreateActorSelector<T>(parentActorMetaData).GetActorName();
        }

        public static string GetActorRelativePathString(this IActorRef actor)
        {
            try
            {
                return actor.Path.ToString().Replace(actor.Path.Root.ToString(), "");
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static string GetActorRelativePathString(this ActorSelection actor)
        {
            return actor.PathString;
        }
    }
}
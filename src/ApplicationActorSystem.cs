using Akka.Actor;

using System;
using System.Linq;

namespace Akka.Tdd.Core
{
    public class ApplicationActorSystem : IDisposable
    {
        public ActorSystem ActorSystem { get; set; }

        [Obsolete("please use the method 'RegisterAndCreateActorSystem()' instead")]
        public void Register(IAkkaDependencyResolver resolver)
        {
            RegisterAndCreateActorSystem(resolver, null, null);
        }
        public void RegisterAndCreateActorSystem(IAkkaDependencyResolver resolver)
        {
            RegisterAndCreateActorSystem(resolver, null, null);
        }
        public void RegisterAndCreateActorSystem(IAkkaDependencyResolver resolver, string actorSystemName)
        {
            if (actorSystemName == null) throw new ArgumentNullException(nameof(actorSystemName));
            RegisterAndCreateActorSystem(resolver, null, actorSystemName);
        }

        public void RegisterAndCreateActorSystem(IAkkaDependencyResolver resolver, ActorSystem actorSystem )
        {
            if (actorSystem == null) throw new ArgumentNullException(nameof(actorSystem));
            RegisterAndCreateActorSystem(resolver, actorSystem, null);
        }

        public void RegisterAndCreateActorSystem(IAkkaDependencyResolver resolver, ActorSystem actorSystem ,string systemName)
        {
            if (resolver == null) throw new ArgumentNullException(nameof(resolver));
            var actorSystemName = string.IsNullOrEmpty(systemName)? typeof(ApplicationActorSystem).Name:systemName;
            ActorSystem = actorSystem ?? Akka.Actor.ActorSystem.Create(actorSystemName);
            if (resolver.TypeRegistrer == null) return;
            IterateThroughAllActorsInCurrentAppDomain(resolver.TypeRegistrer);
            resolver.TypeIterationCompleted?.Invoke(ActorSystem);
        }

        private static void IterateThroughAllActorsInCurrentAppDomain(Action<Type, bool> registrer)
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                try
                {
                    var types = assembly.GetTypes();

                    foreach (var type in types.Where(t => typeof(ActorBase).IsAssignableFrom(t)))
                    {
                        if (type.IsGenericType)
                        {
                            registrer?.Invoke(type, true);
                        }
                        else
                        {
                            registrer?.Invoke(type, true);
                        }
                    }
                }
                catch (System.Reflection.ReflectionTypeLoadException e)
                {
                }
            }
        }

        public void ShutDownActorSystem()
        {
            Dispose();
        }

        public void Dispose()
        {
            var name = ActorSystem.Name;
            ActorSystem.Terminate();
            ActorSystem.WhenTerminated.Wait();
            ActorSystem.Dispose();
            Console.WriteLine("ActorSystem terminated at " + DateTime.UtcNow + " : " + name);
        }
    }
}
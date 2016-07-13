using Akka.Actor;
using Akka.Routing;

namespace Akka.Tdd.Core
{
    public class ActorSetUpOptions
    {
        public RouterConfig RouterConfig { get; set; }
        public SupervisorStrategy SupervisoryStrategy { get; set; }
        public string Dispatcher { get; set; }
        public string MailBox { get; set; }
    }
}
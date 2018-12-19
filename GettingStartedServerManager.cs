using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnifiedAutomation.UaBase;
using UnifiedAutomation.UaServer; 

namespace MyUAServer
{
    internal class GettingStartedServerManager : ServerManager
    {
        protected override void OnRootNodeManagerStarted(RootNodeManager nodeManager)
        {
            Console.WriteLine("Creating Node Managers.");
            Lesson1NodeManager lession1 = new Lesson1NodeManager(this);
            lession1.Startup();
        }

    }
}

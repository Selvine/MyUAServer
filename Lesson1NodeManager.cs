using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using UnifiedAutomation.UaBase;
using UnifiedAutomation.UaServer;
using System.Reflection;

namespace MyUAServer
{
    internal class Lesson1NodeManager : BaseNodeManager
    {
        public Lesson1NodeManager(ServerManager server, params string[] namespaceUris) : base(server, namespaceUris)
        {
        }
        public ushort InstanceNamespaceIndex { get; set; }
        public ushort TypeNamespaceIndex { get; set; }
        /// [Set Namespace URI]
        public override void Startup()
        {
            try
            {
                Console.WriteLine("Starting Lesson1NodeManager.");

                base.Startup();

                InstanceNamespaceIndex = AddNamespaceUri("http://yourorganisation.org/MethodModel/");
                TypeNamespaceIndex = AddNamespaceUri(THI.MM.Namespaces.MM);

                Console.WriteLine("Loading the Controller Model.");
                ImportUaNodeset(Assembly.GetEntryAssembly(), "methodmodel.xml");

                // initialize the underlying system.
                m_system.Initialize();

                // Create a Folder for Controllers
                CreateObjectSettings settings = new CreateObjectSettings()
                {
                    ParentNodeId = ObjectIds.ObjectsFolder,
                    ReferenceTypeId = ReferenceTypeIds.Organizes,
                    RequestedNodeId = new NodeId("Controllers", InstanceNamespaceIndex),
                    BrowseName = new QualifiedName("Controllers", InstanceNamespaceIndex),
                    TypeDefinitionId = ObjectTypeIds.FolderType
                };
                CreateObject(Server.DefaultRequestContext, settings);
                //ObjectNode controllersNode =

                // Create an PLC Conditioner Controller
                //  settings = new CreateObjectSettings()
                //  {
                //      ParentNodeId = controllersNode.NodeId,
                //      ReferenceTypeId = ReferenceTypeIds.Organizes,
                //      RequestedNodeId = new NodeId("PLC11", InstanceNamespaceIndex),
                //      BrowseName = new QualifiedName("PLC1", InstanceNamespaceIndex),
                //      TypeDefinitionId = new NodeId(THI.MM.ObjectTypes.PLCControllerType, TypeNamespaceIndex)
                //  };
                //  CreateObject(Server.DefaultRequestContext, settings);

                foreach (BlockConfiguration block in m_system.GetBlocks())
                {
                    // set type definition NodeId
                    NodeId typeDefinitionId = ObjectTypeIds.BaseObjectType;
                    if (block.Type == BlockType.PLC)
                    {
                        typeDefinitionId = new NodeId(THI.MM.ObjectTypes.PLCControllerType, TypeNamespaceIndex);
                    }

                    // create object.
                    settings = new CreateObjectSettings()
                    {
                        ParentNodeId = new NodeId("Controllers", InstanceNamespaceIndex),
                        ReferenceTypeId = ReferenceTypeIds.Organizes,
                        RequestedNodeId = new NodeId(block.Name, InstanceNamespaceIndex),
                        BrowseName = new QualifiedName(block.Name, TypeNamespaceIndex),
                        TypeDefinitionId = typeDefinitionId
                    };
                    CreateObject(Server.DefaultRequestContext, settings);

                    // set addressing information for method nodes that allows them to be called.
                    SetChildUserData(
                        new NodeId(block.Name, InstanceNamespaceIndex),
                        new QualifiedName(THI.MM.BrowseNames.Click, TypeNamespaceIndex),
                        new SystemFunction() { Address = block.Address, Function = 1 });

                    //SetChildUserData(
                    //    new NodeId(block.Name, InstanceNamespaceIndex),
                    //    new QualifiedName(THI.MM.BrowseNames.Stop, TypeNamespaceIndex),
                    //    new SystemFunction() { Address = block.Address, Function = 2 });

                    foreach (BlockProperty property in block.Properties)
                    {
                        // the node was already created when the controller object was instantiated.
                        // this call links the node to the underlying system data.
                        VariableNode variable = SetVariableConfiguration(
                            new NodeId(block.Name, InstanceNamespaceIndex),
                            new QualifiedName(property.Name, TypeNamespaceIndex),
                            NodeHandleType.ExternalPolled,
                            new SystemAddress() { Address = block.Address, Offset = property.Offset });
                    }
                }
            }
            /// [Implement Method]
            /// [Set Namespace URI]
            catch (Exception e)
            {
                Console.WriteLine("Failed to start Lesson1NodeManager " + e.Message);
            }
        }

        public override void Shutdown()
        {
            try
            {
                Console.WriteLine("Stopping Lesson1NodeManager.");

                // TBD 

                base.Shutdown();
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to stop Lesson1NodeManager " + e.Message);
            }
        }
        /// [Forward Method to Method Handler]
        protected override CallMethodEventHandler GetMethodDispatcher(
           RequestContext context,
           MethodHandle methodHandle)
        {
            if (methodHandle.MethodData is SystemFunction)
            {
                return DispatchControllerMethod;
            }

            return null;
        }
        /// <summary>
        /// Dispatches a method to the controller.
        /// </summary>
        /// [Implement Method]
        private StatusCode DispatchControllerMethod(
            RequestContext context,
            MethodHandle methodHandle,
            IList<Variant> inputArguments,
            List<StatusCode> inputArgumentResults,
            List<Variant> outputArguments)
        {
            SystemFunction data = methodHandle.MethodData as SystemFunction;

            if (data != null)
            {
                switch (data.Function)
                {
                    case 1:
                        {
                            return m_system.Click(data.Address);
                        }
                }
            }

            return StatusCodes.BadNotImplemented;
        }
        
        

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public Lesson1NodeManager(ServerManager server) : base(server)
        {
            m_system = new UnderlyingSystem();
        }
        #endregion

        #region IDisposable
        /// <summary>
        /// An overrideable version of the Dispose.
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // TBD
            }
        }
        #endregion

        #region Private Methods
        #endregion

        #region Private Fields
        private UnderlyingSystem m_system;
        #endregion

        #region SystemAddress Class
        private class SystemAddress
        {
            public int Address;
            public int Offset;
        }
        #endregion

        #region SystemFunction Class
        /// [SystemFunction]
        private class SystemFunction
        {
            public int Address;
            public int Function;
        }
        /// [SystemFunction]
        #endregion

    }
}

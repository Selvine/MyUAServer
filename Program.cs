using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnifiedAutomation.UaBase;
using UnifiedAutomation.UaServer;
using UnifiedAutomation.UaSchema;

namespace MyUAServer
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                // The license file must be loaded from an embedded resource.
                ApplicationLicenseManager.AddProcessLicenses(System.Reflection.Assembly.GetExecutingAssembly(), "License.lic");
                // Start the server.
                Console.WriteLine("Starting Server.");
                GettingStartedServerManager server = new GettingStartedServerManager();
                //***********************************************************************
                // The following function can be called to configure the server from code
                // This will disable the configuration settings from app.config file
                //ConfigureOpcUaApplicationFromCode();
                //***********************************************************************
                ApplicationInstance.Default.AutoCreateCertificate = true;
                ApplicationInstance.Default.Start(server, null, server);
                // Print endpoints for information.
                PrintEndpoints(server);
                // Block until the server exits.
                Console.WriteLine("Press <enter> to exit the program.");
                Console.ReadLine();
                // Stop the server.
                Console.WriteLine("Stopping Server.");
                server.Stop();
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR: {0}", e.Message);
                Console.WriteLine("Press <enter> to exit the program.");
                Console.ReadLine();
            }
        }

        static void PrintEndpoints(GettingStartedServerManager server)
        {
            // print the endpoints.
            Console.WriteLine(string.Empty);
            Console.WriteLine("Listening at the following endpoints:");
            foreach (EndpointDescription endpoint in ApplicationInstance.Default.Endpoints)
            {
                StatusCode error = server.Application.GetEndpointStatus(endpoint);
                Console.WriteLine("   {0}: Status={1}", endpoint, error.ToString(true));
            }
            Console.WriteLine(string.Empty);
        }

        static void ConfigureOpcUaApplicationFromCode()
        {
            // fill in the application settings in code
            // The configuration settings are typically provided by another module
            // of the application or loaded from a data base. In this example the
            // settings are hardcoded
            SecuredApplication application = new SecuredApplication();
            // ***********************************************************************
            // standard configuration options
            // general application identification settings
            application.ApplicationName = "UnifiedAutomation GettingStartedServer";
            application.ApplicationUri = "urn:localhost:UnifiedAutomation:GettingStartedServer";
            application.ApplicationType = UnifiedAutomation.UaSchema.ApplicationType.Server_0;
            application.ProductName = "UnifiedAutomation GettingStartedServer";
            // configure certificate stores
            application.ApplicationCertificate = new UnifiedAutomation.UaSchema.CertificateIdentifier();
            application.ApplicationCertificate.StoreType = "Directory";
            application.ApplicationCertificate.StorePath = @"%CommonApplicationData%\unifiedautomation\UaSdkNet\pki\own";
            application.ApplicationCertificate.SubjectName = "CN=GettingStartedServer/O=UnifiedAutomation/DC=localhost";
            application.TrustedCertificateStore = new UnifiedAutomation.UaSchema.CertificateStoreIdentifier();
            application.TrustedCertificateStore.StoreType = "Directory";
            application.TrustedCertificateStore.StorePath = @"%CommonApplicationData%\unifiedautomation\UaSdkNet\pki\trusted";
            application.IssuerCertificateStore = new UnifiedAutomation.UaSchema.CertificateStoreIdentifier();
            application.IssuerCertificateStore.StoreType = "Directory";
            application.IssuerCertificateStore.StorePath = @"%CommonApplicationData%\unifiedautomation\UaSdkNet\pki\issuers";
            application.RejectedCertificatesStore = new UnifiedAutomation.UaSchema.CertificateStoreIdentifier();
            application.RejectedCertificatesStore.StoreType = "Directory";
            application.RejectedCertificatesStore.StorePath = @"%CommonApplicationData%\unifiedautomation\UaSdkNet\pki\rejected";
            // configure endpoints
            application.BaseAddresses = new UnifiedAutomation.UaSchema.ListOfBaseAddresses();
            application.BaseAddresses.Add("opc.tcp://localhost:48030");
            application.SecurityProfiles = new ListOfSecurityProfiles();
            application.SecurityProfiles.Add(new SecurityProfile() { ProfileUri = SecurityProfiles.Basic256Sha256, Enabled = true });
            application.SecurityProfiles.Add(new SecurityProfile() { ProfileUri = SecurityProfiles.None, Enabled = true });
            // ***********************************************************************
            // ***********************************************************************
            // extended configuration options
            // trace settings
            TraceSettings trace = new TraceSettings();
            trace.MasterTraceEnabled = true;
            trace.DefaultTraceLevel = UnifiedAutomation.UaSchema.TraceLevel.Info;
            trace.TraceFile = @"%CommonApplicationData%\unifiedautomation\logs\ConfigurationServer.log.txt";
            trace.MaxLogFileBackups = 3;
            trace.ModuleSettings = new ModuleTraceSettings[]
                {
                    new ModuleTraceSettings() { ModuleName = "UnifiedAutomation.Stack", TraceEnabled = true },
                    new ModuleTraceSettings() { ModuleName = "UnifiedAutomation.Server", TraceEnabled = true },
                };
            application.Set<TraceSettings>(trace);
            // Installation settings
            InstallationSettings installation = new InstallationSettings();
            installation.GenerateCertificateIfNone = true;
            installation.DeleteCertificateOnUninstall = true;
            application.Set<InstallationSettings>(installation);
            // ***********************************************************************
            // set the configuration for the application (must be called before start to have any effect).
            // these settings are discarded if the /configFile flag is specified on the command line.
            ApplicationInstance.Default.SetApplicationSettings(application);
        }



    }
}

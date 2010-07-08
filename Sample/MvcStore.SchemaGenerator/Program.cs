using System;
using System.Configuration;
using MvcStoreModels.NHibernate;

namespace MvcStore.SchemaGenerator
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var mode = ConfigurationManager.AppSettings["mode"];
            if (string.IsNullOrEmpty(mode))
            {
                if (args.Length < 1)
                {
                    PrintUsage();
                    return;    
                }
                mode = args[0];    
            }
                
            
            var nhConfig = new NHStoreConfiguration();
            switch (mode)
            {
                case "create":
                    Console.Write("Executing create mode...");
                    nhConfig.SchemaCreate();
                    Console.WriteLine("Done");
                    break;
                case "update":
                    Console.Write("Executing update mode...");
                    nhConfig.SchemaUpdate();
                    Console.WriteLine("Done.");
                    break;
                default:
                    PrintUsage();
                    break;
            }
        }

        static void PrintUsage()
        {
            Console.WriteLine("Failed to find mode appSetting in config file and no/invalid command line parameters found.");
            Console.WriteLine("Either add a mode attribute the the application's appSettings file or use the following command line usage.");
            Console.WriteLine("Usage: mvcstore.schemagenerator.exe [mode]");
            Console.WriteLine("Where [mode] arugment must be one of the following arguments:");
            Console.WriteLine("create [Drops all tables from the target database and then re-creates all tables and associations.");
            Console.WriteLine("update [Updates the target database schema.]");
        }
    }
}
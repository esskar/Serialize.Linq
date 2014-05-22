using System;
using System.ServiceModel;
using Serialize.Linq.Examples.WcfHost.Services;

namespace Serialize.Linq.Examples.WcfHost
{
    class Program
    {
        static void Main()
        {
            using (var host = new ServiceHost(typeof(PersonService)))
            {
                host.Open();

                Console.WriteLine("The service is ready.");
                Console.WriteLine("Press <ENTER> to terminate service.");                
                Console.ReadLine();                
            }
        }
    }
}

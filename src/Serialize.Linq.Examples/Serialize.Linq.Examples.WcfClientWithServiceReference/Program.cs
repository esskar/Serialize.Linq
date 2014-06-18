using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Serialize.Linq.Examples.WcfClientWithServiceReference.PersonServiceReference;
using Serialize.Linq.Examples.WcfContracts.Entities;
using Serialize.Linq.Extensions;

namespace Serialize.Linq.Examples.WcfClientWithServiceReference
{
    class Program
    {
        static void Main()
        {
            using (var client = new PersonServiceClient())
            {
                RunAllPersons(client);
                RunAllPersonsFromJapan(client);
                RunAllPersonsOfAge100(client);
                RunAllMalePersons(client);
                RunAllLivingPersons(client);
            }
        }

        private static void RunAllPersons(IPersonService client)
        {
            Console.WriteLine("All persons");

            var persons = client.GetAllPersons();
            ShowPersons(persons);
        }

        private static void RunAllPersonsFromJapan(IPersonService client)
        {
            Console.WriteLine("All persons from Japan");
            Expression<Func<Person, bool>> expression = p => p.Residence == "Japan";

            var persons = client.FindPersons(expression.ToExpressionNode());
            ShowPersons(persons);
        }

        private static void RunAllPersonsOfAge100(IPersonService client)
        {
            Console.WriteLine("All persons of Age >= 100");
            Expression<Func<Person, bool>> expression = p => p.Age >= 100;

            var persons = client.FindPersons(expression.ToExpressionNode());
            ShowPersons(persons);
        }

        private static void RunAllMalePersons(IPersonService client)
        {
            Console.WriteLine("All male persons");
            Expression<Func<Person, bool>> expression = p => p.Gender == Gender.Male;

            var persons = client.FindPersons(expression.ToExpressionNode());
            ShowPersons(persons);
        }

        private static void RunAllLivingPersons(IPersonService client)
        {
            Console.WriteLine("All living persons");
            Expression<Func<Person, bool>> expression = p => p.DeathDate == null;

            var persons = client.FindPersons(expression.ToExpressionNode());
            ShowPersons(persons);
        }

        private static void ShowPersons(IEnumerable<Person> persons)
        {
            foreach (var person in persons)
                Console.WriteLine("{0}) {1} {2}, {5}, age {3}, form {4}", person.Id, person.FirstName, person.LastName, person.Age, person.Residence, person.Gender);
            Console.WriteLine();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Serialize.Linq.Examples.RestContracts.Entities;
using Serialize.Linq.Extensions;

namespace Serialize.Linq.Examples.RestClient
{
    class Program
    {
        private static readonly List<MediaTypeFormatter> _formatters;
        private static readonly MediaTypeWithQualityHeaderValue _mediaTypeJson;

        static Program()
        {
            _formatters = new List<MediaTypeFormatter> { new JsonMediaTypeFormatter
            {
                SerializerSettings = new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Objects
                }
            }};
            _mediaTypeJson = new MediaTypeWithQualityHeaderValue("application/json");
        }

        static void Main()
        {
            RunAsync().Wait();
        }

        private static async Task RunAsync()
        {
            var cancellationToken = CancellationToken.None;

            await RunAllPersonsAsync(cancellationToken);
            await RunAllPersonsFromJapan(cancellationToken);
            await RunAllPersonsOfAge100(cancellationToken);
            await RunAllMalePersons(cancellationToken);
            await RunAllLivingPersons(cancellationToken);
        }

        private static async Task RunAllPersonsAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("All persons");

            var persons = await GetAllPersons(cancellationToken);
            ShowPersons(persons);
        }

        private static async Task RunAllPersonsFromJapan(CancellationToken cancellationToken)
        {
            Console.WriteLine("All persons from Japan");
            Expression<Func<Person, bool>> expression = p => p.Residence == "Japan";

            var persons = await QueryPersons(expression, cancellationToken);
            ShowPersons(persons);
        }

        private static async Task RunAllPersonsOfAge100(CancellationToken cancellationToken)
        {
            Console.WriteLine("All persons of Age >= 100");
            Expression<Func<Person, bool>> expression = p => p.Age >= 100;

            var persons = await QueryPersons(expression, cancellationToken);
            ShowPersons(persons);
        }

        private static async Task RunAllMalePersons(CancellationToken cancellationToken)
        {
            Console.WriteLine("All male persons");
            Expression<Func<Person, bool>> expression = p => p.Gender == Gender.Male;

            var persons = await QueryPersons(expression, cancellationToken);
            ShowPersons(persons);
        }

        private static async Task RunAllLivingPersons(CancellationToken cancellationToken)
        {
            Console.WriteLine("All living persons");
            Expression<Func<Person, bool>> expression = p => p.DeathDate == null;

            var persons = await QueryPersons(expression, cancellationToken);
            ShowPersons(persons);
        }

        private static async Task<IEnumerable<Person>> GetAllPersons(CancellationToken cancellationToken)
        {
            using (var client = PrepareHttpClient())
            {
                var response = await client.GetAsync("api/Person", cancellationToken);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsAsync<IEnumerable<Person>>(_formatters, cancellationToken);
            }
        }

        private static async Task<IEnumerable<Person>> QueryPersons(Expression<Func<Person, bool>> query, CancellationToken cancellationToken)
        {
            var queryNode = query.ToExpressionNode();
            using (var client = PrepareHttpClient())
            {
                var response = await client.PostAsync("api/Person", queryNode, _formatters[0], _mediaTypeJson, cancellationToken);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsAsync<IEnumerable<Person>>(_formatters, cancellationToken);
            }
        }

        private static HttpClient PrepareHttpClient()
        {
            var client = new HttpClient { BaseAddress = new Uri("http://localhost:60376/") };
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(_mediaTypeJson);

            return client;
        }

        private static void ShowPersons(IEnumerable<Person> persons)
        {
            foreach (var person in persons)
                Console.WriteLine("{0}) {1} {2}, {5}, age {3}, form {4}", person.Id, person.FirstName, person.LastName, person.Age, person.Residence, person.Gender);
            Console.WriteLine();
        }
    }
}

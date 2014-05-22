using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Serialize.Linq.Examples.WcfContracts.Entities;
using Serialize.Linq.Examples.WcfContracts.Interfaces;
using Serialize.Linq.Nodes;

namespace Serialize.Linq.Examples.WcfHost.Services
{
    public class PersonService : IPersonService
    {
        private readonly List<Person> _persons;
        private readonly Func<DateTime> _todayFunc;

        public PersonService()
            : this(() => DateTime.Today) { }

        public PersonService(Func<DateTime> todayFunc)
        {
            if (todayFunc == null)
                throw new ArgumentNullException("todayFunc");
            _todayFunc = todayFunc;

            _persons = new List<Person>();
            this.Initialize();
        }

        public IList<Person> GetAllPersons()
        {
            return _persons.ToList();
        }

        public IList<Person> FindPersons(ExpressionNode query)
        {
            try
            {
                var expression = query.ToBooleanExpression<Person>();
                return _persons.Where(expression.Compile()).ToList();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
                return new List<Person>();
            }            
        }

        private void Initialize()
        {
            if (_persons.Count > 0)
                return;

            var records = ReadPersonRecords();
            var culture = new CultureInfo("de-DE");
            foreach (var record in records)
            {
                if (string.IsNullOrWhiteSpace(record))
                    continue;

                try
                {
                    var line = record.Split(';');

                    var birthdate = DateTime.Parse(line[3], culture);
                    var deathdate = line[4].Equals("Living", StringComparison.OrdinalIgnoreCase) ? null : (DateTime?)DateTime.Parse(line[4], culture);

                    _persons.Add(this.CreatePerson(_persons.Count + 1, line[1], line[2], birthdate, deathdate, line[6]));
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine("Failed to parse record '{0}': {1}", record, ex.Message);
                }
            }
        }

        private Person CreatePerson(int id, string name, string gender, DateTime birthDate, DateTime? deathDate, string residence)
        {
            var names = name.Split(new[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries);
            var age = this.CalculateAge(birthDate, deathDate);

            return new Person
            {
                Id = id,
                Age = age,
                BirthDate = birthDate,
                DeathDate = deathDate,
                FirstName = names[0],
                LastName = names[1],
                Gender = gender.Trim().ToLowerInvariant() == "f" ? Gender.Female : Gender.Male,
                Residence = residence,
            };
        }

        private int CalculateAge(DateTime birthDate, DateTime? deathDate)
        {
            var endDate = deathDate != null ? deathDate.Value : _todayFunc();
            var age = endDate.Year - birthDate.Year;
            if (birthDate > endDate.AddYears(-age))
                --age;
            return age;
        }

        private static string[] ReadPersonRecords()
        {
            var csvStream = Assembly.GetEntryAssembly().GetManifestResourceStream("Serialize.Linq.Examples.WcfHost.Persons.csv");
            if (csvStream == null)
                throw new InvalidProgramException("Failed to read Persons.");
            using (csvStream)
            {
                using (var reader = new StreamReader(csvStream))
                {
                    var text = reader.ReadToEnd();
                    text = text.Replace("\r", "");
                    return text.Split('\n');
                }
            }
        }
    }
}

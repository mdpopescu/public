using System.Collections.Generic;
using System.Linq;
using EverythingIsADatabase.Logic.Contracts;
using EverythingIsADatabase.Logic.Models;

namespace EverythingIsADatabase.Logic.Implementations
{
    /// <summary>
    /// This is the default database, used for in-memory operations.
    /// </summary>
    public class SessionDatabase : Database
    {
        public SessionDatabase()
        {
            list = new List<Record>();
        }

        public IEnumerable<Record> Search(params Attribute[] attributes)
        {
            return list.Where(r => AreMatching(r.Attributes.ToList(), attributes));
        }

        public IEnumerable<Record> Search(params AttributeMatch[] matches)
        {
            return list.Where(r => AreMatching(r.Attributes.ToList(), matches));
        }

        public void Commit(params Record[] records)
        {
            foreach (var record in records)
                Upsert(record);
        }

        //

        private readonly List<Record> list;

        private void Upsert(Record record)
        {
            list
                .Where(it => it.Id == record.Id)
                .ToList()
                .ForEach(r => list.Remove(r));

            list.Add(record);
        }

        private static bool AreMatching(IList<Attribute> attributes, IEnumerable<Attribute> toMatch)
        {
            return toMatch.All(it => attributes.Any(a => a.Matches(it)));
        }

        private static bool AreMatching(List<Attribute> attributes, IEnumerable<AttributeMatch> toMatch)
        {
            return toMatch.All(it => attributes.Any(a => a.Matches(it)));
        }
    }
}
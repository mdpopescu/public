using System.Collections.Generic;
using EverythingIsADatabase.Logic.Models;

namespace EverythingIsADatabase.Logic.Contracts
{
    public interface Database
    {
        // reading

        /// <summary>
        /// Returns the matching records.
        /// </summary>
        /// <param name="attributes">A list of attributes, optionally with values.</param>
        /// <returns>A list of records matching attribute names; for the attributes with values, the values must also match.</returns>
        /// <remarks>Example: <code>Search(new Attribute("name"), new Attribute("age", 30))</code></remarks>
        IEnumerable<Record> Search(params Attribute[] attributes);

        /// <summary>
        /// Returns the matching records.
        /// </summary>
        /// <param name="matches">A list of attribute predicates.</param>
        /// <returns>A list of records matching attribute names; for the attributes with predicates, those must also match.</returns>
        /// <remarks>Example: <code>Search(new AttributeMatch("age", it => it > 30))</code></remarks>
        IEnumerable<Record> Search(params AttributeMatch[] matches);

        // writing

        /// <summary>
        /// Adds / updates one or more records to the database.
        /// </summary>
        /// <param name="records">The list of records to add or update.</param>
        void Commit(params Record[] records);
    }
}
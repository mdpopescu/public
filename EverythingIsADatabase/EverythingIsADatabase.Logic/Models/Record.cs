using System;
using System.Collections.Generic;

namespace EverythingIsADatabase.Logic.Models
{
    public class Record
    {
        public Guid Id { get; }
        public IEnumerable<Attribute> Attributes { get; }

        public Record(params Attribute[] attributes)
        {
            Id = Guid.NewGuid();
            Attributes = attributes;
        }
    }
}
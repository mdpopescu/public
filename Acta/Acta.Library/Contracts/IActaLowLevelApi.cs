﻿using System;
using System.Collections.Generic;
using Acta.Library.Models;

namespace Acta.Library.Contracts
{
  public interface IActaLowLevelApi
  {
    /// <summary>
    ///   Appends a (guid, name, value) tuple to the log.
    /// </summary>
    /// <param name="guid">The entity guid.</param>
    /// <param name="name">The property name.</param>
    /// <param name="value">The property value.</param>
    /// <remarks>
    ///   This method is used to either create a new entity (if the guid hasn't been used before)
    ///   or to add or update a property to an existing entity.
    /// </remarks>
    void Write(Guid guid, string name, object value);

    /// <summary>
    ///   Atomically appends a set of (guid, name, value) tuples to the log.
    /// </summary>
    /// <param name="guid">The entity guid.</param>
    /// <param name="pairs">The property name/value pairs.</param>
    /// <remarks>
    ///   This method is used to either create a new entity (if the guid hasn't been used before)
    ///   or to add or update a set of properties to an existing entity.
    /// </remarks>
    void Write(Guid guid, IEnumerable<ActaKeyValuePair> pairs);

    /// <summary>
    ///   Retrieves the Ids of entities that have a property with the given value.
    /// </summary>
    /// <param name="name">The property name.</param>
    /// <param name="value">The property value.</param>
    /// <returns>A list of GUIDs.</returns>
    IEnumerable<Guid> GetIds(string name, object value);

    /// <summary>
    ///   Returns a property value from a (guid, name, value) tuple, if one exists.
    /// </summary>
    /// <param name="guid">The GUID.</param>
    /// <param name="name">The property name.</param>
    /// <returns>The property value or <c>null</c> if one cannot be found.</returns>
    object Read(Guid guid, string name);
  }
}
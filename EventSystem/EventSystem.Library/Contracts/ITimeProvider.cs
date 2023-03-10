using System;

namespace EventSystem.Library.Contracts;

public interface ITimeProvider
{
    DateTimeOffset Now { get; }
}
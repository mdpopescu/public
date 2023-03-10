using EventSystem.Library.Contracts;
using System;

namespace EventSystem.Library.Implementations;

public class TimeProvider : ITimeProvider
{
    public DateTimeOffset Now => DateTimeOffset.Now;
}
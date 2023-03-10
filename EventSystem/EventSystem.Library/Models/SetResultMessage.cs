using EventSystem.Library.Contracts;
using System;

namespace EventSystem.Library.Models;

public record SetResultMessage(Guid Id, object Result) : IMessage;
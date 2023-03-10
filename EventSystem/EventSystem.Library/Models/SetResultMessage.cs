using EventSystem.Library.Contracts;
using System;

namespace EventSystem.Library.Models;

internal record SetResultMessage(Guid Id, object Result) : IMessage;
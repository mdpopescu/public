﻿using System;

namespace EventSystem.Library.Contracts;

public interface IMessage
{
    Guid Id { get; }
}
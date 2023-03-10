using EventSystem.Library.Contracts;

namespace Tester.Models;

internal record NullCommand : IMessage;

internal record InitCommand : IMessage;

internal record MoveLeftCommand : IMessage;

internal record MoveRightCommand : IMessage;

internal record DrawCommand : IMessage;

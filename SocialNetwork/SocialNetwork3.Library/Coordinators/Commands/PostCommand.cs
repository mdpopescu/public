﻿using System;
using System.Collections.Generic;
using SocialNetwork3.Library.Logic;
using SocialNetwork3.Library.Models;

namespace SocialNetwork3.Library.Coordinators.Commands
{
    public class PostCommand : Command
    {
        /// <summary>Initializes a new instance of the <see cref="PostCommand"/> class.</summary>
        /// <param name="messages">The message repository.</param>
        public PostCommand(MessageRepository messages)
        {
            this.messages = messages;
        }

        public override List<string> Execute(DateTime time, string user, string argument)
        {
            messages.Add(new Message(time, user, argument));
            return new List<string>();
        }

        //

        private readonly MessageRepository messages;
    }
}
﻿using Elomen.Library.Model;

namespace Elomen.Library.Contracts
{
    public interface Executable
    {
        /// <summary>
        /// Executes a command.
        /// </summary>
        /// <param name="account">The account of the user sending the command.</param>
        /// <param name="commandText">The (natural language) command.</param>
        /// <returns>The result of executing the command (or an error message).</returns>
        string Execute(Account account, string commandText);
    }
}
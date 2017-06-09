using System;
using System.Collections.Generic;
using System.Linq;

namespace Prometheus.Core.Usenet
{
    internal class CommandBuilder
    {
        private readonly List<string> parameters = new List<string>();

        private readonly string command;

        private CommandBuilder(string command)
        {
            this.command = command;
        }

        public void AddParameter<T>(T parameter)
        {
            parameters.Add(parameter.ToString());
        }

        public void AddParameter(string format, params object[] args)
        {
            parameters.Add(string.Format(format, args));
        }

        public void AddDateAndTimeParameters(DateTime date)
        {
            AddParameter(date.ToString("yyyyMMdd"));
            AddParameter(date.ToString("HHmmss"));
        }

        public override string ToString()
        {
            return Enumerable.Any<string>(parameters) ? string.Join(" ", Enumerable.Union<string>(new[] { command }, parameters)) : command;
        }

        public static string Build(string command, Action<CommandBuilder> builder)
        {
            var commandBuilder = new CommandBuilder(command);

            builder(commandBuilder);

            return commandBuilder.ToString();
        }
    }
}
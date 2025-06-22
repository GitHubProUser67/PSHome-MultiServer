using System;

namespace NthDeveloper.TelnetServer
{
    public struct CommandParameter
    {
        public string Name { get; private set; }
        public bool IsRequired { get; private set; }
        public string Description { get; private set; }

        public CommandParameter(string name, bool isRequired, string description)
        {
            this.Name = name;
            this.IsRequired = isRequired;
            this.Description = description;
        }
    }
}

using System;

namespace TestHelpers.DotNetCore.WebApi
{
    public class Header
    {
        public string Name { get; }
        public string Value { get; }

        public Header(string name, string value)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));

            Name = name;
            Value = value;
        }
    }
}
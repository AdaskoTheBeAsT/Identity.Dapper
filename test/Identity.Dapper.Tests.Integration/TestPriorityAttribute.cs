using System;

namespace Identity.Dapper.Tests.Integration
{
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class TestPriorityAttribute : Attribute
    {
        public TestPriorityAttribute(int priority)
        {
            this.Priority = priority;
        }

        public int Priority { get; set; }
    }
}

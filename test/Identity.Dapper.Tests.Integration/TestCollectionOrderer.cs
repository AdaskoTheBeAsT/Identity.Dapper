using System;
using System.Collections.Generic;
using System.Linq;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Identity.Dapper.Tests.Integration
{
    public class TestCollectionOrderer : ITestCaseOrderer
    {
        public const string TypeName = "Identity.Dapper.Tests.Integration.TestCollectionOrderer";
        public const string AssemblyName = "Identity.Dapper.Tests.Integration";

        public IEnumerable<TTestCase> OrderTestCases<TTestCase>(IEnumerable<TTestCase> testCases)
            where TTestCase : ITestCase
        {
            if (testCases is null)
            {
                throw new ArgumentNullException(nameof(testCases));
            }

            var sortedMethods = new SortedDictionary<int, TTestCase>();

            foreach (TTestCase testCase in testCases)
            {
                var attribute = testCase.TestMethod.Method.GetCustomAttributes(typeof(TestPriorityAttribute).AssemblyQualifiedName)
                                                          .FirstOrDefault();
                if (attribute is null)
                {
                    continue;
                }

                var priority = attribute.GetNamedArgument<int>("Priority");
                sortedMethods.Add(priority, testCase);
            }

            return sortedMethods.Values;
        }
    }
}

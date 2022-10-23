using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace IGroceryStore.Shared.Services
{
    public static class EnvironmentService
    {
        public const string TestEnvironment = "Test";
        public static bool IsTestEnvironment(this IHostEnvironment env) => 
            env.EnvironmentName == TestEnvironment;
        public static bool IsEnvironment(this IHostEnvironment env, string environment) =>
            env.EnvironmentName == environment;

    }
}

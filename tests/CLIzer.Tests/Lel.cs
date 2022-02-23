//using CLIzer.Utils;
//using Microsoft.Extensions.DependencyInjection;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace CLIzer.Tests
//{
//    internal class Lel
//    {
//        public void Test()
//        {
//            var clizer = new Clizer();
//            clizer.Configure((config) => config

//                .RegisterCommands((container) => container
//                    .Root<KrustyKrabCmd>())

//                .RegisterServices((services) => services
//                    .AddScoped<DriveThroughService > ())

//                .RegisterMiddleware<KelpShakeMiddleware>()

//                .RegisterConfig<SecretFormularConfiguration>("config", "secret_formula.json")

//                .HandleException((ex) => ConsoleExtensions.WriteColoredLine(ConsoleColor.Red, ex.Message))
//            );
//        }
//    }
//}

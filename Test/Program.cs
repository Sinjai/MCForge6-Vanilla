using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Test {
    class Program {

        private static int Port { get; set; }


        static void Main(string[] args) {

            for ( int i = 0; i < TestSuite.Tests.Count; i++ ) {
                var result = TestSuite.Tests[i];
                Console.WriteLine("Running Test {0} of {1} : {2}", i, TestSuite.Tests.Count, result.TestName);
                Console.WriteLine("Start Time {0} ", DateTime.Now);
                result.RunTest();
                Console.WriteLine("Test Finished {0}", DateTime.Now);
                Console.Write("Result : ");
                if ( result.Result == TestResultEnum.Passed ) {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("Passed" + Environment.NewLine);
                    Console.ForegroundColor = ConsoleColor.White;
                }
                else {
                     Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("Failed" + Environment.NewLine);

                    Console.ForegroundColor = ConsoleColor.Gray;

                    Console.WriteLine(result.ExceptionTrace.Message);
                    Console.WriteLine(result.ExceptionTrace.StackTrace);

                    Console.ForegroundColor = ConsoleColor.White;
                }

            }


            Console.ReadLine();

        }
    }
}

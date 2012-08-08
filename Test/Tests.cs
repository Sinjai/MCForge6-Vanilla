using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Test {
    /// <summary>
    /// A little test suite. Not as good as NUnit, but it will do.
    /// </summary>
    public class TestSuite {

        public static readonly List<Test> Tests;

        static TestSuite() {
            Tests = new List<Test>();
            Tests.Add(TurnOnServer());
        }


        #region Tests

        private static Test TurnOnServer() {
            Test t = new Test();
             t.TestName = "Turn on server";
             t.TestAction = new Action(()=>{
             
                Process.Start("MCForgeConsole.exe");
             
             });
             return t;
        }

     #endregion
    }

    public class Test{

        public Action TestAction {get; set;}
        public string TestName {get; set;}
        
        public Exception ExceptionTrace {get; set;}
        public TestResultEnum Result {get; set;}

        /// <summary>
        /// Runs the test.
        /// </summary>
        public void RunTest(){
            try {
                TestAction();
                Result  = TestResultEnum.Passed;
            }
            catch ( Exception e ) {
                ExceptionTrace = e;
                Result = TestResultEnum.Failed;
            }
        }

    }


    public enum TestResultEnum {
        /// <summary>
        /// Test passed
        /// </summary>
        Passed,


        /// <summary>
        /// The test has failed
        /// </summary>
        Failed
    }


}

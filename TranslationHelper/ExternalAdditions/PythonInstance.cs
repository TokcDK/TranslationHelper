//using IronPython.Hosting;
//using Microsoft.Scripting.Hosting;

///// <summary>
///// https://stackoverflow.com/questions/14139766/run-a-particular-python-function-in-c-sharp-with-ironpython
///// </summary>
//namespace TranslationHelper.ExternalAdditions
//{
//    public class PythonInstance
//    {
//        private readonly ScriptEngine engine;
//        private readonly ScriptScope scope;
//        private readonly ScriptSource source;
//        private readonly CompiledCode compiled;
//        private readonly object pythonClass;

//        public PythonInstance(string code, string className = "PyClass")
//        {
//            //creating engine and stuff
//            engine = Python.CreateEngine();
//            scope = engine.CreateScope();

//            //loading and compiling code
//            source = engine.CreateScriptSourceFromString(code, Microsoft.Scripting.SourceCodeKind.Statements);
//            compiled = source.Compile();

//            //now executing this code (the code should contain a class)
//            compiled.Execute(scope);

//            //now creating an object that could be used to access the stuff inside a python script
//            pythonClass = engine.Operations.Invoke(scope.GetVariable(className));
//        }

//        public void SetVariable(string variable, dynamic value)
//        {
//            scope.SetVariable(variable, value);
//        }

//        public dynamic GetVariable(string variable)
//        {
//            return scope.GetVariable(variable);
//        }

//        public void CallMethod(string method, params dynamic[] arguments)
//        {
//            engine.Operations.InvokeMember(pythonClass, method, arguments);
//        }

//        public dynamic CallFunction(string method, params dynamic[] arguments)
//        {
//            return engine.Operations.InvokeMember(pythonClass, method, arguments);
//        }

//    }
//}

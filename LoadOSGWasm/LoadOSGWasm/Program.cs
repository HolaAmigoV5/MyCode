using Wasmtime;
using System;

EvaluateHelloWorld();

Console.Read();

static void EvaluateHelloWorld()
{
    using var engine = new Engine();
    using var module= Module.FromFile(engine, "hello-emcc.wasm");

    using var linker = new Linker(engine);
    using var store = new Store(engine);

    linker.Define(
               "",
               "hello",
               Function.FromCallback(store, () => Console.WriteLine("Hello from C#, WebAssembly!"))
           );

    var instance = linker.Instantiate(store, module);

    var run = instance.GetFunction(store, "run");
    if (run is null)
    {
        Console.WriteLine("error: run export is missing");
        return;
    }

    run.Invoke(store);
}



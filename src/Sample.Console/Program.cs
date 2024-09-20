using System.Text;
using MassTransit;
using MassTransit.SagaStateMachine;
using MassTransit.Visualizer;
using Sample.Consumer;

// ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator

var projectDirectory = Directory.GetParent(Environment.CurrentDirectory)!.Parent!.Parent!.FullName;

var stateMachineTypes = ConsumerAssembly.Current
    .GetTypes()
    .Where(x => x.GetInterfaces().Contains(typeof(StateMachine)))
    .ToList();

var getGraphExtensionMethod = typeof(GraphStateMachineExtensions)
    .GetMethods()
    .First(x => x is { Name: "GetGraph", IsGenericMethod: true });

var file = Path.Join(projectDirectory, "state-machine.md");
await using var fs = File.Open(file, FileMode.Create);

foreach (var stateMachineType in stateMachineTypes)
{
    var name = stateMachineType.FullName;

    Console.WriteLine("Generating state machine diagram for {0}", name);

    var sagaStateMachine = Activator.CreateInstance(stateMachineType)!;
    var sagaType = stateMachineType.BaseType!.GetGenericArguments().First();
    var graph = getGraphExtensionMethod
        .MakeGenericMethod(sagaType)
        .Invoke(null, [sagaStateMachine]) as StateMachineGraph;
    var generator = new StateMachineMermaidGenerator(graph);
    var mermaid = generator.CreateMermaidFile();

    var content = $"""
                  ### {name}

                  ``` mermaid
                  {mermaid}
                  ```

                  """;

    await fs.WriteAsync(Encoding.UTF8.GetBytes(content));
    await fs.FlushAsync();
}

Console.WriteLine($"State machine diagrams written to {file}");

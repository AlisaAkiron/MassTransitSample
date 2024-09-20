using System.Reflection;

namespace Sample.Consumer;

public static class ConsumerAssembly
{
    public static Assembly Current => typeof(ConsumerAssembly).Assembly;
}

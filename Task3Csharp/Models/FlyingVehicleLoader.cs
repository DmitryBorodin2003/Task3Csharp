using System.Reflection;

namespace Task3CSharp.Models;

public class FlyingVehicleLoader
{
    private const string _targetClassName = "FlyingVehicle";

    public Assembly LoadedAssembly { get; private set; }
    public List<Type> FlyingVehiclesTypes { get; private set; }

    public void LoadAssembly(string path)
    {
        LoadedAssembly = Assembly.LoadFrom(path);
        FlyingVehiclesTypes = LoadedAssembly.GetTypes()
            .Where(t => (t.BaseType.FullName != null && t.BaseType!.FullName!.Contains(_targetClassName)) && !t.IsAbstract)
            .ToList();
    }

    public List<MethodInfo> GetMethods(Type type)
    {
        return type.GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(m => m.DeclaringType == type)
            .ToList();
    }
}
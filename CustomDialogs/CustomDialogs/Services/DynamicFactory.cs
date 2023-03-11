using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;

namespace CustomDialogs.Services;

public class DynamicFactory
{
    public static dynamic Create(Type type) => new DynamicFactory(type).CreateInstance();
    public static dynamic Create(Assembly assembly, string typeName) => Create(assembly.GetType(typeName) ?? throw new Exception("Type not found."));

    public DynamicFactory(Type type)
    {
        methodHandler = CreateMethod(type.GetConstructor(Type.EmptyTypes) ?? throw new Exception("Constructor not found."));
    }

    public DynamicFactory(ConstructorInfo target)
    {
        methodHandler = CreateMethod(target);
    }

    public dynamic CreateInstance()
    {
        var result = methodHandler();
        Debug.Assert(result != null);
        return result;
    }

    //

    private delegate object MethodInvoker();

    private readonly MethodInvoker methodHandler;

    private static MethodInvoker CreateMethod(ConstructorInfo target)
    {
        Debug.Assert(target.DeclaringType != null, "target.DeclaringType != null");

        var dynamic = new DynamicMethod(string.Empty, typeof(object), Type.EmptyTypes, target.DeclaringType);
        var il = dynamic.GetILGenerator();
        il.DeclareLocal(target.DeclaringType);
        il.Emit(OpCodes.Newobj, target);
        il.Emit(OpCodes.Stloc_0);
        il.Emit(OpCodes.Ldloc_0);
        il.Emit(OpCodes.Ret);

        return (MethodInvoker)dynamic.CreateDelegate(typeof(MethodInvoker));
    }
}
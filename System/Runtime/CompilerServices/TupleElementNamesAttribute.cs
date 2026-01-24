using System.Collections.Generic;

namespace System.Runtime.CompilerServices;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.ReturnValue | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Event)]
public sealed class TupleElementNamesAttribute : Attribute
{
    private readonly string?[] transformNames;

    public IList<string?> TransformNames => transformNames;

    public TupleElementNamesAttribute(string?[] transformNames)
    {
        if (transformNames == null)
            throw new ArgumentNullException();

        this.transformNames = transformNames;
    }
}

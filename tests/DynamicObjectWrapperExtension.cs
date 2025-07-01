using System.Collections;
using System.ComponentModel;
using System.Dynamic;
using System.Reflection;

namespace CartonCaps.Tests;

public static class DynamicObjectWrapperExtension
{
    /// <summary>
    /// Return provided object as a <seealso cref="System.Dynamic.DynamicObject"/>
    /// </summary>  
    public static dynamic AsDynamicObject(this object value)
    {
        return new DynamicObjectWrapper(value);
    }
}

#pragma warning disable CS8604 // Possible null reference argument.
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
public class DynamicObjectWrapper : DynamicObject
{
    private readonly object value;
    private readonly Type valueType;

    public DynamicObjectWrapper(object value)
    {
        this.value = value;
        this.valueType = value.GetType();
    }

    public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result)
    {
        result = null;
        //1d collection
        if (potentialIndex(indexes))
        {
            int index = (int)indexes[0];
            var list = value as IList;
            if (validIndex(index, list))
            {
                result = checkValue(list[index]);
                return true;
            }
        }
        return false;
    }

    private bool validIndex(int index, IList list)
    {
        return index >= 0 && index < list.Count;
    }

    private bool potentialIndex(object[] indexes)
    {
        return indexes[0] != null && typeof(int) == indexes[0].GetType() && value is IList;
    }

    public override bool TryGetMember(GetMemberBinder binder, out object result)
    {
        return TryGetValue(binder.Name, out result);
    }

    public bool TryGetValue(string propertyName, out object result)
    {
        result = null;
        var property = valueType.GetProperty(propertyName);
        if (property != null)
        {
            var propertyValue = property.GetValue(value, null);
            result = checkValue(propertyValue);
            return true;
        }
        return false;
    }

    private object checkValue(object value)
    {
        var valueType = value.GetType();
        return isAnonymousType(valueType)
            ? new DynamicObjectWrapper(value)
            : value;
    }

    private bool isAnonymousType(Type type)
    {
        //HACK: temporary hack till a proper function can be implemented
        return type.Namespace == null &&
            type.IsGenericType &&
            type.IsClass &&
            type.IsSealed &&
            type.IsPublic == false;
    }
}
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
#pragma warning restore CS8604 // Possible null reference argument.
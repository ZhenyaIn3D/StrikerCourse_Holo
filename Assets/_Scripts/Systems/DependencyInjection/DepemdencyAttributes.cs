using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field| AttributeTargets.Method| AttributeTargets.Property)]
public sealed class InjectAttribute : Attribute
{
}

[AttributeUsage(AttributeTargets.Method)]
public sealed class ProvideAttribute : Attribute
{
}

using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field)]
public class ExposeField : PropertyAttribute
{
   public string name { get; }
   public ExposeField()
   {
      
   }
   public ExposeField(string name)
   {
      this.name = name;
   }
}

using System;

[AttributeUsage(AttributeTargets.Method)]
public class ButtonAttribute : Attribute
{
    public string buttonLabel { get; }
    public object[] value { get; }
  

    public ButtonAttribute(string buttonLabel)
    {
        this.buttonLabel = buttonLabel;
    }
    public ButtonAttribute(string buttonLabel,params object[] values)
    {
        this.buttonLabel = buttonLabel;
        value = new object[values.Length];
        for (var index = 0; index < values.Length; index++)
        {
            value[index] = values[index];
        }
    }
   
}
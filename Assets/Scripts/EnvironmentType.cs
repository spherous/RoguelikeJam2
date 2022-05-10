using System;

public enum EnvironmentType {None = 0, }

public static class EnvironmentTypeExtensions
{
    public static Action GetEffect(this EnvironmentType type) => type switch{
          
        _ => null
    };
}
// This file is release under EUPL_v1.2 license. See LICENSE.md

namespace TracerLib;

public static class Functions
{
    /// <summary>
    /// Determines whether two floating-point values are approximately equal
    /// within a given tolerance.
    /// </summary>
    /// <param name="a">First value.</param>
    /// <param name="b">Second value.</param>
    /// <param name="epsilon">Maximum allowed difference between the two values.</param>
    /// <returns>
    /// True if the absolute difference between a and b is less than epsilon; otherwise false.
    /// </returns>
    public static bool AreClose(float a, float b, float epsilon = 1e-5f)
    {
        return MathF.Abs(a - b) < epsilon;
    }
    
    /// <summary>
    /// Determines whether two arrays of floats are approximately equal
    /// element by element within a given tolerance.
    /// </summary>
    /// <param name="a">First array.</param>
    /// <param name="b">Second array.</param>
    /// <param name="epsilon">Maximum allowed difference between corresponding elements.</param>
    /// <returns>
    /// True if both arrays have the same length and all corresponding elements
    /// differ by less than epsilon; otherwise false.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when the input arrays have different lengths.
    /// </exception>
    public static bool AreArraysClose(float[] a, float[] b, float epsilon = 1e-5f)
    {
        int length = a.Length;
        if (length != b.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(b), b, "the "+ nameof(b)+ " array has not the same length as the "+nameof(a)+ " array");
        }
        bool areArrayClose = true;
        for (int i = 0; i < length; i++)
        {
            areArrayClose = areArrayClose
                             && Functions.AreClose(a[i], b[i], epsilon);
        }

        return areArrayClose;
    }

    /// <summary>
    /// Converts degrees to radians using the formula: radians = degrees * π / 180.
    /// </summary>
    /// <param name="deg">Angle in degrees.</param>
    /// <returns>Angle in radians.</returns>
    public static float DegToRad(float deg)
    {
        return deg * MathF.PI / 180f;
    }
    
    /// <summary>
    /// Parses an array of variable definitions and converts them into a dictionary.
    /// Each definition must follow the format "NAME:VALUE", where NAME is the
    /// variable identifier and VALUE is a valid floating-point number.
    /// </summary>
    /// <param name="definitions">
    /// Array of string definitions following the pattern "NAME:VALUE".
    /// Each element represents a variable declaration.
    /// </param>
    /// <returns>
    /// A dictionary where each key is the variable name and each value is the
    /// corresponding floating-point value.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Thrown when a definition does not follow the "NAME:VALUE" format or when
    /// the provided value cannot be parsed as a floating-point number.
    /// </exception>
    public static Dictionary<string, float> VariableTable(string[] definitions)
    {
        var variables = new Dictionary<string, float>();

        foreach (string declaration in definitions)
        {
            string[] parts = declaration.Split(':');

            if (parts.Length != 2)
            {
                throw new ArgumentException($"The definition {declaration} doesn't follow the pattern NAME:VALUE");
            }

            string name = parts[0];

            if (!float.TryParse(parts[1], out float result))
            {
                throw new ArgumentException($"Invalid floating point value {parts[1]}");
            }

            variables[name] = result;
        }

        return variables;
    } 
}
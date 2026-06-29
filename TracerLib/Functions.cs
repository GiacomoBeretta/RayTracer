// This file is release under EUPL_v1.2 license. See LICENSE.md

namespace TracerLib;

public static class Functions
{
    /// <summary>
    /// Ensures that the specified value is greater than the given threshold.
    /// Throws an <see cref="ArgumentOutOfRangeException"/> if the condition is not met.
    /// </summary>
    /// <typeparam name="T">A comparable type used for threshold validation.</typeparam>
    /// <param name="value">The value to validate.</param>
    /// <param name="paramName">The name of the parameter used in the exception message.</param>
    /// <param name="threshold">The exclusive lower bound.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="value"/> is less than or equal to <paramref name="threshold"/>.
    /// </exception>
    public static void EnsureGreaterThan<T>(T value, string paramName, T threshold) where T : IComparable<T>
    {
        if (value.CompareTo(threshold) < 0)
        {
            throw new ArgumentOutOfRangeException(paramName, value, $"{paramName} must be greater than {threshold}");
        }
    }

    /// <summary>
    /// Ensures that the specified value is greater than or equal to the given threshold.
    /// Throws an <see cref="ArgumentOutOfRangeException"/> if the condition is not met.
    /// </summary>
    /// <typeparam name="T">A comparable type used for threshold validation.</typeparam>
    /// <param name="value">The value to validate.</param>
    /// <param name="paramName">The name of the parameter used in the exception message.</param>
    /// <param name="threshold">The inclusive lower bound.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="value"/> is less than <paramref name="threshold"/>.
    /// </exception>
    public static void EnsureGreaterThanOrEqual<T>(T value, string paramName, T threshold) where T : IComparable<T>
    {
        if (value.CompareTo(threshold) < 0)
        {
            throw new ArgumentOutOfRangeException(paramName, value,
                $"{paramName} must be greater than or equal to {threshold}");
        }
    }

    /// <summary>
    /// Ensures that the specified value is within the inclusive range [min, max].
    /// Throws an <see cref="ArgumentOutOfRangeException"/> if the condition is not met.
    /// </summary>
    /// <typeparam name="T">A comparable type used for range validation.</typeparam>
    /// <param name="value">The value to validate.</param>
    /// <param name="paramName">The name of the parameter used in the exception message.</param>
    /// <param name="min">The inclusive lower bound.</param>
    /// <param name="max">The inclusive upper bound.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="value"/> is less than <paramref name="min"/> or greater than <paramref name="max"/>.
    /// </exception>
    public static void EnsureInRange<T>(T value, string paramName, T min, T max) where T : IComparable<T>
    {
        if (value.CompareTo(min) < 0 || value.CompareTo(max) > 0)
        {
            throw new ArgumentOutOfRangeException(paramName, value, $"{paramName} must be in [{min}, {max}]");
        }
    }

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
            throw new ArgumentOutOfRangeException(nameof(b), b,
                "the " + nameof(b) + " array has not the same length as the " + nameof(a) + " array");
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
    public static Dictionary<string, float> ParseVariableTable(string[] definitions)
    {
        Dictionary<string, float> variables = new Dictionary<string, float>();

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
///
/// This file is release under ... license. See LICENSE.md
/// 

using TracerLib;

// sul libro c'è scritto
// You can handle multiple exception types with multiple catch clauses (again, this
// example could be written with explicit argument checking rather than exception
// handling)
// esempio:
// try
// {
//  int y = Calc (0);
//  Console.WriteLine (y);
// }
// catch (DivideByZeroException ex)
// {
//  Console.WriteLine ("x cannot be zero");
// }
// Console.WriteLine ("program completed");
// int Calc (int x) => 10 / x;
// This is a simple example to illustrate exception handling. We
// could deal with this particular scenario better in practice by
// checking explicitly for the divisor being zero before calling
// Calc.
// Checking for preventable errors is preferable to relying on
// try/catch blocks because exceptions are relatively expensive
// to handle, taking hundreds of clock cycles or more.

//allora non ho capito quando usare le eccezioni

public static class RayTracer
{
    public static int Main(string[] args)
    {
        if (args.Length != 4)
        {
            Console.WriteLine("Usage: main input_file.pfm a_factor gamma output_file.png");
            return 1;
        }
        Console.WriteLine($"{args[0]}, {args[1]},{args[2]},{args[3]}, {args[4]}");
        try
        {
            string inputFileName = args[1];
            float factor = float.Parse(args[2]);
            float gamma = float.Parse(args[3]);
            string outputFileName = args[4];
        }
        catch (ArgumentException ex)
        {
            throw new ArgumentException("Usage: main input_file.pfm a_factor gamma output_file.png\n" +
                                        "the a_factor and gamma must be floating numbers.", ex);
        }
        return 0;
    }
}
/*///////////////////////inizio prova Giacomo
Color[,] colorMatrix = new Color[2, 3];

for (int i = 0; i < colorMatrix.GetLength(0); i++)
{
    for (int j = 0; j < colorMatrix.GetLength(1); j++)
    {
        float red = (i * colorMatrix.GetLength(1) + j);
        float green = (i * colorMatrix.GetLength(1) + j) * 2;
        float blue = (i * colorMatrix.GetLength(1) + j) * 3;
        colorMatrix[i, j] = new Color(red, green, blue);
    }
}

Color[] colors = new Color[6];
for (int i = 0; i < 2; i++)
{
    for (int j = 0; j < 3; j++)
    {
        colors[i * 3 + j] = colorMatrix[i, j];
    }
}

HDRImage image = new HDRImage(2, 3, colors);
image.Print();
Console.WriteLine(image);

image[^1].Print();
Console.WriteLine();
image[1, 1].Print();

Color color1 = new Color(387, 129, 530);
image[0] = color1;
image.Print();
//////////////////////////////fine prova Giacomo*/ 


/*
string prova = "2 3";
try
{
    int w, h;
    HDRImage._parse_img_size(prova, out w, out h);

}
catch (ArgumentException err1)
{
    Console.WriteLine(err1.Message);
}
*/
//Console.WriteLine($"Prova: {w}x{h}");




///////////////////////////////////////////////////////////////
//Simone's Main:
/*
using System;
using Colors;
using Hdr;
using Exception;

class Program
{
    static void Main()
    {
        Console.WriteLine("Hello world!");

        /* try
         {
             var x = 5.0f;
             var y = 1.0f;
             var result = x / y;
             if (y == 0)
             {
                 throw new ZeroDivision("Impossibile dividere per zero!");
             }
             Console.WriteLine($"Il risultato della tua operazione è: {result}");
         }
         catch (ZeroDivision er)
         {
             Console.WriteLine(er.Message);
         }
         */




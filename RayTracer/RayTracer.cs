///
/// This file is release under ... license. See LICENSE.md
/// 

using TracerLib;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats; //for the Rgb24 Pixel Format

public static class RayTracer
{
    public static string InputFileName = "";
    public static string OutputFileName = "";
    public static float AFactor = 1.0f;
    public static float Gamma = 1.0f;
    
    public static int Main(string[] args)
    {
        /*
         * Tomasi qui ha scritto così
           try:
               parameters.parse_command_line(argv)
           except RuntimeError as err:
               print("Error: ", err)
               return
            ma non ho capito perché mettere un try except. Tanto se non funziona verrà lanciata l'eccezione comunque
            e il programma andrà in crash lo stesso, no?
         */
        ParseArgs(args);
        
        /* suggerimento della lezione 4 in python
         * with open(parameters.input_pfm_file_name, "rb") as inpf:
               img = hdrimages.read_pfm_image(inpf)

           print(f'File "{parameters.input_pfm_file_name}" has been read from disk.')

           img.normalize_image(factor=parameters.factor)
           img.clamp_image()

           # Same as above: use try…except to produce a human-readable message
           # if something goes wrong
           with open(parameters.output_png_file_name, "wb") as outf:
               img.write_ldr_image(stream=outf, format="PNG", gamma=parameters.gamma)

           print(f'File "{parameters.output_png_file_name}" has been written to disk.')
         */
         
        //suggerimento della lezione 4 in c#
         // Create a sRGB bitmap
           var bitmap = new Image<Rgb24>(Configuration.Default, width, height);

           // The bitmap can be used as a matrix. To draw the pixels in the bitmap
           // just use the syntax "bitmap[x, y]" like the following:
           bitmap[SOMEX, SOMEY] = new Rgb24(255, 255, 128); // Three "Byte" values!

           // Save the bitmap as a PNG file
           using (Stream fileStream = File.OpenWrite("output.png")) {
               bitmap.Save(fileStream, new PngEncoder());
           }
         */
        return 0;
    }
    
    public static void ParseArgs(string[] args)
    {
        if (args.Length != 4)
        {
            throw new ArgumentOutOfRangeException(nameof(args),
                "Usage: main inputFileName.pfm aFactor gamma outputFileName.png");
        }
        
        InputFileName = args[0];
        
        if(!Single.TryParse(args[1], out AFactor))
        {
            throw new ArgumentException($"Invalid aFactor ('{args[1]}'). It must be a floating number.", nameof(AFactor));
        }

        if (!Single.TryParse(args[2], out Gamma))
        {
            throw new ArgumentException($"Invalid gamma ('{args[2]}'). It must be a floating number.", nameof(Gamma));

        }

        OutputFileName = args[3];
        
        Console.WriteLine($"I parametri passati sono: InputFileName={args[0]}, aFactor={args[1]}, gamma={args[2]}, OutputFileName={args[3]}");
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
///
/// This file is release under ... license. See LICENSE.md
///

namespace TracerLib;

using System.Diagnostics; //For the debug.assert
using System.Text; //for the Encoding.ASCII.GetBytes
/*
 * using Colors;
   using Exception;
 */

/// <summary>
/// An HDRImage is essentially a matrix of colors with RGB floats (see the Color class)
/// with a Width and a Height of type integer and a one dimensional (for efficient reason) vector called Pixels
/// Attention: the matrix elements are indexed by giving first the colums and then the row!
/// </summary>
public class HDRImage
{
    //provare a mettere delle verifiche sulle funzioni get e set
    //per verificare per esempio che RGB siano positivi e che get
    //e set pixel verifichino che row e column siano positivi con
    //la funzione validCoordinates. E vedere se il programma non
    //rallenta troppo


    //non sapevo se tenere i membri privati con le funzioni get e set
    //oppure usare le proprietà pubbliche che forse è più o meno la stessa cosa
    //ma si scrivono meno righe di codice.
    //Forse però con le proprietà come le ho scritte io non si possono mettere i controlli.

    /*private int _width;
    private int _height;
    private Color[] _pixels;
    //List<Color> colors;

    //get and set functions
    public int Width
    {
        get => _width;
        set => _width = value;
    }

    public int Height
    {
        get => _height;
        set => _height = value;
    }

    public Color[] Pixels
    {
        get => _pixels;
        set => _pixels = value;
    }*/

    //Variables HDR image
    public int Width { get; set; }
    public int Height { get; set; }
    public Color[]? Pixels { get; set; } //Controllare nullable

    //con i controlli invece viene
    /*private int width;
     private height;
     private Color[] pixels;

     public int Width{
        get{return width;}
        if(value < 0){
            throw new ArgumentException("the width must be >= 0")
        }
        set{}
     }

     */

    /// <summary>
    /// Checks for the validity of the coordinates
    /// </summary>
    /// <param name="column"></param>
    /// <param name="row"></param>
    /// <returns></returns>
    public bool _AreCoordinatesValid(int column, int row)
    {
        Console.WriteLine("ok2");
        Console.WriteLine("column: {0}, row: {1}", column, row);
        Console.WriteLine("Width: {0}, Height: {1}", Width, Height);
        return column >= 0 && column < Width && row >= 0 && row < Height;
    }

    //index and range for the pixels 1D vector with the type indexer
    public Color this[Index index]
    {
        get => Pixels[index];
        set => Pixels[index] = value;
    }

    public Color[] this[Range range] => Pixels[range];

    /// <summary>
    /// gives the index integer for the one dimensional vector, given the column and row of the corresponding matrix
    /// </summary>
    /// <param name="column"></param>
    /// <param name="row"></param>
    /// <returns></returns>
    public int _PixelOffset(int column, int row)
    {
        Debug.Assert(_AreCoordinatesValid(column, row));
        Console.WriteLine("ok");
        return row * Width + column;
    }


    //così non si riesce a mettere l'assert
    /// <summary>
    /// gives the Color at the indexes (column, row) of the corresponding matrix
    /// </summary>
    /// <param name="column"></param>
    /// <param name="row"></param>
    public Color this[int column, int row]
    {
        //Debug.Assert(_AreCoordinatesValid(column,row));
        get => Pixels[_PixelOffset(column, row)];
        set => Pixels[_PixelOffset(column, row)] = value;
    }

    //Begin - Constructors 
    public HDRImage(int width, int height)
    {
        Width = width;
        Height = height;
        Pixels = new Color[Width * Height];
    }

    public HDRImage(int width, int height,
        in Color[] colorVector) //capire come mai senza aver messo public allo struct color dava errore
    {
        Debug.Assert(colorVector.Length == width * height);
        Width = width;
        Height = height;
        Pixels = colorVector;
    }

    /*costruttore che prende una matrice con *due* indici
     da rifinire i controlli
    public HDRImage(int width, int height,
        in Color[,] colorMatrixColumnsPerRows)
    {
        Debug.Assert(color.Length == width * height); //da mettere nel main?
        _width = width;
        _height = height;
        _pixels = new Color[width * height];

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                _pixels[Offset(i, j)] = colorMatrixColumnsPerRows[i, j];
            }
        }
    }
    */

    //Costruttore immagine Hdr a partire da una stream
    public HDRImage(Stream stream)
    {
        // read_pfm_file(stream);
    }

    //Costruttore immagine Hdr a partire da un file
    public HDRImage(string fileName)
    {
        using (Stream filestream = File.OpenRead(fileName))
        {
            // read_pfm_file(filestream);
        }
    }

    //End - Constructors

    /// <summary>
    /// prints the color matrix
    /// </summary>
    public void Print()
    {
        Console.WriteLine("Height: {0}, Width: {1}", Height, Width);
        Console.WriteLine("Pixel's matrix:");
        Console.WriteLine("\tColumns ->");
        Console.Write("Rows");
        for (int j = 0; j < Width; j++)
        {
            Console.Write($"\t{j}");
        }

        Console.WriteLine();
        for (int i = 0; i < Height; i++)
        {
            Console.Write($"{i}\t");
            for (int j = 0; j < Width; j++)
            {
                Pixels[_PixelOffset(j, i)].Print();
                Console.Write("\t");
            }

            Console.WriteLine("");
        }
    }

    /*//io preferisco che le funzioni vengano chiamate are... o is... ma è questione di gusto se
     //non sei d'accordo va bene anche _ValidCoord. Meglio togliere i trattini bassi tra le parole
     //se vogliamo seguire la convenzione. Almeno io ho capito così cercando su internet.
    public bool _Valid_coord( int x, int y)
    {
        return x >= 0 && x < Width && y >= 0 && y < Height;
    }*/

    //cambiare il tipo di eccezione
    /// <summary>
    /// returns the values of width and height (as reference values) that were written in the string stringImgSize 
    /// </summary>
    /// <param name="stringImgSize"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <exception cref="ArgumentException"></exception>
    public static void _ParseImgSize(in string stringImgSize, out int width, out int height)
    {
        string[] stringSizeArray = stringImgSize.Split(" ");
        if (stringSizeArray.Length != 2)
        {
            throw new InvalidPfmFileFormat(
                "there isn't the right number of sizes: there must be two sizes width and height.");
        }

        try
        {
            width = int.Parse(stringSizeArray[0]);
            height = int.Parse(stringSizeArray[1]);
        }
        catch (ArgumentException ex)
        {
            throw new InvalidPfmFileFormat("The size is not made of two ints.", ex);
        }

        if (width < 0)
        {
            throw new InvalidPfmFileFormat("width must be greater than zero.");
        }

        if (height < 0)
        {
            throw new InvalidPfmFileFormat("height must be greater than zero.");
        }
    }

    //rivedere se restituire un float o un int
    /// <summary>
    /// returns the endianness written in the string stringEndianness as 1.0 or -1.0
    /// </summary>
    /// <param name="stringEndianness"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public static int _ParseEndianness(string stringEndianness)
    {
        int endian;
        try
        {
            endian = (int)float.Parse(stringEndianness);
        }
        catch (FormatException ex)
        {
            throw new InvalidPfmFileFormat(ex.Message);
        }

        if (endian != 1 && endian != -1)
        {
            throw new InvalidPfmFileFormat("The endianness must be written as 1.0 or -1.0");
        }

        return endian;
    }


    public float ReadFloat(Stream stream, bool bigEndian = true)
    {
        var value = new byte[4];
        var totalRead = 0;

        while (totalRead < 4)
        {
            var bytesRead = stream.Read(value, totalRead, 4 - totalRead);

            if (bytesRead == 0)
            {
                throw new InvalidPfmFileFormat("Impossibile leggere i file binari dai dati");
            }

            totalRead += bytesRead;
        }

        if (BitConverter.IsLittleEndian != bigEndian)
        {
            Array.Reverse(value);
        }

        return BitConverter.ToSingle(value, 0);
    }

    public static void WriteFloat(Stream outputstream, float value)
    {
        var seq = BitConverter.GetBytes(value);
        outputstream.Write(seq, 0, seq.Length);
    }

    /* public HdrImage read_pfm_file(Stream stream)
     {
         StreamReader sr = new StreamReader(stream);
         var magic = sr.ReadLine();
         if (magic != "PF")
         {
             throw new InvalidPfmFileFormat("Invalid magic in PFM file");
         }

         var imgSize = sr.ReadLine();
         var width, height = _parse_img_size(imgSize);

         var endiannessLine = sr.ReadLine();
         var endianness = _parse_endianness(endiannessLine);

         var result = new HdrImage(width, height);
         var color = new Color();
         for (int i = height-1; i >= 0; i--){
             for (int j = 0; j <= width; j++){
                 color.R = ReadFloat(stream, endianness);
                 color.G = ReadFloat(stream, endianness);
                 color.B = ReadFloat(stream, endianness);
                 result.Set_pixel(j, i, color);
             }
         }

         return result;
     } */

    public static void write_pfm_file(HDRImage img, double endian, string filename)
    {
        using (Stream filestream = File.OpenWrite(filename))
        {
            var header = Encoding.ASCII.GetBytes($"PF\n{img.Width} {img.Height}\n{endian}\n");
            filestream.Write(header, 0, header.Length);

            for (int i = img.Height - 1; i >= 0; i--)
            {
                for (int j = 0; j <= img.Width; j++)
                {
                    var color = img[j, i];
                    WriteFloat(filestream, color.R);
                    WriteFloat(filestream, color.G);
                    WriteFloat(filestream, color.B);
                }
            }
        }
    }

    // Oveloading write_pfm con stream
    public static void write_pfm_file(HDRImage img, double endian, Stream filestream)
    {
        var header = Encoding.ASCII.GetBytes($"PF\n{img.Width} {img.Height}\n{endian}\n");
        filestream.Write(header, 0, header.Length);

        for (int i = img.Height - 1; i >= 0; i--)
        {
            for (int j = 0; j <= img.Width; j++)
            {
                var color = img[j, i];
                WriteFloat(filestream, color.R);
                WriteFloat(filestream, color.G);
                WriteFloat(filestream, color.B);
            }
        }
    }

    /// <summary>
    /// normalizes the RGB values of each pixel by the average luminosity computed by the AverageLuminosity function
    /// and by another empirical number (here called factor).
    /// The int luminosityFunction tells which of function of the color class to use to compute the luminosity of the pixel
    /// </summary>
    /// <param name="factor"></param>
    /// <param name="averageLuminosity"></param>
    /*public void _Normalize(float factor, int luminosityFunction, float? averageLuminosity = null, float delta = 1e-10f)
    {
        //if averageLuminosity is null compute it with the _AverageLuminosity function
        averageLuminosity ??= _AverageLuminosity(luminosityFunction, delta);
        foreach (Color color in Pixels)
        {
            color = color * (factor / averageLuminosity);
        }
    }*/

    //(Giacomo)
    /// <summary>
    /// Computes the average luminosity of the entire image,
    /// using a particular luminosity function of the Color class based on the value of the int luminosityFunction parameter
    /// </summary>
    /// <param name="luminosityFunction"></param>
    /// <param name="delta"></param>
    /// <returns></returns>
    public float _AverageLuminosity(int luminosityFunction, float delta = 1e-10f)
    {
        float sum = 0;
        //perceived value of luminosity follows a logarithmic scale
        //so we must use a logarithmic average
        //delta is needed to avoid singular values for the logarithm
        if (luminosityFunction == 0)
        {
            foreach (Color color in Pixels)
            {
                sum += MathF.Log10(delta + color.LuminosityShirleyMorley());
            }

            return MathF.Pow(10, sum / Pixels.Length);
        }
        else //(luminosityFunction == 1)
        {
            foreach (Color color in Pixels)
            {
                sum += MathF.Log10(delta + color.LuminosityWeightedAverage());
            }

            return MathF.Pow(10, sum / Pixels.Length);
        }
    }

    /// <summary>
    /// returns the corresponding LDR image
    /// </summary>
    /// <param name="gamma"></param>
    public HDRImage ToLDR(float gamma)
    {
        HDRImage image = new HDRImage(Width, Height, Pixels); //forse è meglio usare un copy constructor?
        for (int i = 0; i < Pixels.Length; i++)
        {
            image[i] = Pixels[i].To8BitRGB(gamma);
        }

        return image;
    }
}
// This file is release under EUPL_v1.2 license. See LICENSE.md

// Implementare dei controlli per i constructor e in altre funzioni se necessario, vediamo cosa dice Tomasi in proposito.
// Forse si possono mettere i membri privati e rendere la classe dei test una friend?

using System.Diagnostics.CodeAnalysis; // per sopprimere i messaggi di errore
using
    System.Globalization; //per il metodo cultureInfo e quindi per risolvere il problema dell'1.0 che viene letto come 10
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats; //for the Rgb24 Pixel Format

namespace TracerLib;

using System.Text; //for the Encoding.ASCII.GetBytes

/// <summary>
/// Represents an HDR image stored as a 1D array of RGB float colors.
/// Attention: the matrix elements are indexed by giving first the column and then the row!
/// (left to right, top to bottom).
/// </summary>
[SuppressMessage("ReSharper", "InconsistentNaming")]
public class HDRImage
{
    //Provare a mettere delle verifiche sulle funzioni get e set
    //per verificare per esempio che RGB siano positivi e che get
    //e set pixel verifichino che row e column siano positivi con
    //la funzione validCoordinates. E vedere se il programma non
    //rallenta troppo

    //Variables HDR image

    /// <summary>
    /// Width of the matrix of pixels.
    /// </summary>
    public int Width { get; private set; }

    /// <summary>
    /// Height of the matrix of pixels.
    /// </summary>
    public int Height { get; private set; }

    /// <summary>
    /// The array of <see cref="Color"/>s that make up the HDR image.
    /// </summary>
    public Color[] Pixels { get; set; } //Controllare nullable (Color[])?

    //con i controlli invece viene
    /*
     private int width;
     private height;
     private Color[] pixels;

     public int Width{
        get{return width;}
        if(value < 0){
            throw new ArgumentException("the width must be >= 0")
        }
        set{}
     }*/

    //avendo implementato checkCoordinates non so se è più utile areCoordinatesValid
    /// <summary>
    /// Checks for the validity of the coordinates
    /// </summary>
    /// <param name="column">Image's width</param>
    /// <param name="row">Image's height</param>
    /// <returns></returns>
    public bool _AreCoordinatesValid(int column, int row)
    {
        return column >= 0 && column < Width && row >= 0 && row < Height;
    }

    /// <summary>
    /// Validates that the specified coordinates are within the range
    /// [0, Width) and [0, Height).
    /// </summary>
    /// <param name="column">The column index to validate.</param>
    /// <param name="row">The row index to validate.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the coordinates fall outside the image bounds.</exception>
    public void _ValidateCoordinates(int column, int row)
    {
        if (column < 0 || column >= Width)
        {
            throw new ArgumentOutOfRangeException(nameof(column), column,
                nameof(column) + $" must be non-negative and less than {Width}");
        }

        if (row < 0 || row >= Height)
        {
            throw new ArgumentOutOfRangeException(nameof(row), row,
                nameof(row) + $" must be non-negative and less than {Height}");
        }
    }

    /// <summary>
    /// Validates that width and height are greater than zero.
    /// </summary>
    /// <param name="width">The image width.</param>
    /// <param name="height">The image height.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when width or height is negative.
    /// </exception>
    public static void _CheckWidthHeight(int width, int height)
    {
        if (width <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(width), width, nameof(width) + " must be greater than zero");
        }

        if (height <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(height), height,
                nameof(height) + " must be greater than zero");
        }
    }

    /// <summary>
    /// Validates that the pixel array is not null and its length matches
    /// the expected image size (width × height).
    /// </summary>
    /// <param name="width">The image width.</param>
    /// <param name="height">The image height.</param>
    /// <param name="colorVector">The pixel data array.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="colorVector"/> is null.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown when the length of <paramref name="colorVector"/> does not match width × height.
    /// </exception>
    public static void _CheckPixels(int width, int height, in Color[] colorVector)
    {
        ArgumentNullException.ThrowIfNull(colorVector);
        if (colorVector.Length != width * height)
        {
            throw new ArgumentException("the Length of the colorVector and the width and height passed do not match",
                nameof(colorVector));
        }
    }

    //vedere se anche per questo indice si possono mettere dei controlli
    //index and range for the pixels 1D vector with the type indexer
    /// <summary>
    /// Returns the <c>Color</c> given by the i-th element of the 1D Pixel's array.
    /// </summary>
    /// <param name="index"></param>
    public Color this[Index index]
    {
        get => Pixels[index];
        set => Pixels[index] = value;
    }

    public Color[] this[Range range] => Pixels[range];
    
    /// <summary>
    /// Returns the index in a 1D array corresponding to the specified matrix column and row.
    /// </summary>
    /// <param name="column">The column index.</param>
    /// <param name="row">The row index.</param>
    /// <returns>The computed 1D array index.</returns>
    public int _PixelOffset(int column, int row)
    {
        _ValidateCoordinates(column, row);
        return row * Width + column;
    }

    //vedere come mettere un controllo con l'eccezione
    /// <summary>
    /// Gives the Color at the indexes (column, row) of the corresponding matrix
    /// </summary>
    /// <param name="column"></param>
    /// <param name="row"></param>
    public Color this[int column, int row]
    {
        get => Pixels[_PixelOffset(column, row)];
        set => Pixels[_PixelOffset(column, row)] = value;
    }

    //Constructors - Begin

    #region Constructors

    public HDRImage(int width, int height)
    {
        _CheckWidthHeight(width, height);
        Width = width;
        Height = height;
        Pixels = new Color[Width * Height];
    }

    public HDRImage(int width, int height,
        in Color[] colorVector)
    {
        _CheckWidthHeight(width, height);
        _CheckPixels(width, height, colorVector);

        Width = width;
        Height = height;
        Pixels = new Color[Width * Height];
        for (int i = 0; i < Pixels.Length; i++)
        {
            Pixels[i] = colorVector[i];
        }
    }

    public HDRImage(Stream stream)
    {
        HDRImage img = ReadPFM_File(stream);

        Width = img.Width;
        Height = img.Height;
        Pixels = new Color[Width * Height];
        for (int i = 0; i < Pixels.Length; i++)
        {
            Pixels[i] = img.Pixels[i];
        }
    }

    public HDRImage(string fileName)
    {
        using (Stream filestream = File.OpenRead(fileName))
        {
            HDRImage img = ReadPFM_File(filestream);

            Width = img.Width;
            Height = img.Height;
            Pixels = new Color[Width * Height];
            for (int i = 0; i < Pixels.Length; i++)
            {
                Pixels[i] = img.Pixels[i];
            }
        }
    }

    //Copy constructor
    protected HDRImage(HDRImage other)
    {
        Width = other.Width;
        Height = other.Height;
        Pixels = new Color[Width * Height];
        for (int i = 0; i < Pixels.Length; i++)
        {
            Pixels[i] = other.Pixels[i];
        }
    }

    #endregion
    
    //Constructors - End

    //meglio usare stringBuilder qua
    /// <summary>
    /// Returns a human-readable string representation of the HDR image,
    /// including dimensions and the full pixel matrix.
    /// </summary>
    public override string ToString()
    {
        string str = $"Height: {Height}, Width: {Width}\n" +
                     "Pixel's matrix:\n" +
                     "\tColumns ->\n" +
                     "Rows";
        for (int j = 0; j < Width; j++)
        {
            str += $"\t{j}";
        }

        str += "\n";

        for (int i = 0; i < Height; i++)
        {
            str += $"{i}";
            for (int j = 0; j < Width; j++)
            {
                str += "\t";
                str += Pixels[_PixelOffset(j, i)].ToString();
            }

            str += "\n";
        }

        return str;
    }

    /// <summary>
    /// Prints the string converted HDRImage.
    /// </summary>
    public void Print()
    {
        Console.WriteLine(ToString());
    }

    /// <summary>
    /// Returns a clone of this HDRImage
    /// </summary>
    /// <returns></returns>
    public HDRImage Clone()
    {
        return new HDRImage(this);
    }

    //Methods for Read and Write PFM files - Begin

    #region PFM_Files

    /// <summary>
    /// Parses an image size string and extracts width and height values.
    /// </summary>
    /// <param name="stringImgSize">The input string containing width and height separated by whitespace.</param>
    /// <param name="width">The parsed width value.</param>
    /// <param name="height">The parsed height value.</param>
    /// <exception cref="InvalidPfmFileFormatException">
    /// Thrown when the input string is not in the correct format or contains invalid values.
    /// </exception>
    public static void _ParseImgSize(string stringImgSize, out int width, out int height)
    {
        string[] stringSizeArray = stringImgSize.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (stringSizeArray.Length != 2)
        {
            throw new InvalidPfmFileFormatException(
                "there isn't the right number of sizes: there must be two sizes width and height.");
        }

        try
        {
            width = int.Parse(stringSizeArray[0], CultureInfo.InvariantCulture);
            height = int.Parse(stringSizeArray[1], CultureInfo.InvariantCulture);
        }
        catch (FormatException ex)
        {
            throw new InvalidPfmFileFormatException("The size is not made of two ints.", ex);
        }

        if (width < 0)
        {
            throw new InvalidPfmFileFormatException("width must be greater than zero.");
        }

        if (height < 0)
        {
            throw new InvalidPfmFileFormatException("height must be greater than zero.");
        }
    }

    /// <summary>
    /// Parses the endianness value from a PFM file string.
    /// </summary>
    /// <param name="stringEndianness">
    /// The endianness value as string. Valid values are "1", "+1", "1.0", "+1.0" for Big Endian,
    /// and "-1", "-1.0" for Little Endian.
    /// </param>
    /// <returns>
    /// The parsed <see cref="Endianness"/> value corresponding to the input string.
    /// </returns>
    /// <exception cref="InvalidPfmFileFormatException">
    /// Thrown when the input string is not a valid endianness value.
    /// </exception>
    public static Endianness _ParseEndianness(string stringEndianness)
    {
        switch (stringEndianness)
        {
            case "1":
            case "+1":
            case "1.0":
            case "+1.0":
                return Endianness.Big;
            case "-1":
            case "-1.0":
                return Endianness.Little;
        }
        throw new InvalidPfmFileFormatException("The endianness must be written as 1.0 or -1.0");
    }

    /// <summary>
    /// Reads a single ASCII line from a binary stream in PFM format.
    /// </summary>
    /// /// <param name="br">The binary reader used to read the stream.</param>
    /// <returns>The line content, or null if the end of the stream is reached before reading any data.</returns>
    public static string? _ReadLine(BinaryReader br)
    {
        var bytes = new List<byte>();

        while (true)
        {
            byte b;

            try
            {
                b = br.ReadByte();
            }
            catch (EndOfStreamException)
            {
                if (bytes.Count == 0)
                    return null;
                break;
            }

            if (b == '\n')
                break;

            if (b != '\r')
                bytes.Add(b);
        }

        return Encoding.ASCII.GetString(bytes.ToArray());
    }

    /// <summary>
    /// Reads a 4-byte single precision floating-point value from the binary reader,
    /// taking into account the specified byte endianness of the source data.
    /// </summary>
    /// <param name="br">The binary reader used to read the data stream.</param>
    /// <param name="endianness">The byte order of the source data (little or big endian).</param>
    /// <returns>The decoded single-precision floating-point value.</returns>
    /// <exception cref="InvalidPfmFileFormatException">
    /// Thrown when the stream does not contain enough bytes to read a 32-bit float.
    /// </exception>
    public static float _ReadFloat(BinaryReader br, Endianness endianness)
    {
        byte[] bytes = br.ReadBytes(4);

        if (bytes.Length < 4)
        {
            throw new InvalidPfmFileFormatException("Unexpected end of file");
        }

        // If the hardware reads bytes in big endian order and the bytes are written in big endian we don't need any reversing.
        // If the hardware reads in big endian but the bytes are written in little endian we need to reverse them.
        // If the hardware reads in little endian but the bytes are written in big endian we need reversing.
        // If the hardware reads in little endian but also the bytes are written in little endian we don't need reversing.
        // In conclusion, we need to reverse the bytes only when the two endianness differ:
        if (BitConverter.IsLittleEndian != (endianness == Endianness.Little))
        {
            Array.Reverse(bytes);
        }

        return BitConverter.ToSingle(bytes, 0);
    }

    /// <summary>
    /// Writes a 32-bit floating point value to the specified stream,
    /// using the big-endian byte order.
    /// </summary>
    /// <param name="outputStream">Target stream.</param>
    /// <param name="value">The single-precision floating-point value to write.</param>
    public static void _WriteFloat(Stream outputStream, float value)
    {
        byte[] bytes = BitConverter.GetBytes(value);

        // if the hardware writes the bytes in little endian order reverse the bytes.
        if (BitConverter.IsLittleEndian)
        {
            Array.Reverse(bytes);
        }

        outputStream.Write(bytes, 0, bytes.Length);
    }

    /// <summary>
    /// Reads and parses the header section of a PFM file.
    /// </summary>
    /// <remarks>
    /// This method validates the PFM magic number, extracts image dimensions,
    /// and reads the endianness marker. It also constructs an <see cref="HDRImage"/>
    /// instance with the parsed width and height, but does not read pixel data.
    /// </remarks>
    /// <param name="br">
    /// A <see cref="BinaryReader"/> positioned at the beginning of a PFM file stream.
    /// </param>
    /// <param name="image">
    /// The constructed <see cref="HDRImage"/> based on the parsed width and height.
    /// </param>
    /// <param name="endianness">
    /// The byte order of the pixel data as specified in the PFM file.
    /// </param>
    /// <exception cref="InvalidPfmFileFormatException">
    /// Thrown when the stream does not represent a valid PFM header
    /// (invalid magic value, missing lines, or malformed metadata).
    /// </exception>
    public static void ReadPFM_Header(BinaryReader br, out HDRImage image, out Endianness endianness)
    {
        string? magic = _ReadLine(br);
        if (magic != "PF")
        {
            throw new InvalidPfmFileFormatException("Invalid magic in PFM file");
        }

        string? imgSize = _ReadLine(br);
        if (imgSize == null)
        {
            throw new InvalidPfmFileFormatException("Missing image size line");
        }

        _ParseImgSize(imgSize, out int width, out int height);

        string? endiannessLine = _ReadLine(br);
        if (endiannessLine == null)
        {
            throw new InvalidPfmFileFormatException("Missing endianness line");
        }

        endianness = _ParseEndianness(endiannessLine);

        /*Console.WriteLine($"POS BEFORE PIXELS: {br.BaseStream.Position}");
        long expectedBytes = width * height * 3 * 4;
        Console.WriteLine($"EXPECTED PIXEL BYTES: {expectedBytes}"); */

        image = new HDRImage(width, height);
        //Console.WriteLine($"Width = {result.Width}, Height = {result.Height}");
    }

    /// <summary>
    /// Returns an HDR image read from a stream in PFM format.
    /// </summary>
    /// <param name="stream">The input stream containing the PFM file data.</param>
    /// <returns>An <see cref="HDRImage"/> containing the decoded floating-point RGB image.</returns>
    /// <exception cref="InvalidPfmFileFormatException">
    /// Thrown when the file does not conform to the expected PFM format (invalid header,
    /// missing metadata lines, or malformed image size/endianness information).
    /// </exception>
    public static HDRImage ReadPFM_File(Stream stream)
    {
        using var br = new BinaryReader(stream);

        HDRImage.ReadPFM_Header(br, out HDRImage image, out Endianness endianness);

        // the matrix of colors in PFM files is saved bottom to top and left to right
        for (int row = image.Height - 1; row >= 0; row--)
        {
            for (int col = 0; col < image.Width; col++)
            {
                //Console.WriteLine($"Column = {j}, Row = {i}");
                //Console.WriteLine($"Offset = {result._PixelOffset(j,i)}");
                //Console.WriteLine($"Offset 2 = {i * result.Width + j}");
                var color = new Color
                {
                    R = _ReadFloat(br, endianness),
                    G = _ReadFloat(br, endianness),
                    B = _ReadFloat(br, endianness)
                };
                image[col, row] = color;
            }
        }

        //if (result.Pixels != null) Console.WriteLine($"W={result.Width}, H={result.Height}, Pixels={result.Pixels.Length}");

        return image;
    }

    /// <summary>
    /// Returns an HDR image read from a file in PFM format.
    /// </summary>
    /// <param name="filePath">The path of the PFM file.</param>
    /// <returns>An <see cref="HDRImage"/> containing the decoded floating-point RGB image.</returns>
    public static HDRImage ReadPFM_File(string filePath)
    {
        using (Stream filestream = File.OpenRead(filePath))
        {
            return ReadPFM_File(filestream);
        }
    }

    /// <summary>
    /// Writes an HDR image to a stream in PFM format.
    /// </summary>
    /// <remarks>
    /// It's used the big-endian byte order to write the RGB values of the colors.
    /// </remarks>
    /// <param name="img">The HDR image to write.</param>
    /// <param name="filestream">The output stream where the PFM data will be written.</param>
    public static void WritePFM(HDRImage img, Stream filestream)
    {
        if (img == null)
        {
            throw new ArgumentNullException(nameof(img));
        }
        
        byte[] header = Encoding.ASCII.GetBytes($"PF\n{img.Width} {img.Height}\n1.0\n");
        filestream.Write(header, 0, header.Length);

        // the matrix of colors in PFM files must be saved bottom to top and left to right
        for (int row = img.Height - 1; row >= 0; row--)
        {
            for (int col = 0; col < img.Width; col++)
            {
                Color color = img[col, row];
                _WriteFloat(filestream, color.R);
                _WriteFloat(filestream, color.G);
                _WriteFloat(filestream, color.B);
            }
        }
    }

    /// <summary>
    /// Writes an HDR image to a file in PFM format.
    /// </summary>
    /// <remarks>
    /// Pixel data is stored in big-endian byte order.
    /// </remarks>
    /// <param name="img">The HDR image to write.</param>
    /// <param name="filePath">The path of the output file where the PFM data will be written.</param>
    public static void WritePFM_File(HDRImage img, string filePath)
    {
        using (Stream filestream = File.OpenWrite(filePath))
        {
            WritePFM(img, filestream);
        }
    }

    #endregion
    
    // Methods for Read and Write PFM files - End

    // Methods for conversion to an LDR Image (Tone mapping) - Begin

    #region Tone_mapping

     /// <summary>
    /// Computes the logarithmic average luminosity of the image.
    /// The luminosity of each pixel is evaluated according to the specified <see cref="LumFunction"/>
    /// (Shirley = Shirley and Morley method, Weighted = Weighted Average)
    /// </summary>
    /// <param name="luminosityFunction">The pixel luminosity algorithm to use.</param>
    /// <param name="delta">Small positive value added to pixel luminosity to avoid
    /// logarithm singularities when luminosity is zero.</param>
    /// <returns>The logarithmic average luminosity of the image.</returns>
    public float _AverageLuminosity(LumFunction luminosityFunction, float delta = 1e-10f)
    {
        float sum = 0.0f;
        // perceived value of luminosity follows a logarithmic scale
        // so we must use a logarithmic average
        // delta is needed to avoid singular values for the logarithm
        switch (luminosityFunction)
        {
            case LumFunction.Shirley:

                foreach (Color color in Pixels)
                {
                    sum += MathF.Log10(delta + color.LuminosityShirleyMorley());
                }

                return MathF.Pow(10, sum / Pixels.Length);

            case LumFunction.Weighted:

                foreach (Color color in Pixels)
                {
                    sum += MathF.Log10(delta + color.LuminosityWeightedAverage());
                }

                return MathF.Pow(10, sum / Pixels.Length);
            default:
                throw new NotImplementedException("Average Luminosity: case not implemented.");

        }
    }
    
    /// <summary>
    /// Scales all pixels so that their RGB values are normalized with respect
    /// to the image average luminosity.
    /// Each pixel is multiplied by factor / averageLuminosity.
    /// The luminosityFunction tells which of function of the color class to use to compute the luminosity of the pixel, see <c>LumFunction</c>
    /// If <paramref name="averageLuminosity"/> is not provided, it is computed
    /// using <see cref="_AverageLuminosity"/> and the specified
    /// <paramref name="luminosityFunction"/>.
    /// </summary>
    /// <param name="luminosityFunction">Function used to compute pixel luminosity when the average luminosity
    /// needs to be calculated.</param>
    /// <param name="factor"> An empirical value.</param>
    /// <param name="averageLuminosity">Optional precomputed average luminosity.</param>
    /// <param name="delta">Small positive value added to pixel luminosity to avoid
    /// logarithm singularities in the computation of <see cref="_AverageLuminosity"/></param>
    public void _Normalize(LumFunction luminosityFunction, float factor, float? averageLuminosity = null,
        float delta = 1e-10f)
    {
        //if averageLuminosity is null compute it with the _AverageLuminosity function
        averageLuminosity ??= _AverageLuminosity(luminosityFunction, delta);
        for (int i = 0; i < Pixels.Length; i++)
        {
            //averageLuminosity is a nullable type so we must explicitly cast it from float? to float
            Pixels[i] = Pixels[i] * (factor / averageLuminosity.Value);
        }
    }

    /// <summary>
    /// Compress all the colors of the image into the range [0,1),
    /// using the function x / (x + 1), reducing the intensity of bright spots.
    /// </summary>
    public void _ClampImage()
    {
        for (int i = 0; i < Pixels.Length; i++)
        {
            Pixels[i]._Clamp();
        }
    }
    
    /// <summary>
    /// Applies gamma correction and converts all pixels
    /// to a 0–255 RGB representation.
    /// </summary>
    /// <param name="gamma">Gamma exponent used for power-law correction (must be > 0).
    /// It's characteristic of the display used.</param>
    public void _ImageTo8BitRGB(float gamma)
    {
        for (int i = 0; i < Pixels.Length; i++)
        {
            Pixels[i] = Pixels[i].To8BitRGB(gamma);
        }
    }
    
    //Questa non è tecnicamente un HDR. Rivedere in futuro
    /// <summary>
    /// Returns Creates an LDR representation of the current HDR image.
    /// It accounts for the gamma correction of the display and of the empirical factor here named "factor"
    /// The luminosityFunction parameter allow to choose between some possible ways to compute the luminosity of a pixel.
    /// averageLuminosity is an optional parameter you can use if you've already computed it previously.
    /// </summary>
    /// <param name="luminosityFunction">Function used to compute pixel luminosity.</param>
    /// <param name="factor">Empirical scaling factor used in normalization.</param>
    /// <param name="gamma">Gamma exponent used for power-law correction (must be > 0).
    /// It's characteristic of the display used.</param>
    /// <param name="averageLuminosity">Optional precomputed average luminosity.</param>
    /// <param name="delta">Small positive value added to pixel luminosity to avoid
    /// logarithm singularities in the computation of <see cref="_AverageLuminosity"/></param>
    /// <returns>The LDR image (0–255 range per channel).</returns>
    public HDRImage CreateLDR(LumFunction luminosityFunction, float factor, float gamma,
        float? averageLuminosity = null,
        float delta = 1e-10f)
    {
        HDRImage image = Clone();
        image._Normalize(luminosityFunction, factor, averageLuminosity, delta);
        image._ClampImage();
        image._ImageTo8BitRGB(gamma);
        return image;
    }

    #endregion
    
    // Methods for conversion to an LDR Image - End
    
    /// <summary>
    /// Writes on the outputStream the corresponding LDR image.
    /// It applies the gamma correction of the display and of the empirical factor here named "factor".
    /// The luminosityFunction parameter allow to choose between some possible ways to compute the luminosity of a pixel.
    /// averageLuminosity is an optional parameter you can use if you've already computed it previously.
    /// </summary>
    /// <param name="outputStream">Destination stream where the PNG image will be written.</param>
    /// <param name="luminosityFunction">Function used to compute pixel luminosity for tone mapping.</param>
    /// <param name="factor">Empirical scaling factor used in normalization.</param>
    /// <param name="gamma">Gamma exponent used for power-law correction (must be > 0).
    /// It's characteristic of the display used.</param>
    /// <param name="averageLuminosity">Optional precomputed average luminosity.</param>
    /// <param name="delta">Small positive value added to pixel luminosity to avoid
    /// logarithm singularities in the computation of <see cref="_AverageLuminosity"/></param>
    public void WritePNG(Stream outputStream, LumFunction luminosityFunction, float factor, float gamma,
        float? averageLuminosity = null,
        float delta = 1e-10f)
    {
        HDRImage LDRimage = CreateLDR(luminosityFunction, factor, gamma, averageLuminosity, delta);

        Image<Rgb24> bitmap = new Image<Rgb24>(Configuration.Default, Width, Height);

        for (int i = 0; i < Width; i++)
        {
            for (int j = 0; j < Height; j++)
            {
                //the Rgb format requires 3 numbers of the type byte
                //so we must convert the RGB values to bytes
                bitmap[i, j] = new Rgb24((byte)LDRimage[i, j].R, (byte)LDRimage[i, j].G, (byte)LDRimage[i, j].B);
            }
        }

        bitmap.SaveAsPng(outputStream);
    }

    /// <summary>
    /// Creates a PNG file of the corresponding LDR image
    /// It accounts for the gamma correction of the display and of the empirical factor here named "factor"
    /// The luminosityFunction parameter allow to choose between some possible ways to compute the luminosity of a pixel
    /// </summary>
    /// <param name="outputFilePath"></param>
    /// <param name="luminosityFunction"></param>
    /// <param name="factor"></param>
    /// <param name="gamma"></param>
    /// <param name="averageLuminosity"></param>
    /// <param name="delta"></param>
    public void WritePNG(string outputFilePath, LumFunction luminosityFunction, float factor, float gamma,
        float? averageLuminosity = null,
        float delta = 1e-10f)
    {
        //using (Stream fileStream = File.OpenWrite(outputFilename))
        using (Stream fileStream = new FileStream(outputFilePath, FileMode.Create))
        {
            WritePNG(fileStream, luminosityFunction, factor, gamma, averageLuminosity, delta);
        }
    }
}

/// <summary>
/// /// Specifies the byte order convention used when reading or writing multibyte values.
/// 
/// Big-endian order writes the most significant byte before the least significant (as we, human people, normally do).
/// Little-endian order writes the least significant byte before the most significant.
/// 
/// For example, the decimal number 14943, that is 3A5F in hexadecimal,
/// is stored as 3A 5F in Big Endian and as 5F 3A in Little Endian. 
/// </summary>
public enum Endianness
{
    /// <summary>
    /// Most significant byte first.
    /// </summary>
    Big,
    
    /// <summary>
    /// Least significant byte first.
    /// </summary>
    Little
}

/// <summary>
/// Specifies the algorithm used to compute pixel luminosity.
/// </summary>
public enum LumFunction
{
    Shirley,
    Weighted
}
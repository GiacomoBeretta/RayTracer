#nullable disable

using System.Diagnostics.CodeAnalysis;
using TracerLib;

namespace TracerTests;

[SuppressMessage("ReSharper", "InconsistentNaming")]
public class HDRImageTest
{
    [Fact]
    public void TestCheckCoordinates()
    {
        HDRImage image1 = new HDRImage(6, 11);
        image1._ValidateCoordinates(0, 0);
        image1._ValidateCoordinates(5, 10);
        Assert.Throws<ArgumentOutOfRangeException>(() => image1._ValidateCoordinates(-1, 0));
        Assert.Throws<ArgumentOutOfRangeException>(() => image1._ValidateCoordinates(0, -1));
        Assert.Throws<ArgumentOutOfRangeException>(() => image1._ValidateCoordinates(6, 2));
        Assert.Throws<ArgumentOutOfRangeException>(() => image1._ValidateCoordinates(4, 11));
    }

    [Fact]
    public void TestCheckWidthHeight()
    {
        HDRImage._CheckWidthHeight(1,1);
        HDRImage._CheckWidthHeight(190,201);
        Assert.Throws<ArgumentOutOfRangeException>(() => HDRImage._CheckWidthHeight(-5, 2));
        Assert.Throws<ArgumentOutOfRangeException>(() => HDRImage._CheckWidthHeight(0, 2));
        Assert.Throws<ArgumentOutOfRangeException>(() => HDRImage._CheckWidthHeight(1, -7));
        Assert.Throws<ArgumentOutOfRangeException>(() => HDRImage._CheckWidthHeight(6, 0));
    }

    [Fact]
    public void TestCheckPixels()
    {
        HDRImage._CheckPixels(1,1, new Color[1]);
        HDRImage._CheckPixels(5,2, new Color[10]);
        Assert.Throws<ArgumentNullException>(() => HDRImage._CheckPixels(1, 2, null));
        Assert.Throws<ArgumentException>(() => HDRImage._CheckPixels(10, 91, new Color[911]));
    }

    [Fact]
    public void Test1DIndex()
    {
        int width = 10;
        int height = 4;
        HDRImage image = new HDRImage(width, height);

        Color[] colors = new Color[width * height];
        float red, green, blue;
        int offset;
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                offset = image._PixelOffset(i, j);
                red = offset;
                green = offset * 2;
                blue = offset * 3;
                colors[offset] = new Color(red, green, blue);
            }
        }

        image.Pixels = colors;
        Assert.Equal(new Color(4, 8, 12), image[4]);
        Assert.Equal(new Color(39, 78, 117), image[^1]);

        Color[] colorArray = new Color[3];
        colorArray[0] = new Color(7, 14, 21);
        colorArray[1] = new Color(8, 16, 24);
        colorArray[2] = new Color(9, 18, 27);
        Assert.Equal(colorArray, image[7..10]);
    }

    [Fact]
    public void TestPixelOffset()
    {
        HDRImage image1 = new HDRImage(5, 13);

        Assert.Throws<ArgumentOutOfRangeException>(() => image1._PixelOffset(-1, 0));
        Assert.Throws<ArgumentOutOfRangeException>(() => image1._PixelOffset(0, -1));
        Assert.Throws<ArgumentOutOfRangeException>(() => image1._PixelOffset(1, 13));
        Assert.Throws<ArgumentOutOfRangeException>(() => image1._PixelOffset(5, 2));

        Assert.Equal(0, image1._PixelOffset(0, 0));
        Assert.Equal(2, image1._PixelOffset(2, 0));
        Assert.Equal(10, image1._PixelOffset(0, 2));
        Assert.Equal(29, image1._PixelOffset(4, 5));

        HDRImage image2 = new HDRImage(100, 100);
        Assert.Equal(203, image2._PixelOffset(3, 2));
    }

    [Fact]
    public void Test2DIndex()
    {
        int width = 15;
        int height = 20;
        HDRImage image = new HDRImage(width, height);

        Color[] colors = new Color[width * height];
        float red, green, blue;
        int offset = 0;
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                offset = image._PixelOffset(i, j);
                red = offset * 3;
                green = offset;
                blue = offset * 4;
                colors[offset] = new Color(red, green, blue);
            }
        }

        image.Pixels = colors;
        Assert.Equal(new Color(246, 82, 328), image[7, 5]);
    }

    [Fact]
    public void TestConstructorWithColorVector()
    {
        Color[]? colors = null;
        Assert.Throws<ArgumentNullException>(() => new HDRImage(10, 10, colors));

        colors = new Color[99];
        Assert.Throws<ArgumentException>(() => new HDRImage(10, 10, colors));
    }

    //test constructor from stream and from filename

    [Fact]
    public void TestToString()
    {
        HDRImage image = new HDRImage(2, 1);
        image.Pixels[0] = new Color(3, 7, 10);
        image.Pixels[1] = new Color(4, 21, 15);
        string str = "Height: 1, Width: 2\n" +
                     "Pixel's matrix:\n" +
                     "\tColumns ->\n" +
                     "Rows\t0\t1\n" +
                     "0\t(R=3, G=7, B=10)\t(R=4, G=21, B=15)\n";
        Assert.Equal(str, image.ToString());
    }

    [Fact]
    public void TestClone()
    {
        Color[] colors = new Color[2];
        colors[0] = new Color(2, 4, 7);
        colors[1] = new Color(5, 3, 9);
        HDRImage image = new HDRImage(2, 1, colors);
        HDRImage copy = image.Clone();

        Assert.Equal(image.Width, copy.Width);
        Assert.Equal(image.Height, copy.Height);
        Assert.Equal(image.Pixels, copy.Pixels);

        Assert.NotEqual(image, copy);
        image[0] = colors[1];
        Assert.NotEqual(image.Pixels, copy.Pixels);
    }

    [Fact]
    public void TestParseImgSize()
    {
        int width, height;
        string imgSize;

        imgSize = "1 3 8";
        Assert.Throws<InvalidPfmFileFormatException>(() => HDRImage._ParseImgSize(imgSize, out width, out height));

        imgSize = "a  b";
        Assert.Throws<InvalidPfmFileFormatException>(() => HDRImage._ParseImgSize(imgSize, out width, out height));

        imgSize = "-1 23";
        Assert.Throws<InvalidPfmFileFormatException>(() => HDRImage._ParseImgSize(imgSize, out width, out height));

        imgSize = "5 -8";
        Assert.Throws<InvalidPfmFileFormatException>(() => HDRImage._ParseImgSize(imgSize, out width, out height));

        imgSize = "-5 -8";
        Assert.Throws<InvalidPfmFileFormatException>(() => HDRImage._ParseImgSize(imgSize, out width, out height));

        imgSize = "41 78";
        HDRImage._ParseImgSize(imgSize, out width, out height);
        Assert.Equal(41, width);
        Assert.Equal(78, height);
    }

    [Fact]
    public void TestParseEndianness()
    {
        string endianness;

        endianness = "1 .0";
        Assert.Throws<InvalidPfmFileFormatException>(() => HDRImage._ParseEndianness(endianness));
        endianness = "zws";
        Assert.Throws<InvalidPfmFileFormatException>(() => HDRImage._ParseEndianness(endianness));
        endianness = "2. 0";
        Assert.Throws<InvalidPfmFileFormatException>(() => HDRImage._ParseEndianness(endianness));
        endianness = "0";
        Assert.Throws<InvalidPfmFileFormatException>(() => HDRImage._ParseEndianness(endianness));
        endianness = "- 2.0";
        Assert.Throws<InvalidPfmFileFormatException>(() => HDRImage._ParseEndianness(endianness));

        endianness = "1.0";
        Assert.Equal(Endianness.Big, HDRImage._ParseEndianness(endianness));
        endianness = "-1.0";
        Assert.Equal(Endianness.Little, HDRImage._ParseEndianness(endianness));
        endianness = "+102.0";
        Assert.Equal(Endianness.Big, HDRImage._ParseEndianness(endianness));
        endianness = "-920.40000";
        Assert.Equal(Endianness.Little, HDRImage._ParseEndianness(endianness));
    }

    //test read float

    //test write float

    //test read pfm file (2)

    //test write_pfm_file (2)

    [Fact]
    public void TestAverageLuminosityShirleyMorley()
    {
        HDRImage image1 = new HDRImage(1, 2);
        image1[0] = new Color(5, 10, 15); //Luminosity = 10.0
        image1[1] = new Color(500, 1000, 1500); //Luminosity = 1000.0

        Assert.Equal(100.0, image1._AverageLuminosity(0, delta: 0.0f));

        HDRImage image2 = new HDRImage(1, 3);
        image2[0] = new Color(1, 0, 2); //Luminosity = 1
        image2[1] = new Color(300000, 1550000, 1700000); //Luminosity = 1000000
        image2[2] = new Color(0, 0, 0); // Luminosity = 0
        Assert.True(Functions.AreClose(10.00333f, image2._AverageLuminosity(0, delta: 1e-3f), 1e-5f));
    }

    [Fact]
    public void TestAverageLuminosityWeighted()
    {
        HDRImage image = new HDRImage(1, 3);
        image[0] = new Color(4.1f, 2.0f, 11); //Luminosity = 3.09626
        image[1] = new Color(33.6f, 83, 27.2f); //Luminosity = 68.4688
        image[2] = new Color(0.3f, 44.9f, 9.3f); // Luminosity = 32.84772
        Assert.True(Functions.AreClose(19.0961195f, image._AverageLuminosity(LumFunction.Weighted, 0), 1e-5f));
    }

    [Fact]
    public void TestNormalizeShirleyMorley()
    {
        HDRImage image = new HDRImage(2, 1);
        image[0] = new Color(5, 10, 15); // luminosity = 10
        image[1] = new Color(500, 1000, 1500); // luminosity = 1000
        // log10(average luminosity) ~= ( log10(10) + log10(1000) ) / 2 = 2
        // average luminosity = 10^2 = 100
        // factor/averageluminosity = 1000/100=10
        image._Normalize(0, 1000);
        Assert.True(Color._AreColorsClose(image[0], new Color(50, 100, 150)));
        Assert.True(Color._AreColorsClose(image[1], new Color(5000, 10000, 15000)));
    }

    [Fact]
    public void TestNormalizeWeighted()
    {
        HDRImage image = new HDRImage(2, 1);
        image[0] = new Color(102.5f, 233.4f, 140.8f); // Luminosity = 32.84772
        image[1] = new Color(1683.7f, 2380.2f, 3400.6f); // Luminosity = 32.84772
        //averageLuminosityWeighted = 677.19147515
        image._Normalize(LumFunction.Weighted, 1, delta: 0);
        Assert.True(Color._AreColorsClose(image[0], new Color(0.1513604f, 0.3446588f, 0.2079176f)));
        Assert.True(Color._AreColorsClose(image[1], new Color(2.4862983f, 3.5148109f, 5.0216226f)));
    }
    
     [Fact]
    public void TestReadPFM_File()  //Rivedi
    {
        /*
        byte[] LE_REFERENCE_BYTES = new byte[]
        { 
            0x50, 0x46, 0x0a, 0x33, 0x20, 0x32, 0x0a, 0x2d, 0x31, 0x2e, 0x30, 0x0a,
            0x00, 0x00, 0xc8, 0x42, 0x00, 0x00, 0x48, 0x43, 0x00, 0x00, 0x96, 0x43,
            0x00, 0x00, 0xc8, 0x43, 0x00, 0x00, 0xfa, 0x43, 0x00, 0x00, 0x16, 0x44,
            0x00, 0x00, 0x2f, 0x44, 0x00, 0x00, 0x48, 0x44, 0x00, 0x00, 0x61, 0x44,
            0x00, 0x00, 0x20, 0x41, 0x00, 0x00, 0xa0, 0x41, 0x00, 0x00, 0xf0, 0x41,
            0x00, 0x00, 0x20, 0x42, 0x00, 0x00, 0x48, 0x42, 0x00, 0x00, 0x70, 0x42,
            0x00, 0x00, 0x8c, 0x42, 0x00, 0x00, 0xa0, 0x42, 0x00, 0x00, 0xb4, 0x42
        };

        byte[] BE_REFERENCE_BYTES = new byte[]
        {
            0x50, 0x46, 0x0a, 0x33, 0x20, 0x32, 0x0a, 0x31, 0x2e, 0x30, 0x0a, 0x42,
            0xc8, 0x00, 0x00, 0x43, 0x48, 0x00, 0x00, 0x43, 0x96, 0x00, 0x00, 0x43,
            0xc8, 0x00, 0x00, 0x43, 0xfa, 0x00, 0x00, 0x44, 0x16, 0x00, 0x00, 0x44,
            0x2f, 0x00, 0x00, 0x44, 0x48, 0x00, 0x00, 0x44, 0x61, 0x00, 0x00, 0x41,
            0x20, 0x00, 0x00, 0x41, 0xa0, 0x00, 0x00, 0x41, 0xf0, 0x00, 0x00, 0x42,
            0x20, 0x00, 0x00, 0x42, 0x48, 0x00, 0x00, 0x42, 0x70, 0x00, 0x00, 0x42,
            0x8c, 0x00, 0x00, 0x42, 0xa0, 0x00, 0x00, 0x42, 0xb4, 0x00, 0x00
        };*/

        byte[] LE_REFERENCE_BYTES =
        [
            0x50, 0x46, 0x0a, 0x33, 0x20, 0x32, 0x0a, 0x2d, 0x31, 0x2e, 0x30, 0x0a,
            0x00, 0x00, 0xc8, 0x42, 0x00, 0x00, 0x48, 0x43, 0x00, 0x00, 0x96, 0x43,
            0x00, 0x00, 0xc8, 0x43, 0x00, 0x00, 0xfa, 0x43, 0x00, 0x00, 0x16, 0x44,
            0x00, 0x00, 0x2f, 0x44, 0x00, 0x00, 0x48, 0x44, 0x00, 0x00, 0x61, 0x44,
            0x00, 0x00, 0x20, 0x41, 0x00, 0x00, 0xa0, 0x41, 0x00, 0x00, 0xf0, 0x41,
            0x00, 0x00, 0x20, 0x42, 0x00, 0x00, 0x48, 0x42, 0x00, 0x00, 0x70, 0x42,
            0x00, 0x00, 0x8c, 0x42, 0x00, 0x00, 0xa0, 0x42, 0x00, 0x00, 0xb4, 0x42
        ];
        
        byte[] BE_REFERENCE_BYTES =
        [
            0x50, 0x46, 0x0a, 0x33, 0x20, 0x32, 0x0a, 0x31, 0x2e, 0x30, 0x0a, 0x42,
            0xc8, 0x00, 0x00, 0x43, 0x48, 0x00, 0x00, 0x43, 0x96, 0x00, 0x00, 0x43,
            0xc8, 0x00, 0x00, 0x43, 0xfa, 0x00, 0x00, 0x44, 0x16, 0x00, 0x00, 0x44,
            0x2f, 0x00, 0x00, 0x44, 0x48, 0x00, 0x00, 0x44, 0x61, 0x00, 0x00, 0x41,
            0x20, 0x00, 0x00, 0x41, 0xa0, 0x00, 0x00, 0x41, 0xf0, 0x00, 0x00, 0x42,
            0x20, 0x00, 0x00, 0x42, 0x48, 0x00, 0x00, 0x42, 0x70, 0x00, 0x00, 0x42,
            0x8c, 0x00, 0x00, 0x42, 0xa0, 0x00, 0x00, 0x42, 0xb4, 0x00, 0x00
        ];

        using (Stream reference_bytes = new MemoryStream(LE_REFERENCE_BYTES))
        {
            HDRImage img = new HDRImage(reference_bytes);
            
            Assert.Equal(2, img.Height);
            Assert.Equal(3, img.Width); 
            
            Assert.True(Color._AreColorsClose(img[0,0],new Color(10f, 20f, 30f)));
            Assert.True(Color._AreColorsClose(img[1,0],new Color(40f, 50f, 60f)));
            Assert.True(Color._AreColorsClose(img[2,0],new Color(70f, 80f, 90f)));
            Assert.True(Color._AreColorsClose(img[0,1],new Color(100f, 200f, 300f)));
            Assert.True(Color._AreColorsClose(img[1,1],new Color(400f, 500f, 600f)));
            Assert.True(Color._AreColorsClose(img[2,1],new Color(700f, 800f, 900f)));
        }
        
        using (Stream reference_bytes = new MemoryStream(BE_REFERENCE_BYTES))
        {
            HDRImage img = new HDRImage(reference_bytes);
                    
            Assert.Equal(2, img.Height);
            Assert.Equal(3, img.Width); 
                    
            Assert.True(Color._AreColorsClose(img[0,0],new Color(10f, 20f, 30f)));
            Assert.True(Color._AreColorsClose(img[1,0],new Color(40f, 50f, 60f)));
            Assert.True(Color._AreColorsClose(img[2,0],new Color(70f, 80f, 90f)));
            Assert.True(Color._AreColorsClose(img[0,1],new Color(100f, 200f, 300f)));
            Assert.True(Color._AreColorsClose(img[1,1],new Color(400f, 500f, 600f)));
            Assert.True(Color._AreColorsClose(img[2,1],new Color(700f, 800f, 900f)));
        }
        
    }
    
    [Fact]
    public void TestClampImage()
    {
        HDRImage img = new HDRImage(2, 1)
        {
            [0, 0] = new Color(0.5f, 1.0f, 1.5f),
            [1, 0] = new Color(50.0f, 100.0f, 150.0f)
        };

        img._ClampImage();

        foreach (Color pixel in img.Pixels)
        {
            Assert.True(pixel.R is >= 0 and <= 1);
            Assert.True(pixel.G is >= 0 and <= 1);
            Assert.True(pixel.B is >= 0 and <= 1);
        }
    }

    [Fact]
    public void TestImageTo8BitRGB()
    {
        float gamma = 3.6f;

        HDRImage image = new HDRImage(3, 1)
        {
            [0] = new Color(0.883f, 0.2102f, 0.3775f),
            [1] = new Color(0.2381f, 0.9324f, 0.4467f),
            [2] = new Color(0.1941f, 0.5728f, 0.9483f)
        };
        image._ImageTo8BitRGB(gamma);
        Assert.Equal(new Color(246, 165, 195), image[0]);
        Assert.Equal(new Color(171, 250, 204), image[1]);
        Assert.Equal(new Color(162, 218, 251), image[2]);
    }

    [Fact]
    public void TestCreateLDR()
    {
        Color[] colorVector = new Color[3];
        colorVector[0] = new Color(62.89f, 32.7f, 7772.02f);
        colorVector[1] = new Color(37.83f, 462.006f, 422.55f);
        colorVector[2] = new Color(174, 25.3f, 2773);

        float factor = 1;
        LumFunction luminosityFunction = LumFunction.Shirley;
        float gamma = 2;
        float delta = 0;
        HDRImage hdrImage = new HDRImage(3, 1, colorVector);

        HDRImage ldrImage = hdrImage.CreateLDR(luminosityFunction, factor, gamma, delta);
        hdrImage._Normalize(luminosityFunction, factor, delta);
        hdrImage._ClampImage();
        hdrImage._ImageTo8BitRGB(gamma);

        Assert.Equal(hdrImage.Width, ldrImage.Width);
        Assert.Equal(hdrImage.Height, ldrImage.Height);
        Assert.Equal(hdrImage.Pixels, ldrImage.Pixels);
    }

    [Fact]
    public void TestWritePNG()
    {
    }
}
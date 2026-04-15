using Xunit;
using TracerLib;

/*
  *using Colors;
   using Hdr;
 */
namespace TracerTests;

// namespace Hdr.Test;
public class HDRTest
{
    [Fact]
    public void TestAreCoordinatesValid()
    {
        HDRImage image1 = new HDRImage(4, 10);
        Assert.True(image1._AreCoordinatesValid(0, 0));
        Assert.True(image1._AreCoordinatesValid(3, 2));
        Assert.True(image1._AreCoordinatesValid(3, 9));
        Assert.False(image1._AreCoordinatesValid(4, 1));
        Assert.False(image1._AreCoordinatesValid(0, 10));
        Assert.False(image1._AreCoordinatesValid(-1, 2));
        Assert.False(image1._AreCoordinatesValid(2, -1));

        var image2 = new HDRImage(100, 100);
        Assert.True(image2._AreCoordinatesValid(53, 68));
        Assert.False(image2._AreCoordinatesValid(200, 39));
    }

    [Fact]
    public void Test1DIndex()
    {
        int width = 10;
        int height = 4;
        HDRImage image = new HDRImage(width, height);

        Color[] colors = new Color[width * height];
        float red, green, blue;
        int offset = 0;
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
    }

    [Fact]
    public void TestPixelOffset()
    {
        HDRImage image1 = new HDRImage(5, 13);

        Assert.Equal(0, image1._PixelOffset(0, 0));
        Assert.Equal(2, image1._PixelOffset(2, 0));
        Assert.Equal(10, image1._PixelOffset(0, 2));
        Assert.Equal(29, image1._PixelOffset(4, 5));

        var image2 = new HDRImage(100, 100);
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

    //da rivedere dopo aver visto meglio le lambda functions
    [Fact]
    public void TestParseImgSize()
    {
        int width, height;
        string imgSize;

        imgSize = "1 3 8";
        Assert.Throws<InvalidPfmFileFormat>(() => HDRImage._ParseImgSize(imgSize, out width, out height));

        imgSize = "a  b";
        Assert.Throws<InvalidPfmFileFormat>(() => HDRImage._ParseImgSize(imgSize, out width, out height));

        imgSize = "-1 23";
        Assert.Throws<InvalidPfmFileFormat>(() => HDRImage._ParseImgSize(imgSize, out width, out height));

        imgSize = "5 -8";
        Assert.Throws<InvalidPfmFileFormat>(() => HDRImage._ParseImgSize(imgSize, out width, out height));

        imgSize = "-5 -8";
        Assert.Throws<InvalidPfmFileFormat>(() => HDRImage._ParseImgSize(imgSize, out width, out height));

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
        Assert.Throws<InvalidPfmFileFormat>(() => HDRImage._ParseEndianness(endianness));
        endianness = "zws";
        Assert.Throws<InvalidPfmFileFormat>(() => HDRImage._ParseEndianness(endianness));
        endianness = "2.0";
        Assert.Throws<InvalidPfmFileFormat>(() => HDRImage._ParseEndianness(endianness));
        endianness = "0";
        Assert.Throws<InvalidPfmFileFormat>(() => HDRImage._ParseEndianness(endianness));
        endianness = "-2.0";
        Assert.Throws<InvalidPfmFileFormat>(() => HDRImage._ParseEndianness(endianness));

        endianness = "1.0";
        Assert.True( HDRImage._ParseEndianness(endianness));
        endianness = "-1.0";
        Assert.True( HDRImage._ParseEndianness(endianness));
    }

    //test readfloat

    //test write float

    //test write_pfm_file

    //test Oveloading write_pfm con stream


    /*
    //TestAverageLuminosityShirleyMorley
     [Fact]
     public void TestAverageLuminosityShirleyMorley()
     {
         HDRImage image1 = new HDRImage(1, 2);
         image1[0] = new Color(5, 10, 15); //Luminosity = 10.0
         image1[1] = new Color(500, 1000, 1500); //Luminosity = 1000.0

         Assert.Equal(100.0, image1._AverageLuminosity(0,delta:0.0f));

         HDRImage image2 = new HDRImage(1, 3);
         image2[0] = new Color(1, 0, 2); //Luminosity = 1
         image2[1] = new Color(1550000, 1300000, 1700000); //Luminosity = 1000000
         image2[2] = new Color(0, 0, 0); // Luminosity
         //Assert.Equal(10, image2.AverageLuminosity(delta:1e-3f));
     }

     [Fact]
     public void TestAverageLuminosityWeighted()
     {
         HDRImage image = new HDRImage(1, 3);
         image[0] = new Color(4.1f, 2.0f, 11); //Luminosity = 3.09626
         image[1] = new Color(33.6f, 83, 27.2f); //Luminosity = 68.4688
         image[2] = new Color(0.3f, 44.9f, 9.3f); // Luminosity = 32.84772
         Assert.True(Functions.AreClose(19.0961195f, image._AverageLuminosity(1,0));
     }

    [Fact]
    public void TestNormalizeShirleyMorley()
    {
        HDRImage image = new HDRImage(2, 1);
        image[0]=new Color(5, 10, 15);
        image[1] = new Color(500, 1000, 1500);

        image._Normalize(1000, 0);
        Assert.True(Color._AreCloseColor(image[0], new Color(0.5e2f, 1.0e2f, 1.5e2f)));
        Assert.True(Color._AreCloseColor(image[1], new Color(0.5e4f, 1.0e4f, 1.5e4f)));
    }

    [Fact]
    public void TestNormalizeWeighted()
    {
        HDRImage image = new HDRImage(2, 1);
        image[0]=new Color(102.5f, 233.4f, 140.8f); // Luminosity = 32.84772
        image[1] = new Color(1683.7f, 2380.2f, 3400.6f);// Luminosity = 32.84772
        //averageLuminosityWeighted = 677.19147515
        image._Normalize(1,1, delta:0);
        Assert.True(Color._AreCloseColor(image[0], new Color(0.1513604f, 0.3446588f,0.2079176f )));
        Assert.True(Color._AreCloseColor(image[1], new Color(2.4862983f, 3.5148109f, 5.0216226f )));
    }*/

     [Fact]
    public void TestReadPFM_File()  //Rivedi
    {
        var LE_REFERENCE_BYTES = new byte[]
        { 
            0x50, 0x46, 0x0a, 0x33, 0x20, 0x32, 0x0a, 0x2d, 0x31, 0x2e, 0x30, 0x0a,
            0x00, 0x00, 0xc8, 0x42, 0x00, 0x00, 0x48, 0x43, 0x00, 0x00, 0x96, 0x43,
            0x00, 0x00, 0xc8, 0x43, 0x00, 0x00, 0xfa, 0x43, 0x00, 0x00, 0x16, 0x44,
            0x00, 0x00, 0x2f, 0x44, 0x00, 0x00, 0x48, 0x44, 0x00, 0x00, 0x61, 0x44,
            0x00, 0x00, 0x20, 0x41, 0x00, 0x00, 0xa0, 0x41, 0x00, 0x00, 0xf0, 0x41,
            0x00, 0x00, 0x20, 0x42, 0x00, 0x00, 0x48, 0x42, 0x00, 0x00, 0x70, 0x42,
            0x00, 0x00, 0x8c, 0x42, 0x00, 0x00, 0xa0, 0x42, 0x00, 0x00, 0xb4, 0x42
        };

        var BE_REFERENCE_BYTES = new byte[]
        {
            0x50, 0x46, 0x0a, 0x33, 0x20, 0x32, 0x0a, 0x31, 0x2e, 0x30, 0x0a, 0x42,
            0xc8, 0x00, 0x00, 0x43, 0x48, 0x00, 0x00, 0x43, 0x96, 0x00, 0x00, 0x43,
            0xc8, 0x00, 0x00, 0x43, 0xfa, 0x00, 0x00, 0x44, 0x16, 0x00, 0x00, 0x44,
            0x2f, 0x00, 0x00, 0x44, 0x48, 0x00, 0x00, 0x44, 0x61, 0x00, 0x00, 0x41,
            0x20, 0x00, 0x00, 0x41, 0xa0, 0x00, 0x00, 0x41, 0xf0, 0x00, 0x00, 0x42,
            0x20, 0x00, 0x00, 0x42, 0x48, 0x00, 0x00, 0x42, 0x70, 0x00, 0x00, 0x42,
            0x8c, 0x00, 0x00, 0x42, 0xa0, 0x00, 0x00, 0x42, 0xb4, 0x00, 0x00
        };

        using (Stream reference_bytes = new MemoryStream(LE_REFERENCE_BYTES))
        {
            var img = new HDRImage(reference_bytes);
            
            Assert.Equal(2, img.Height);
            Assert.Equal(3, img.Width); 
            
            Assert.True(Color._AreCloseColor(img[0,0],new Color(10f, 20f, 30f)));
            Assert.True(Color._AreCloseColor(img[1,0],new Color(40f, 50f, 60f)));
            Assert.True(Color._AreCloseColor(img[2,0],new Color(70f, 80f, 90f)));
            Assert.True(Color._AreCloseColor(img[0,1],new Color(100f, 200f, 300f)));
            Assert.True(Color._AreCloseColor(img[1,1],new Color(400f, 500f, 600f)));
            Assert.True(Color._AreCloseColor(img[2,1],new Color(700f, 800f, 900f)));
        }
        
        using (Stream reference_bytes = new MemoryStream(BE_REFERENCE_BYTES))
        {
            var img = new HDRImage(reference_bytes);
                    
            Assert.Equal(2, img.Height);
            Assert.Equal(3, img.Width); 
                    
            Assert.True(Color._AreCloseColor(img[0,0],new Color(10f, 20f, 30f)));
            Assert.True(Color._AreCloseColor(img[1,0],new Color(40f, 50f, 60f)));
            Assert.True(Color._AreCloseColor(img[2,0],new Color(70f, 80f, 90f)));
            Assert.True(Color._AreCloseColor(img[0,1],new Color(100f, 200f, 300f)));
            Assert.True(Color._AreCloseColor(img[1,1],new Color(400f, 500f, 600f)));
            Assert.True(Color._AreCloseColor(img[2,1],new Color(700f, 800f, 900f)));
        }
        
    }
    
    [Fact]
    public void TestToLDR()
    {
        float gamma = 3.6f;

        HDRImage imageHDR = new HDRImage(3, 1);
        imageHDR[0] = new Color(0.883f, 0.2102f, 0.3775f);
        imageHDR[1] = new Color(0.2381f, 0.9324f, 0.4467f);
        imageHDR[2] = new Color(0.1941f, 0.5728f, 0.9483f);
        HDRImage imageLDR = imageHDR.ToLDR(gamma);
        imageHDR[0].Print();
        imageHDR[1].Print();
        imageHDR[2].Print();
        Assert.Equal(new Color(246, 165, 195), imageHDR[0]);
        Assert.Equal(new Color(171, 250, 204), imageHDR[1]);
        Assert.Equal(new Color(162, 218, 251), imageHDR[2]);
    }
    
    [Fact]
    public void TestClampImage()
    {
        var img = new HDRImage(2, 1);
        
        img[0,0] = new Color(0.5f, 1.0f, 1.5f);
        img[1,0] = new Color(50.0f, 100.0f, 150.0f);
        
        img.Clamp_Image();

        foreach (var pixel in img.Pixels)
        {
            Assert.True(pixel.R is >= 0 and <= 1);
            Assert.True(pixel.G is >= 0 and <= 1);
            Assert.True(pixel.B is >= 0 and <= 1);
        }
    }
}
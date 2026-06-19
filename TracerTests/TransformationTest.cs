using TracerLib;

namespace TracerTests;

public class HomMatrixTests
{
    [Fact]
    public void TestConstructor()
    {
        HomMatrix m = new HomMatrix();
        float[] identity =
        [
            1, 0, 0, 0,
            0, 1, 0, 0,
            0, 0, 1, 0,
            0, 0, 0, 1
        ];
        Assert.Equal(identity, m.M);
    }

    [Fact]
    public void TestConstructorWithArray_And_CheckHomogeneity()
    {
        float[] m =
        [
            6, 9, 2.440f, 20,
            283, 236.472f, 28, 9,
            2, 1, 4, 5,
            3, 69, 100.5f, 2
        ];

        Assert.Throws<ArgumentException>(() => new HomMatrix(m));
        
        m =
        [
            6, 9, 2.440f, 20,
            283, 236.472f, 28, 9,
            2, 1, 4, 5,
            0, 0, 0, 1
        ];
        HomMatrix matrix = new HomMatrix(m);
        Assert.Equal(m, matrix.M);
    }

    [Fact]
    public void TestConstructorTranslation()
    {
        Vector k = new Vector(1, 9, 183.48f);
        HomMatrix actual = new HomMatrix(k);
        float[] expectedArray =
        [
            1, 0, 0, k.X,
            0, 1, 0, k.Y,
            0, 0, 1, k.Z,
            0, 0, 0, 1
        ];
        Assert.Equal(expectedArray, actual.M);
    }

    [Fact]
    public void TestConstructorScale()
    {
        float x = 6.2f;
        float y = 4.93f;
        float z = 912.01f;
        HomMatrix actual = new HomMatrix(x, y, z);
        float[] expectedArray =
        [
            x, 0, 0, 0,
            0, y, 0, 0,
            0, 0, z, 0,
            0, 0, 0, 1
        ];
        HomMatrix expected = new HomMatrix(expectedArray);
        Assert.Equal(expected.M, actual.M);
    }
    
    [Fact]
    public void TestMatrixOffset()
    {
        float[] m =
        [
            1.0f, 2.0f, 3.0f, 4.0f,
            5.0f, 6.0f, 7.0f, 8.0f,
            9.0f, 9.0f, 8.0f, 7.0f,
            0, 0, 0, 1,
        ];

        HomMatrix matrix = new HomMatrix(m);

        Assert.Equal(9, matrix._MatrixOffset(2, 1));
        Assert.Equal(10, matrix._MatrixOffset(2, 2));
        Assert.Equal(15, matrix._MatrixOffset(3, 3));
        Assert.Throws<ArgumentOutOfRangeException>(() => matrix._MatrixOffset(-1, 16));
    }
    
    [Fact]
    public void Test1DIndex()
    {
        float[] m =
        [
            2, 8, 10.45f, 0.3f,
            4, 1, 9, 10,
            9, 6, 7, 1002,
            0, 0, 0, 1
        ];
        HomMatrix matrix = new HomMatrix(m);

        Assert.Equal(2, matrix[0]);
        Assert.Equal(0.3f, matrix[3]);
        Assert.Equal(1002, matrix[11]);
        Assert.Equal(9, matrix[8]);
        Assert.Equal(1, matrix[15]);
    }

    [Fact]
    public void Test2DIndex()
    {
        float[] m =
        [
            1.0f, 2.0f, 3.0f, 4.0f,
            5.0f, 6.0f, 7.0f, 8.0f,
            9.0f, 9.0f, 8.0f, 7.0f,
            0, 0, 0, 1,
        ];

        HomMatrix matrix = new HomMatrix(m);

        Assert.Equal(1.0f, matrix[0, 0]);
        Assert.Equal(7.0f, matrix[2, 3]);
        Assert.Equal(0, matrix[3, 2]);
    }
    
    [Fact]
    public void TestCheckCoordinates()
    {
        float[] m =
        [
            1.0f, 2.0f, 3.0f, 4.0f,
            5.0f, 6.0f, 7.0f, 8.0f,
            9.0f, 9.0f, 8.0f, 7.0f,
            0, 0, 0, 1
        ];
        HomMatrix matrix = new HomMatrix(m);

        Assert.Equal(4.0f, matrix[0, 3]);
        Assert.Throws<ArgumentOutOfRangeException>(() => matrix._CheckCoordinates(-1, 0));
        Assert.Throws<ArgumentOutOfRangeException>(() => matrix._CheckCoordinates(0, -1));
        Assert.Throws<ArgumentOutOfRangeException>(() => matrix._CheckCoordinates(16, 7));
        Assert.Throws<ArgumentOutOfRangeException>(() => matrix._CheckCoordinates(3, 16));
        Assert.Throws<ArgumentOutOfRangeException>(() => matrix._CheckCoordinates(19, 20));
        Assert.Throws<ArgumentOutOfRangeException>(() => matrix._CheckCoordinates(-17, -40));
    }
    
    [Fact]
    public void TestAreMatrixClose()
    {
        float[] m1 =
        [
            3, 5, 2, 8.3f,
            9.0f, 32.5920f, 9, 45,
            102.56921f, 39, 0, 0,
            0, 0, 0, 1
        ];

        float[] m2 =
        [
            3, 5, 2, 8.3f,
            9.0f, 32.5920f, 9, 45,
            102.56923f, 39, 0, 0,
            0, 0, 0, 1
        ];

        HomMatrix matrix1 = new HomMatrix(m1);
        HomMatrix matrix2 = new HomMatrix(m2);

        Assert.True(HomMatrix.AreMatricesClose(matrix1, matrix2, 1e-3f));
        Assert.False(HomMatrix.AreMatricesClose(matrix1, matrix2)); // epsilon = 1e-5f
    }

    [Fact]
    public void TestToString()
    {
        float[] m =
        [
            3, -23.25f, 9, 4,
            4, 6, 2, 7,
            -2, -10, -3.589f, 12,
            0, 0, 0, 1
        ];

        HomMatrix matrix = new HomMatrix(m);

        string str = "((3.00,\t-23.25,\t9.00,\t4.00)\n"
                     + "(4.00,\t6.00,\t2.00,\t7.00)\n"
                     + "(-2.00,\t-10.00,\t-3.59,\t12.00)\n"
                     + "(0.00,\t0.00,\t0.00,\t1.00))\n";
        Assert.Equal(str, matrix.ToString());
    }

    // test with optimized product
    [Fact]
    public void TestMatrixProduct()
    {
        float[] m1 =
        [
            2, 3, 1, 0.5f,
            3, -3, 0.25f, 1,
            1, 4, 7, 3,
            0, 0, 0, 1
        ];
        HomMatrix matrix1 = new HomMatrix(m1);

        float[] m2 =
        [
            2, 1, 0, 0,
            5, 2.5f, 1, 4,
            0.5f, 1, 3, 6,
            0, 0, 0, 1
        ];
        HomMatrix matrix2 = new HomMatrix(m2);

        float[] product =
        [
            19.5f, 10.5f, 6, 18.5f,
            -8.875f, -4.25f, -2.25f, -9.5f,
            25.5f, 18, 25, 61,
            0, 0, 0, 1
        ];
        HomMatrix productMatrix = new HomMatrix(product);

        Assert.Equal(productMatrix.M, (matrix1 * matrix2).M);
    }

    // test with old product function
    /*[Fact]
    public void TestMatrixProduct()
    {
        float[] m1 =
        [
            2, 3, 1, 0.5f,
            3, -3, 0.25f, 1,
            1, 4, 7, 3,
            21, 6, 3, 2
        ];
        HomMatrix matrix1 = new HomMatrix(m1);

        float[] m2 =
        [
            2, 1, 0, 0,
            5, 2.5f, 1, 4,
            0.5f, 1, 3, 6,
            0, 1, 6, 2
        ];
        HomMatrix matrix2 = new HomMatrix(m2);

        float[] product =
        [
            19.5f, 11, 9, 19,
            -8.875f, -3.25f, 3.75f, -8.5f,
            25.5f, 21, 43, 64,
            73.5f, 41, 27, 46
        ];
        HomMatrix productMatrix = new HomMatrix(product);

        Assert.Equal(productMatrix.M, (matrix1 * matrix2).M);
    }*/
}

public class TransformationTest
{
    /*private readonly ITestOutputHelper _testOutputHelper;

    public TransformationTest(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }*/

    [Fact]
    public void TestConstructor()
    {
        float[] identity =
        [
            1, 0, 0, 0,
            0, 1, 0, 0,
            0, 0, 1, 0,
            0, 0, 0, 1
        ];

        HomMatrix identityMatrix = new HomMatrix(identity);
        Transformation t = new Transformation();
        Assert.Equal(identityMatrix.M, t.M.M);
        Assert.Equal(identityMatrix.M, t.InvM.M);
    }

    [Fact]
    public void TestConstructorWithInverse()
    {
        float[] m =
        [
            3, -23.25f, 9, 4,
            4, 6, 2, 7,
            -2, -10, -2, 12,
            0, 0, 0, 1
        ];
        HomMatrix matrix = new HomMatrix(m);

        float[] invM =
        [
            -0.0249221f, 0.425234f, 0.313084f, -6.63396f,
            -0.0124611f, -0.0373832f, -0.0934579f, 1.43302f,
            0.0872274f, -0.238318f, -0.345794f, 5.46885f,
            0, 0, 0, 1
        ];
        
        HomMatrix invMatrix = new HomMatrix(invM);

        Transformation t = new Transformation(matrix, invMatrix);
        Assert.Equal(matrix, t.M);
        Assert.Equal(invMatrix, t.InvM);
    }

    [Fact]
    public void TestConstructorWithArrays()
    {
        float[] m =
        [
            3, -23.25f, 9, 4,
            4, 6, 2, 7,
            -2, -10, -2, 12,
            0, 0, 0, 1
        ];

        float[] invM =
        [
            -0.0249221f, 0.425234f, 0.313084f, -6.63396f,
            -0.0124611f, -0.0373832f, -0.0934579f, 1.43302f,
            0.0872274f, -0.238318f, -0.345794f, 5.46885f,
            0, 0, 0, 1
        ]; 
        ;
        Transformation t = new Transformation(m, invM);
        Assert.Equal(m, t.M.M);
        Assert.Equal(invM, t.InvM.M);
    }

    [Fact]
    public void TestConstructorTranslation()
    {
        Vector k = new Vector(192.4f, 36, 183.48f);
        float[] expectedArray =
        [
            1, 0, 0, k.X,
            0, 1, 0, k.Y,
            0, 0, 1, k.Z,
            0, 0, 0, 1
        ];
        float[] expectedArrayInv =
        [
            1, 0, 0, -k.X,
            0, 1, 0, -k.Y,
            0, 0, 1, -k.Z,
            0, 0, 0, 1
        ];

        Transformation t = new Transformation(k);
        Assert.Equal(expectedArray, t.M.M);
        Assert.Equal(expectedArrayInv, t.InvM.M);
    }

    [Fact]
    public void TestConstructorScaling()
    {
        float x = 6.2f;
        float y = 4.932f;
        float z = 912.01f;
        float[] expectedArray =
        [
            x, 0, 0, 0,
            0, y, 0, 0,
            0, 0, z, 0,
            0, 0, 0, 1
        ];
        float[] expectedArrayInv =
        [
            1 / x, 0, 0, 0,
            0, 1 / y, 0, 0,
            0, 0, 1 / z, 0,
            0, 0, 0, 1
        ];

        Transformation t = new Transformation(x, y, z);
        Assert.Equal(expectedArray, t.M.M);
        Assert.Equal(expectedArrayInv, t.InvM.M);

        Assert.Throws<ArgumentOutOfRangeException>(() => new Transformation(0, 1, 3));
        Assert.Throws<ArgumentOutOfRangeException>(() => new Transformation(3, 0, 23));
        Assert.Throws<ArgumentOutOfRangeException>(() => new Transformation(302, 29, 0));
        Assert.Throws<ArgumentOutOfRangeException>(() => new Transformation(0, 0, 0));
    }

    [Fact]
    public void TestConstructorRotationMainAxes()
    {
        Axis axis = Axis.Z;
        float alpha = MathF.PI / 2;
        float[] m =
        [
            0, -1, 0, 0,
            1, 0, 0, 0,
            0, 0, 1, 0,
            0, 0, 0, 1
        ];
        float[] invM =
        [
            0, 1, 0, 0,
            -1, 0, 0, 0,
            0, 0, 1, 0,
            0, 0, 0, 1
        ];

        Transformation t = new Transformation(axis, alpha);
        Assert.True(Functions.AreArraysClose(m, t.M.M));
        Assert.True(Functions.AreArraysClose(invM, t.InvM.M));

        axis = Axis.Y;
        alpha = MathF.PI;
        m = invM =
        [
            -1, 0, 0, 0,
            0, 1, 0, 0,
            0, 0, -1, 0,
            0, 0, 0, 1
        ];
        t = new Transformation(axis, alpha);
        Assert.True(Functions.AreArraysClose(m, t.M.M));
        Assert.True(Functions.AreArraysClose(invM, t.InvM.M));

        axis = Axis.X;
        alpha = MathF.PI * 1.5f;
        m =
        [
            1, 0, 0, 0,
            0, 0, 1, 0,
            0, -1, 0, 0,
            0, 0, 0, 1
        ];
        invM =
        [
            1, 0, 0, 0,
            0, 0, -1, 0,
            0, 1, 0, 0,
            0, 0, 0, 1
        ];
        t = new Transformation(axis, alpha);
        Assert.True(Functions.AreArraysClose(m, t.M.M));
        Assert.True(Functions.AreArraysClose(invM, t.InvM.M));
    }

    [Fact]
    public void Test1DIndex()
    {
        float[] mat =
        [
            1.0f, 2.0f, 3.0f, 4.0f,
            5.0f, 6.0f, 7.0f, 8.0f,
            9.0f, 9.0f, 8.0f, 7.0f,
            0, 0, 0, 1f,
        ];

        float[] invMat =
        [
            -3.75f, 2.75f, -1, 0,
            5.75f, -4.75f, 2.0f, 1f,
            -2.25f, 2.25f, -1.0f, -2.0f,
            0, 0, 0, 1
        ];
        Transformation t = new Transformation(mat, invMat);

        Assert.Equal(6.0f, t[5]);
        Assert.Equal(7.0f, t[11]);
    }

    [Fact]
    public void Test2DIndex()
    {
        float[] mat =
        [
            1.0f, 2.0f, 3.0f, 4.0f,
            5.0f, 6.0f, 7.0f, 8.0f,
            9.0f, 9.0f, 8.0f, 7.0f,
            0, 0, 0, 1f,
        ];

        float[] invMat =
        [
            -3.75f, 2.75f, -1, 0,
            5.75f, -4.75f, 2.0f, 1f,
            -2.25f, 2.25f, -1.0f, -2.0f,
            0, 0, 0, 1
        ];
        Transformation t = new Transformation(mat, invMat);

        Assert.Equal(1.0f, t[0, 0]);
        Assert.Equal(7.0f, t[2, 3]);
        Assert.Equal(0f, t[3, 2]);
    }

    /*
    [Fact]
    public void TestIsConsistent()
    {
        float[] m =
        [
            3, -23.25f, 9, 4,
            4, 6, 2, 7,
            -2, -10, -2, 12,
            1.4f, 17, 0, 3.589f
        ];

        float[] invM =
        [
            -0.112625f, 0.410949f, -0.0958651f, -0.355464f,
            0.00648398f, -0.0342974f, -0.00511954f, 0.0767848f,
            0.159527f, -0.226542f, -0.00866817f, 0.293034f,
            0.0132203f, 0.00215331f, 0.0616448f, 0.0535824f
        ];

        Transformation t1 = new Transformation(m, invM);
        Assert.True(t1._IsConsistent());
    }*/

    [Fact]
    public void TestCheckConsistency()
    {
        float[] m1 =
        [
            1, 2, 3, 4,
            5, 6, 7, 8,
            9, 10, 11, 12,
            0, 0, 0, 1
        ];
        float[] invM1 =
        [
            0, 0, 0, 0,
            0, 0, 0, 0,
            0, 0, 0, 0,
            0, 0, 0, 1
        ];
        Assert.Throws<ArgumentException>(() => new Transformation(m1, invM1));

        float[] m2 =
        [
            3, -23.25f, 9, 4,
            4, 6, 2, 7,
            -2, -10, -2, 12,
            0, 0, 0, 1
        ];

        float[] invM2 =
        [
            -0.0249221f, 0.425234f, 0.313084f, -6.63396f,
            -0.0124611f, -0.0373832f, -0.0934579f, 1.43302f,
            0.0872274f, -0.238318f, -0.345794f, 5.46885f,
            0, 0, 0, 1
        ]; 
        
        Transformation t1 = new Transformation(m2, invM2);
        t1._CheckConsistency();
    }

    [Fact]
    public void TestAreTransformationsClose()
    {
        float[] m =
        [
            1.0f, 2.0f, 3.0f, 4.0f,
            5.0f, 6.0f, 7.0f, 8.0f,
            9.0f, 9.0f, 8.0f, 7.0f,
            0, 0, 0, 1.0f,
        ];
        float[] invM =
        [
            -3.75f, 2.75f, -1, 0,
            5.75f, -4.75f, 2.0f, 1f,
            -2.25f, 2.25f, -1.0f, -2.0f,
            0, 0, 0, 1
        ];

        Transformation t1 = new Transformation(m, invM);
        Transformation t2 = new Transformation(m, invM);

        Assert.Equal(t1.M.M, t2.M.M);

        float[] mat =
        [
            3, -23.25f, 9, 4,
            4, 6, 2, 7,
            -2, -10, -2, 12,
            0, 0, 0, 1.0f
        ];

        float[] invMat =
        [
            -0.0249221f, 0.425234f, 0.313084f, -6.63396f,
            -0.0124611f, -0.0373832f, -0.0934579f, 1.43302f,
            0.0872274f, -0.238318f, -0.345794f, 5.46885f,
            0, 0, 0, 1
        ];
        Transformation t3 = new Transformation(mat, invMat);

        Assert.False(Transformation.AreTransformationsClose(t1, t3));
    }

    [Fact]
    public void TestToString()
    {
        float[] m =
        [
            3, -23.25f, 9, 4,
            4, 6, 2, 7,
            -2, -10, -2, 12,
            0, 0, 0, 1.0f
        ];

        float[] invM =
        [
            -0.0249221f, 0.425234f, 0.313084f, -6.63396f,
            -0.0124611f, -0.0373832f, -0.0934579f, 1.43302f,
            0.0872274f, -0.238318f, -0.345794f, 5.46885f,
            0, 0, 0, 1
        ];
        Transformation t = new Transformation(m, invM);

        string str = "Transformation matrix:\n" +
                     "((3.00,\t-23.25,\t9.00,\t4.00)\n" +
                     "(4.00,\t6.00,\t2.00,\t7.00)\n" +
                     "(-2.00,\t-10.00,\t-2.00,\t12.00)\n" +
                     "(0.00,\t0.00,\t0.00,\t1.00))\n" +
                     "\nInverse Matrix:\n" +
                     "((-0.02,\t0.43,\t0.31,\t-6.63)\n" +
                     "(-0.01,\t-0.04,\t-0.09,\t1.43)\n" +
                     "(0.09,\t-0.24,\t-0.35,\t5.47)\n" +
                     "(0.00,\t0.00,\t0.00,\t1.00))\n";
                     
        Assert.Equal(str, t.ToString());
    }

    [Fact]
    public void TestInverse()
    {
        float[] m1 =
        [
            1.0f, 2.0f, 3.0f, 4.0f,
            5.0f, 6.0f, 7.0f, 8.0f,
            9.0f, 9.0f, 8.0f, 7.0f,
            0, 0, 0, 1.0f,
        ];
        float[] invM1 =
        [
            -3.75f, 2.75f, -1, 0,
            5.75f, -4.75f, 2.0f, 1f,
            -2.25f, 2.25f, -1.0f, -2.0f,
            0, 0, 0, 1
        ];
        Transformation t1 = new Transformation(m1, invM1);

        float[] m2 =
        [
            -3.75f, 2.75f, -1, 0,
            5.75f, -4.75f, 2.0f, 1f,
            -2.25f, 2.25f, -1.0f, -2.0f,
            0, 0, 0, 1
        ];

        float[] invM2 =
        [
            1.0f, 2.0f, 3.0f, 4.0f,
            5.0f, 6.0f, 7.0f, 8.0f,
            9.0f, 9.0f, 8.0f, 7.0f,
            0f, 0f, 0, 1.0f,
        ];
        Transformation t2 = new Transformation(m2, invM2);

        Assert.True(Transformation.AreTransformationsClose(t1, t2.Inverse()));
    }

    [Fact]
    public void TestProductTransformations()
    {
        float[] m1 =
        [
            1.0f, 2.0f, 3.0f, 4.0f,
            5.0f, 6.0f, 7.0f, 8.0f,
            9.0f, 9.0f, 8.0f, 7.0f,
            0, 0, 0, 1.0f
        ];
        float[] invM1 =
        [
            -3.75f, 2.75f, -1, 0,
            5.75f, -4.75f, 2.0f, 1f,
            -2.25f, 2.25f, -1.0f, -2.0f,
            0, 0, 0, 1
        ];
        Transformation t1 = new Transformation(m1, invM1);

        float[] m2 =
        [
            3.0f, 5.0f, 2.0f, 4.0f,
            4.0f, 1.0f, 0.0f, 5.0f,
            6.0f, 3.0f, 2.0f, 0.0f,
            0f, 0f, 0f, 1.0f
        ];
        float[] invM2 =
        [
            -0.0909091f, 0.181818f, 0.0909091f, -0.545455f,
            0.363636f, 0.272727f, -0.363636f, -2.81818f,
            -0.272727f, -0.954545f, 0.772727f, 5.86364f,
            0, 0, 0, 1.0f
        ];
        Transformation t2 = new Transformation(m2, invM2);

        float[] product =
        [
            29, 16, 8, 18,
            81, 52, 24, 58,
            111, 78, 34, 88,
            0, 0, 0, 1
        ];
        float[] invProduct =
        [
            1.18181818f, -0.90909090f, 0.36363636f, -0.54545454f,
            1.02272727f, -1.11363636f, 0.54545454f, -1.81818181f,
            -6.2045454f, 5.522727272f, -2.4090909f, 3.36363636f,
            0, 0, 0, 1
        ];
        Transformation expectedT3 = new Transformation(product, invProduct);
        ;
        Transformation actualT3 = t1 * t2;
        Assert.True(Transformation.AreTransformationsClose(expectedT3, actualT3));
    }

    [Fact]
    public void TestProductVector()
    {
        Vector v = new Vector(1, 2, 3);
        float[] m =
        [
            1.0f, 2.0f, 3.0f, 4.0f,
            5.0f, 6.0f, 7.0f, 8.0f,
            9.0f, 9.0f, 8.0f, 7.0f,
            0.0f, 0.0f, 0.0f, 1.0f,
        ];
        float[] invM =
        [
            -3.75f, 2.75f, -1, 0,
            5.75f, -4.75f, 2.0f, 1.0f,
            -2.25f, 2.25f, -1.0f, -2.0f,
            0.0f, 0.0f, 0.0f, 1.0f,
        ];
        Transformation t = new Transformation(m, invM);

        Vector expected = new Vector(14.0f, 38.0f, 51.0f);

        Assert.Equal(expected, t * v);
    }
    
    [Fact]
    public void TestProductPoint()
    {
        Point p = new Point(1, 2, 3);
        float[] m =
        [
            1.0f, 2.0f, 3.0f, 4.0f,
            5.0f, 6.0f, 7.0f, 8.0f,
            9.0f, 9.0f, 8.0f, 7.0f,
            0.0f, 0.0f, 0.0f, 1.0f,
        ];
        float[] invM =
        [
            -3.75f, 2.75f, -1, 0,
            5.75f, -4.75f, 2.0f, 1.0f,
            -2.25f, 2.25f, -1.0f, -2.0f,
            0.0f, 0.0f, 0.0f, 1.0f,
        ];
        Transformation t = new Transformation(m, invM);

        Point expected = new Point(18.0f, 46.0f, 58.0f);

        Assert.Equal(expected, t * p);
    }

    [Fact]
    public void TestProductNormal()
    {
        Normal n = new Normal(3, 2, 4);
        float[] m =
        [
            1.0f, 2.0f, 3.0f, 4.0f,
            5.0f, 6.0f, 7.0f, 8.0f,
            9.0f, 9.0f, 8.0f, 7.0f,
            0.0f, 0.0f, 0.0f, 1.0f,
        ];
        float[] invM =
        [
            -3.75f, 2.75f, -1, 0,
            5.75f, -4.75f, 2.0f, 1.0f,
            -2.25f, 2.25f, -1.0f, -2.0f,
            0.0f, 0.0f, 0.0f, 1.0f,
        ];
        Transformation t = new Transformation(m, invM);
        Normal expected = new Normal(-8.75f, 7.75f, -3.0f);
        Assert.Equal(expected, t * n);
    }
}
using TracerLib;

namespace TransformationTests;

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
    public void TestConstructorWithArray()
    {
        float[] m =
        [
            6, 9, 2.440f, 20,
            283, 236.472f, 28, 9,
            2, 1, 4, 5,
            3, 69, 100.5f, 2
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
        HomMatrix expected = new HomMatrix(expectedArray);
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
    public void Test1DIndex()
    {
        float[] m =
        [
            2, 8, 10.45f, 0.3f,
            4, 1, 9, 10,
            9, 6, 7, 1002,
            1, 0, 0, 4.0f
        ];
        HomMatrix matrix = new HomMatrix(m);

        Assert.Equal(2, matrix[0]);
        Assert.Equal(0.3f, matrix[3]);
        Assert.Equal(1002, matrix[11]);
        Assert.Equal(9, matrix[8]);
        Assert.Equal(4.0f, matrix[15]);
    }

    [Fact]
    public void TestAreMatrixClose()
    {
        float[] m1 =
        [
            3, 5, 2, 8.3f,
            9.0f, 32.5920f, 9, 45,
            102.56921f, 39, 0, 0,
            1, 2, 5, 7
        ];

        float[] m2 =
        [
            3, 5, 2, 8.3f,
            9.0f, 32.5920f, 9, 45,
            102.56921f, 39, 0, 0,
            1, 2, 5, 7.0001f
        ];

        HomMatrix matrix1 = new HomMatrix(m1);
        HomMatrix matrix2 = new HomMatrix(m2);

        Assert.True(HomMatrix.AreMatrixClose(matrix1, matrix2, 1e-3f));
        Assert.False(HomMatrix.AreMatrixClose(matrix1, matrix2)); // epsilon = 1e-5f
    }

    [Fact]
    public void TestCheckCoordinates()
    {
        float[] m =
        [
            1.0f, 2.0f, 3.0f, 4.0f,
            5.0f, 6.0f, 7.0f, 8.0f,
            9.0f, 9.0f, 8.0f, 7.0f,
            6.0f, 5.0f, 4.0f, 1.0f,
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
    public void TestMatrixOffset()
    {
        float[] m =
        [
            1.0f, 2.0f, 3.0f, 4.0f,
            5.0f, 6.0f, 7.0f, 8.0f,
            9.0f, 9.0f, 8.0f, 7.0f,
            6.0f, 5.0f, 4.0f, 1.0f,
        ];

        HomMatrix matrix = new HomMatrix(m);

        Assert.Equal(9, matrix._MatrixOffset(2, 1));
        Assert.Equal(10, matrix._MatrixOffset(2, 2));
        Assert.Equal(15, matrix._MatrixOffset(3, 3));
    }

    [Fact]
    public void Test2DIndex()
    {
        float[] m =
        [
            1.0f, 2.0f, 3.0f, 4.0f,
            5.0f, 6.0f, 7.0f, 8.0f,
            9.0f, 9.0f, 8.0f, 7.0f,
            6.0f, 5.0f, 4.0f, 1.0f,
        ];

        HomMatrix matrix = new HomMatrix(m);

        Assert.Equal(1.0f, matrix[0, 0]);
        Assert.Equal(7.0f, matrix[2, 3]);
        Assert.Equal(4.0f, matrix[3, 2]);
    }

    [Fact]
    public void TestToString()
    {
        float[] m =
        [
            3, -23.25f, 9, 4,
            4, 6, 2, 7,
            -2, -10, -2, 12,
            1.4f, 17, 0, 3.589f
        ];

        HomMatrix matrix = new HomMatrix(m);

        string str = "((3.00,\t-23.25,\t9.00,\t4.00)\n"
                     + "(4.00,\t6.00,\t2.00,\t7.00)\n"
                     + "(-2.00,\t-10.00,\t-2.00,\t12.00)\n"
                     + "(1.40,\t17.00,\t0.00,\t3.59))\n";
        Assert.Equal(str, matrix.ToString());
    }

    [Fact]
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
    }
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
            1.4f, 17, 0, 3.589f
        ];
        HomMatrix matrix = new HomMatrix(m);

        float[] invM =
        [
            -0.112625f, 0.410949f, -0.0958651f, -0.355464f,
            0.00648398f, -0.0342974f, -0.00511954f, 0.0767848f,
            0.159527f, -0.226542f, -0.00866817f, 0.293034f,
            0.0132203f, 0.00215331f, 0.0616448f, 0.0535824f
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
            1.4f, 17, 0, 3.589f
        ];

        float[] invM =
        [
            -0.112625f, 0.410949f, -0.0958651f, -0.355464f,
            0.00648398f, -0.0342974f, -0.00511954f, 0.0767848f,
            0.159527f, -0.226542f, -0.00866817f, 0.293034f,
            0.0132203f, 0.00215331f, 0.0616448f, 0.0535824f
        ];

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
        char axis = 'z';
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
        Assert.True(Functions.AreArrayClose(m, t.M.M));
        Assert.True(Functions.AreArrayClose(invM, t.InvM.M));

        axis = 'y';
        alpha = MathF.PI;
        m = invM =
        [
            -1, 0, 0, 0,
            0, 1, 0, 0,
            0, 0, -1, 0,
            0, 0, 0, 1
        ];
        t = new Transformation(axis, alpha);
        Assert.True(Functions.AreArrayClose(m, t.M.M));
        Assert.True(Functions.AreArrayClose(invM, t.InvM.M));

        axis = 'x';
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
        Assert.True(Functions.AreArrayClose(m, t.M.M));
        Assert.True(Functions.AreArrayClose(invM, t.InvM.M));

        Assert.Throws<ArgumentOutOfRangeException>(() => new Transformation('a', alpha));
    }

    [Fact]
    public void Test1DIndex()
    {
        float[] mat =
        [
            1.0f, 2.0f, 3.0f, 4.0f,
            5.0f, 6.0f, 7.0f, 8.0f,
            9.0f, 9.0f, 8.0f, 7.0f,
            6.0f, 5.0f, 4.0f, 1.0f,
        ];

        float[] invMat =
        [
            -3.75f, 2.75f, -1, 0,
            4.375f, -3.875f, 2.0f, -0.5f,
            0.5f, 0.5f, -1.0f, 1.0f,
            -1.375f, 0.875f, 0.0f, -0.5f
        ];
        Transformation t = new Transformation(mat, invMat);

        Assert.Equal(6.0, t[5]);
        Assert.Equal(4.0, t[14]);
    }

    [Fact]
    public void Test2DIndex()
    {
        float[] mat =
        [
            1.0f, 2.0f, 3.0f, 4.0f,
            5.0f, 6.0f, 7.0f, 8.0f,
            9.0f, 9.0f, 8.0f, 7.0f,
            6.0f, 5.0f, 4.0f, 1.0f,
        ];

        float[] invMat =
        [
            -3.75f, 2.75f, -1, 0,
            4.375f, -3.875f, 2.0f, -0.5f,
            0.5f, 0.5f, -1.0f, 1.0f,
            -1.375f, 0.875f, 0.0f, -0.5f
        ];
        Transformation t = new Transformation(mat, invMat);

        Assert.Equal(1.0f, t[0, 0]);
        Assert.Equal(7.0f, t[2, 3]);
        Assert.Equal(4.0f, t[3, 2]);
    }

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
    }

    [Fact]
    public void TestCheckConsistency()
    {
        float[] m1 =
        [
            1, 2, 3, 4,
            5, 6, 7, 8,
            9, 10, 11, 12,
            13, 14, 15, 16
        ];
        float[] invM1 =
        [
            0, 0, 0, 0,
            0, 0, 0, 0,
            0, 0, 0, 0,
            0, 0, 0, 0
        ];
        Assert.Throws<ArgumentException>(() => new Transformation(m1, invM1));

        float[] m2 =
        [
            3, -23.25f, 9, 4,
            4, 6, 2, 7,
            -2, -10, -2, 12,
            1.4f, 17, 0, 3.589f
        ];
        float[] invM2 =
        [
            -0.112625f, 0.410949f, -0.0958651f, -0.355464f,
            0.00648398f, -0.0342974f, -0.00511954f, 0.0767848f,
            0.159527f, -0.226542f, -0.00866817f, 0.293034f,
            0.0132203f, 0.00215331f, 0.0616448f, 0.0535824f
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
            6.0f, 5.0f, 4.0f, 1.0f,
        ];
        float[] invM =
        [
            -3.75f, 2.75f, -1, 0,
            4.375f, -3.875f, 2.0f, -0.5f,
            0.5f, 0.5f, -1.0f, 1.0f,
            -1.375f, 0.875f, 0.0f, -0.5f,
        ];

        Transformation t1 = new Transformation(m, invM);
        Transformation t2 = new Transformation(m, invM);

        Assert.Equal(t1.M.M, t2.M.M);

        float[] mat =
        [
            3, -23.25f, 9, 4,
            4, 6, 2, 7,
            -2, -10, -2, 12,
            1.4f, 17, 0, 3.589f
        ];

        float[] invMat =
        [
            -0.112625f, 0.410949f, -0.0958651f, -0.355464f,
            0.00648398f, -0.0342974f, -0.00511954f, 0.0767848f,
            0.159527f, -0.226542f, -0.00866817f, 0.293034f,
            0.0132203f, 0.00215331f, 0.0616448f, 0.0535824f
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
            1.4f, 17, 0, 3.589f
        ];

        float[] invM =
        [
            -0.112625f, 0.410949f, -0.0958651f, -0.355464f,
            0.00648398f, -0.0342974f, -0.00511954f, 0.0767848f,
            0.159527f, -0.226542f, -0.00866817f, 0.293034f,
            0.0132203f, 0.00215331f, 0.0616448f, 0.0535824f
        ];
        Transformation t = new Transformation(m, invM);

        string str = "((3.00,\t-23.25,\t9.00,\t4.00)\n"
                     + "(4.00,\t6.00,\t2.00,\t7.00)\n"
                     + "(-2.00,\t-10.00,\t-2.00,\t12.00)\n"
                     + "(1.40,\t17.00,\t0.00,\t3.59))\n";
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
            6.0f, 5.0f, 4.0f, 1.0f,
        ];
        float[] invM1 =
        [
            -3.75f, 2.75f, -1, 0,
            4.375f, -3.875f, 2.0f, -0.5f,
            0.5f, 0.5f, -1.0f, 1.0f,
            -1.375f, 0.875f, 0.0f, -0.5f,
        ];
        Transformation t1 = new Transformation(m1, invM1);

        float[] m2 =
        [
            -3.75f, 2.75f, -1, 0,
            4.375f, -3.875f, 2.0f, -0.5f,
            0.5f, 0.5f, -1.0f, 1.0f,
            -1.375f, 0.875f, 0.0f, -0.5f,
        ];

        float[] invM2 =
        [
            1.0f, 2.0f, 3.0f, 4.0f,
            5.0f, 6.0f, 7.0f, 8.0f,
            9.0f, 9.0f, 8.0f, 7.0f,
            6.0f, 5.0f, 4.0f, 1.0f,
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
            6.0f, 5.0f, 4.0f, 1.0f
        ];
        float[] invM1 =
        [
            -3.75f, 2.75f, -1, 0,
            4.375f, -3.875f, 2.0f, -0.5f,
            0.5f, 0.5f, -1.0f, 1.0f,
            -1.375f, 0.875f, 0.0f, -0.5f
        ];
        Transformation t1 = new Transformation(m1, invM1);

        float[] m2 =
        [
            3.0f, 5.0f, 2.0f, 4.0f,
            4.0f, 1.0f, 0.0f, 5.0f,
            6.0f, 3.0f, 2.0f, 0.0f,
            1.0f, 4.0f, 2.0f, 1.0f
        ];
        float[] invM2 =
        [
            0.4f, -0.2f, 0.2f, -0.6f,
            2.9f, -1.7f, 0.2f, -3.1f,
            -5.55f, 3.15f, -0.4f, 6.45f,
            -0.9f, 0.7f, -0.2f, 1.1f
        ];
        Transformation t2 = new Transformation(m2, invM2);

        float[] product =
        [
            33.0f, 32.0f, 16.0f, 18.0f,
            89.0f, 84.0f, 40.0f, 58.0f,
            118.0f, 106.0f, 48.0f, 88.0f,
            63.0f, 51.0f, 22.0f, 50.0f
        ];
        float[] invProduct =
        [
            -1.45f, 1.45f, -1.0f, 0.6f,
            -13.95f, 11.95f, -6.5f, 2.6f,
            25.525f, -22.025f, 12.25f, -5.2f,
            4.825f, -4.325f, 2.5f, -1.1f
        ];
        Transformation t3 = new Transformation(product, invProduct);

        Assert.True(Transformation.AreTransformationsClose(t3, t1 * t2));
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
        //are close se non funziona
        /*

           expected_n = Normal(-8.75, 7.75, -3.0)
           assert expected_n.is_close(m * Normal(3.0, 2.0, 4.0))
         */
    }

/*
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
        //are close se non funziona
    }

    [Fact]
    public void TestProductNormal()
    {
        Normal n = new Normal(1, 2, 3);
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
        //are close se non funziona
    }
    */
}
// This file is release under EUPL_v1.2 license. See LICENSE.md

using System.Globalization;

namespace TracerLib;

/// <summary>
/// A Homogeneous Matrix is a 4x4 matrix with the last row = (0,0,0,1)
/// It allows to represent scaling transformations, rotations and translations in 3D space.
/// The matrix is indexed in row-major order.
/// </summary>
public struct HomMatrix
{
    /// <summary>
    /// The 1D array of the coefficients of the matrix.
    /// The array contains 16 elements stored in row-major order:
    /// [ m00, m01, m02, m03,
    ///   m10, m11, m12, m13,
    ///   m20, m21, m22, m23,
    ///   m30, m31, m32, m33 ].
    /// </summary>
    public float[] M { get; private set; }

    //Constructors - Begin

    #region Constructors

    /// <summary>
    /// Constructs a 4x4 identity matrix.
    /// </summary>
    public HomMatrix()
    {
        M =
        [
            1, 0, 0, 0,
            0, 1, 0, 0,
            0, 0, 1, 0,
            0, 0, 0, 1
        ];
    }

    public HomMatrix(float[] m)
    {
        M = new float[16];
        for (int i = 0; i < 16; i++)
        {
            M[i] = m[i];
        }

        _CheckHomogeneity();
    }

    /// <summary>
    /// Initializes a translation matrix using the specified translation vector.
    /// </summary>
    /// <param name="k">The translation vector (x, y, z).</param>
    public HomMatrix(Vector k)
    {
        M =
        [
            1, 0, 0, k.X,
            0, 1, 0, k.Y,
            0, 0, 1, k.Z,
            0, 0, 0, 1
        ];
        _CheckHomogeneity();
    }

    /// <summary>
    /// Constructs a scaling matrix using the specified scaling factors x,y,z.
    /// </summary>
    /// <param name="scaleX"></param>
    /// <param name="scaleY"></param>
    /// <param name="scaleZ"></param>
    public HomMatrix(float scaleX, float scaleY, float scaleZ)
    {
        M =
        [
            scaleX, 0, 0, 0,
            0, scaleY, 0, 0,
            0, 0, scaleZ, 0,
            0, 0, 0, 1
        ];

        _CheckHomogeneity();
    }

    /*
    /// <summary>
    /// Constructs a general rotation transformation,
    /// with the axis and angle of rotation passed as arguments.
    /// </summary>
    /// <param name="axis"></param>
    /// <param name="angle"></param>
    /*public Transformation(Vector axis, float angle)
    {
    }*/

    #endregion

    //Constructors - End

    /// <summary>
    /// Validates that the last row of the homogeneous matrix is (0,0,0,1).
    /// </summary>
    /// <exception cref="ArgumentException">Thrown when the last row is not equal to (0,0,0,1).</exception>
    public void _CheckHomogeneity()
    {
        if (M[12] != 0 || M[13] != 0 || M[14] != 0 || M[15] != 1)
        {
            throw new ArgumentException("The matrix must have the last row (0,0,0,1) to be homogeneous.");
        }
    }

    /// <summary>
    /// Returns the index in a 1D array corresponding to the specified matrix column and row.
    /// </summary>
    /// <param name="col">The column index.</param>
    /// <param name="row">The row index.</param>
    /// <returns>The computed 1D array index.</returns>
    public int _MatrixOffset(int row, int col)
    {
        _CheckCoordinates(row, col);
        return row * 4 + col;
    }

    /// <summary>
    /// 1D index for the M matrix
    /// (coefficients are stored in row-major order).
    /// Valid range: 0–15.
    /// </summary>
    /// <param name="index"></param>
    public float this[Index index]
    {
        get => M[index];
        private set => M[index] = value;
    }

    public float this[int row, int col]
    {
        get => M[_MatrixOffset(row, col)];
        set => M[_MatrixOffset(row, col)] = value;
    }

    /// <summary>
    /// Validates that the specified row and column indices are within the valid range [0, 15].
    /// </summary>
    /// <param name="row">The row index to validate.</param>
    /// <param name="col">The column index to validate.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when either the row or column is outside the valid range [0, 15].
    /// </exception>
    public void _CheckCoordinates(int row, int col)
    {
        if (col < 0 || col > 15)
        {
            throw new ArgumentOutOfRangeException(nameof(col), col,
                nameof(col) + $" must be non-negative and less than 16");
        }

        if (row < 0 || row > 15)
        {
            throw new ArgumentOutOfRangeException(nameof(row), row,
                nameof(row) + $" must be non-negative and less than 16");
        }
    }

    /// <summary>
    /// Determines whether two homogeneous matrices are approximately equal within a given tolerance.
    /// </summary>
    /// <param name="a">The first matrix to compare.</param>
    /// <param name="b">The second matrix to compare.</param>
    /// <param name="epsilon">The tolerance used when comparing each coefficient of the matrices.</param>
    /// <returns>
    /// /// True if the absolute difference between corresponding coefficients of the two matrices
    /// is less than or equal to the specified tolerance; otherwise, false.
    /// </returns>
    public static bool AreMatricesClose(HomMatrix a, HomMatrix b, float epsilon = 1e-5f)
    {
        return Functions.AreArraysClose(a.M, b.M, epsilon);
    }

    public override string ToString()
    {
        string str = "(";
        for (int i = 0; i < 4; i++)
        {
            str += "(";
            for (int j = 0; j < 4; j++)
            {
                // F2 for the fixed point format
                str += this[i, j].ToString("F2", CultureInfo.InvariantCulture);
                if (j < 3)
                {
                    str += ",\t";
                }
            }

            if (i < 3)
            {
                str += ")\n";
            }
            else
            {
                str += ")";
            }
        }

        str += ")\n";
        return str;
    }

    public void Print()
    {
        Console.WriteLine(ToString());
    }

    /*
    /// <summary>
    /// Returns the inverse of the matrix passed as argument using the Gauss elimination method
    /// </summary>
    /// <param name="mat"></param>
    /// <returns></returns>
    /*public static float[] ComputeInverse(float[] mat)
    {
        float determinant = Determinant();
        if(determinant == 0){
            throw new Exception;
        }
        return invMatrix;
    }*/

    /// <summary>
    /// Returns the product of two homogeneous transformation matrices.
    /// The computation is optimized by exploiting the fact that the last row
    /// of a homogeneous matrix is always (0, 0, 0, 1).
    /// </summary>
    /// <param name="m1">The first matrix to multiply.</param>
    /// <param name="m2">The second matrix to multiply.</param>
    /// <returns></returns>
    public static HomMatrix operator *(HomMatrix m1, HomMatrix m2)
    {
        float[] m3 =
        [
            0, 0, 0, 0,
            0, 0, 0, 0,
            0, 0, 0, 0,
            0, 0, 0, 1 //the last row is always (0,0,0,1)
        ];

        // the last row (row = 3) is already determined.
        // so we loop only on the first three rows and three columns,
        // and the remaining coefficients of the last column are computed afterwards.
        for (int row = 0; row < 3; row++)
        {
            for (int col = 0; col < 3; col++)
            {
                // The last row of m2 is (0, 0, 0, 1),
                // therefore the k = 3 term is always zero for these coefficients.
                for (int k = 0; k < 3; k++)
                {
                    m3[row * 4 + col] += m1[row * 4 + k] * m2[k * 4 + col];
                }
            }
        }

        // the first three coefficients of the last column of m3
        for (int row = 0; row < 3; row++)
        {
            m3[row * 4 + 3] =
                m1[row * 4] * m2[3] +
                m1[row * 4 + 1] * m2[7] +
                m1[row * 4 + 2] * m2[11] +
                m1[row * 4 + 3]; // m2[15] = 1
        }

        return new HomMatrix(m3);
    }

    // old product function without optimization
    /* /// <summary>
     /// Returns the product of the 2 homogeneous matrices.
     /// </summary>
     /// <param name="m1"></param>
     /// <param name="m2"></param>
     /// <returns></returns>
     public static HomMatrix operator *(HomMatrix m1, HomMatrix m2)
     {
         float[] m3 =
         [
             0, 0, 0, 0,
             0, 0, 0, 0,
             0, 0, 0, 0,
             0, 0, 0, 0
         ];

         //usual row-by-column multiplication between matrices.
         for (int row = 0; row < 4; row++)
         {
             for (int col = 0; col < 4; col++)
             {
                 for (int k = 0; k < 4; k++)
                 {
                     m3[row * 4 + col] += m1[row * 4 + k] * m2[k * 4 + col];
                 }
             }
         }

         return new HomMatrix(m3);
     }*/
}

/// <summary>
/// Represents scaling transformations, rotations and translations.
/// It stores a 4×4 homogeneous transformation matrix and its inverse.
/// </summary>
public struct Transformation
{
    /// <summary>
    /// The 4×4 homogeneous transformation matrix representing this transformation.
    /// </summary>
    /// <remarks>
    /// This matrix is stored in row-major order.
    /// </remarks>
    public HomMatrix M { get; private set; }

    /// <summary>
    /// The inverse homogeneous transformation matrix.
    /// </summary>
    /// <remarks>
    /// This value is precomputed and always kept consistent with <see cref="M"/>.
    /// </remarks>
    public HomMatrix InvM { get; private set; }

    //Constructors - Begin

    #region Constructors

    /// <summary>
    /// Initializes an identity transformation.
    /// Both the transformation matrix and its inverse are set to the 4×4 identity matrix.
    /// </summary>
    public Transformation()
    {
        M = new HomMatrix();
        InvM = new HomMatrix();
    }

    /* constructor that receives only one matrix and computes the inverse
     public Transformation(float[] m)
     {
         this.m = new float[16];
         this.invm = new float[16];
         for (int i = 0; i < 16; i++)
         {
             this.m[i] = m[i];
         }

         invm = ComputeInverse(m);
         _CheckConsistency();
     }*/

    public Transformation(HomMatrix m, HomMatrix invM)
    {
        M = m;
        InvM = invM;
        _CheckConsistency();
    }

    public Transformation(float[] m, float[] invM)
    {
        M = new HomMatrix(m);
        InvM = new HomMatrix(invM);
        _CheckConsistency();
    }

    /// <summary>
    /// Constructs a homogeneous transformation representing a pure translation using the given vector.
    /// </summary>
    /// <param name="k">Translation vector.</param>
    public Transformation(Vector k)
    {
        M = new HomMatrix(k);
        InvM = new HomMatrix(-k);
        _CheckConsistency();
    }

    /// <summary>
    /// Initializes a homogeneous transformation representing a diagonal scaling matrix.
    /// </summary>
    /// <param name="scaleX">Scaling factor along the x-axis.</param>
    /// <param name="scaleY">Scaling factor along the y-axis.</param>
    /// <param name="scaleZ">Scaling factor along the z-axis.</param>
    public Transformation(float scaleX, float scaleY, float scaleZ)
    {
        if (scaleX == 0 || scaleY == 0 || scaleZ == 0)
        {
            throw new ArgumentOutOfRangeException(nameof(scaleX) + ", " + nameof(scaleY) + ", " + nameof(scaleZ) +
                                                  " must be different from zero");
        }

        M = new HomMatrix(scaleX, scaleY, scaleZ);
        InvM = new HomMatrix(1.0f / scaleX, 1.0f / scaleY, 1.0f / scaleZ);
        _CheckConsistency();
    }

    /// <summary>
    /// Initializes a homogeneous transformation representing a rotation
    /// around one of the principal coordinate axes (X, Y, or Z).
    /// The rotation follows the right-hand rule and uses radians.
    /// </summary>
    /// <param name="axis">Rotation axis (X, Y, or Z).</param>
    /// <param name="angle">Rotation angle in radians.</param>
    public Transformation(Axis axis, float angle)
    {
        float c = MathF.Cos(angle);
        float s = MathF.Sin(angle);

        switch (axis)
        {
            case Axis.X:

                M = new HomMatrix
                ([
                    1, 0, 0, 0,
                    0, c, -s, 0,
                    0, s, c, 0,
                    0, 0, 0, 1
                ]);
                InvM = new HomMatrix
                ([
                    1, 0, 0, 0,
                    0, c, s, 0,
                    0, -s, c, 0,
                    0, 0, 0, 1
                ]);
                break;
            case Axis.Y:
                M = new HomMatrix
                ([
                    c, 0, s, 0,
                    0, 1, 0, 0,
                    -s, 0, c, 0,
                    0, 0, 0, 1
                ]);
                InvM = new HomMatrix
                ([
                    c, 0, -s, 0,
                    0, 1, 0, 0,
                    s, 0, c, 0,
                    0, 0, 0, 1
                ]);
                break;
            case Axis.Z:
                M = new HomMatrix
                ([
                    c, -s, 0, 0,
                    s, c, 0, 0,
                    0, 0, 1, 0,
                    0, 0, 0, 1
                ]);
                InvM = new HomMatrix
                ([
                    c, s, 0, 0,
                    -s, c, 0, 0,
                    0, 0, 1, 0,
                    0, 0, 0, 1
                ]);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(axis), axis, "Unsupported axis value.");
        }

        _CheckConsistency();
    }

    /*
    /// <summary>
    /// Constructs a generic rotation transformation,
    /// with the axis and angle of rotation passed as arguments.
    /// </summary>
    /// <param name="axis"></param>
    /// <param name="angle"></param>
    /*public Transformation(Vector axis, float angle)
    {
    }*/

    #endregion

    //Constructors - End

    /// <summary>
    /// 1D index for the transformation atrix
    /// (coefficients are stored in row-major order).
    /// Valid range: 0–15.
    /// </summary>
    /// <param name="index"></param>
    public float this[Index index] => M[index];

    /// <summary>
    /// 2D read only index for the transformation matrix M
    /// </summary>
    /// <param name="row"></param>
    /// <param name="col"></param>
    public float this[int row, int col] => M[M._MatrixOffset(row, col)];

    /// <summary>
    /// Verifies that <c>InvM</c> is the inverse of <c>M</c> by checking that
    /// their product is approximately equal to the identity matrix.
    /// </summary>
    /// <exception cref="ArgumentException">
    /// Thrown when <c>M * InvM</c> is not sufficiently close to the identity matrix.
    /// </exception>
    public void _CheckConsistency()
    {
        HomMatrix identity = new HomMatrix();
        if (!HomMatrix.AreMatricesClose(M * InvM, identity, 2e-4f))
        {
            throw new ArgumentException("if multiplied the matrix and its inverse do not return the identity matrix");
        }
    }

    /// <summary>
    /// Determines whether two transformations are approximately equal within
    /// the specified tolerance by comparing both their transformation matrices
    /// and inverse transformation matrices.
    /// </summary>
    /// <param name="t1">The first transformation to compare.</param>
    /// <param name="t2">The second transformation to compare.</param>
    /// <param name="epsilon">The maximum allowed difference between corresponding matrix elements.</param>
    /// <returns>
    /// True if both the transformation matrices and inverse matrices are
    /// approximately equal; otherwise false.
    /// </returns>
    public static bool AreTransformationsClose(Transformation t1, Transformation t2, float epsilon = 1e-5f)
    {
        return HomMatrix.AreMatricesClose(t1.M, t2.M, epsilon)
               && HomMatrix.AreMatricesClose(t1.InvM, t2.InvM, epsilon);
    }

    /// <summary>
    /// Returns the M and InvM matrices coefficients.
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return "Transformation matrix:\n" +
               M.ToString() +
               "\nInverse Matrix:\n" +
               InvM.ToString();
    }

    /// <summary>
    /// Prints the matrix M and its inverse.
    /// </summary>
    public void Print()
    {
        Console.WriteLine(ToString());
    }

    /// <summary>
    /// Returns the inverse of this transformation by swapping the transformation
    /// matrix and its inverse.
    /// </summary>
    /// <returns>
    /// A new transformation whose matrix is <c>InvM</c> and whose inverse matrix
    /// is <c>M</c>.
    /// </returns>
    public Transformation Inverse()
    {
        return new Transformation(InvM, M);
    }

    /// <summary>
    /// Returns the composition of two transformations.
    /// The resulting transformation has matrix <c>t1.M * t2.M</c> and inverse
    /// matrix <c>t2.InvM * t1.InvM</c>, according to
    /// (AB)<sup>-1</sup> = B<sup>-1</sup>A<sup>-1</sup>.
    /// </summary>
    public static Transformation operator *(Transformation t1, Transformation t2)
    {
        HomMatrix prod = t1.M * t2.M;
        HomMatrix invProd = t2.InvM * t1.InvM;
        return new Transformation(prod, invProd);
    }

    /// <summary>
    /// Transforms a vector using the specified transformation matrix.
    /// We use homogeneous coordinates, so the vectors are expected to have their 4th coordinate equal to 0.
    /// </summary>
    /// <param name="t">The transformation to apply.</param>
    /// <param name="v">The vector to transform.</param>
    /// <returns>A new vector resulting from the matrix–vector multiplication.
    /// </returns>
    public static Vector operator *(Transformation t, Vector v)
    {
        Vector v2 = new Vector
        (
            t[0] * v.X + t[1] * v.Y + t[2] * v.Z,
            t[4] * v.X + t[5] * v.Y + t[6] * v.Z,
            t[8] * v.X + t[9] * v.Y + t[10] * v.Z
        );
        return v2;
    }

    /// <summary>
    /// Applies the transformation to a point.
    /// We use homogeneous coordinates, so the points have their 4th coordinate equal to 1.
    /// </summary>
    /// <param name="t">The transformation to apply.</param>
    /// <param name="p">The point to transform.</param>
    /// <returns></returns>
    public static Point operator *(Transformation t, Point p)
    {
        Point p2 = new Point
        (
            t[0] * p.X + t[1] * p.Y + t[2] * p.Z + t[3],
            t[4] * p.X + t[5] * p.Y + t[6] * p.Z + t[7],
            t[8] * p.X + t[9] * p.Y + t[10] * p.Z + t[11]
        );

        float w = t[12] * p.X + t[13] * p.Y + t[14] * p.Z + t[15];
        if (w == 1.0f)
        {
            return p2;
        }

        return p2 * (1.0f / w);
    }

    /// <summary>
    /// Transforms a normal vector using the inverse transpose of the
    /// transformation matrix.
    /// </summary>
    /// <param name="t">The transformation to apply.</param>
    /// <param name="n1">The normal to transform.</param>
    /// <returns>The transformed normal.</returns>
    public static Normal operator *(Transformation t, Normal n1)
    {
        Normal n2 = new Normal
        (
            t.InvM[0] * n1.X + t.InvM[4] * n1.Y + t.InvM[8] * n1.Z,
            t.InvM[1] * n1.X + t.InvM[5] * n1.Y + t.InvM[9] * n1.Z,
            t.InvM[2] * n1.X + t.InvM[6] * n1.Y + t.InvM[10] * n1.Z
        );
        return n2;
    }
}

public enum Axis
{
    X,
    Y,
    Z
}
// This file is release under EUPL_v1.2 license. See LICENSE.md

//forse è meglio cambiare nome ai membri di HomMatrix e Transformation,
//per ora hanno entrambi M

namespace TracerLib;

/// <summary>
/// A Homogeneous Matrix is a 4x4 matrix with the last row = (0,0,0,1)
/// It allows to represent scaling transformations, rotations and translations in 3D space.
/// </summary>
public struct HomMatrix
{
    public float[] M { get; private set; }

    //Constructors - Begin
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
    }

    /// <summary>
    /// Constructs a translation matrix that translates with vector k.
    /// </summary>
    /// <param name="k"></param>
    public HomMatrix(Vector k)
    {
        M =
        [
            1, 0, 0, k.X,
            0, 1, 0, k.Y,
            0, 0, 1, k.Z,
            0, 0, 0, 1
        ];
    }

    /// <summary>
    /// Constructs a scaling matrix that scales along the x,y,z coordinates
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
    //Constructors - End

    /// <summary>
    /// 1D index for the M matrix
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
    /// Checks that the row and col are non-negative and less than 16
    /// </summary>
    /// <param name="row"></param>
    /// <param name="col"></param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
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

    public int _MatrixOffset(int row, int col)
    {
        _CheckCoordinates(row, col);
        return row * 4 + col;
    }

    public static bool AreMatrixClose(HomMatrix a, HomMatrix b, float epsilon = 1e-5f)
    {
        return Functions.AreArrayClose(a.M, b.M, epsilon);
    }

    public override string ToString()
    {
        string str = "(";
        for (int i = 0; i < 4; i++)
        {
            str += "(";
            for (int j = 0; j < 4; j++)
            {
                str += this[i, j].ToString("F2");
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

    //si può forse ottimizzare dato che l'ultima riga è 0,0,0,1?
    /// <summary>
    /// Returns the product of the 2 homogeneous matrices.
    /// </summary>
    /// <param name="m1"></param>
    /// <param name="m2"></param>
    /// <returns></returns>
    public static HomMatrix operator *(HomMatrix m1, HomMatrix m2)
    {
        HomMatrix m3 = new HomMatrix();
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                m3[i * 4 + j] = 0;
                for (int k = 0; k < 4; k++)
                {
                    m3[i * 4 + j] += m1[i * 4 + k] * m2[j + k * 4];
                }
            }
        }

        return m3;
    }
}

/// <summary>
/// A Transformation represents
/// scaling transformations, rotations and translations.
/// It has two properties, the 4x4 homogeneous matrix and its inverse.
/// </summary>
public struct Transformation
{
    public HomMatrix M { get; private set; }

    public HomMatrix InvM { get; private set; }

    //Constructors - Begin
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

    public Transformation(in HomMatrix m, in HomMatrix invM)
    {
        this.M = m;
        this.InvM = invM;
        _CheckConsistency();
    }

    public Transformation(float[] m, float[] invM)
    {
        this.M = new HomMatrix(m);
        this.InvM = new HomMatrix(invM);
        _CheckConsistency();
    }

    /// <summary>
    /// Constructs a translation of vector k
    /// </summary>
    /// <param name="k"></param>
    public Transformation(Vector k)
    {
        M = new HomMatrix(k);
        InvM = new HomMatrix(-k);
        _CheckConsistency();
    }

    /// <summary>
    /// Constructs a scale transformation along the x,y,z coordinates
    /// </summary>
    /// <param name="scaleX"></param>
    /// <param name="scaleY"></param>
    /// <param name="scaleZ"></param>
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
    /// Constructs a rotation around one of the x,y,z axis
    /// </summary>
    /// <param name="axis"></param>
    /// <param name="angle"></param>
    public Transformation(char axis, float angle)
    {
        if (axis != 'x' && axis != 'y' && axis != 'z')
        {
            throw new ArgumentOutOfRangeException(nameof(axis), axis, nameof(axis) + " must be one of x,y,z");
        }

        float c = MathF.Cos(angle);
        float s = MathF.Sin(angle);

        switch (axis)
        {
            case 'x':

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
            case 'y':
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
            case 'z':
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
    //Constructors - End

    /// <summary>
    /// 1D read only index for the matrix M
    /// </summary>
    /// <param name="index"></param>
    public float this[Index index] => M[index];

    /// <summary>
    /// 2D read only index for the matrix M
    /// </summary>
    /// <param name="row"></param>
    /// <param name="col"></param>
    public float this[int row, int col] => M[M._MatrixOffset(row, col)];

    /// <summary>
    /// Returns if the product of the matrix M and its inverse InvM is (close) to the identity.
    /// </summary>
    /// <returns></returns>
    public bool _IsConsistent()
    {
        HomMatrix identity = new HomMatrix();
        return HomMatrix.AreMatrixClose(M * InvM, identity, 1e-4f);
    }

    /// <summary>
    /// Checks if the product of the matrix M and its inverse InvM is (close) to the identity,
    /// if not throws an exception.
    /// </summary>
    /// <exception cref="ArgumentException"></exception>
    public void _CheckConsistency()
    {
        if (!_IsConsistent())
        {
            throw new ArgumentException("if multiplied the matrix and its inverse do not return the identity matrix");
        }
    }

    public static bool AreTransformationsClose(in Transformation t1, in Transformation t2, float epsilon = 1e-5f)
    {
        return HomMatrix.AreMatrixClose(t1.M, t2.M, epsilon)
               && HomMatrix.AreMatrixClose(t1.InvM, t2.InvM, epsilon);
    }

    /// <summary>
    /// Returns (only) the matrix M coefficients
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return M.ToString();
    }

    /// <summary>
    /// Prints (only) the matrix M
    /// </summary>
    public void Print()
    {
        Console.WriteLine(ToString());
    }

    /// <summary>
    /// Returns the inverse transformation, where the matrix corresponds to the inverse matrix and viceversa.
    /// </summary>
    /// <returns></returns>
    public Transformation Inverse()
    {
        return new Transformation(InvM, M);
    }

    /// <summary>
    /// Returns the composition of the two transformations,
    /// the matrix of the transformation is obtained by multiplying the two matrices M,
    /// the inverse is computed as (AB)^-1 = (B^-1)(A^-1).
    /// </summary>
    /// <param name="t1"></param>
    /// <param name="t2"></param>
    /// <returns></returns>
    public static Transformation operator *(in Transformation t1, in Transformation t2)
    {
        HomMatrix prod = t1.M * t2.M;
        HomMatrix invProd = t2.InvM * t1.InvM;
        return new Transformation(prod, invProd);
    }

    /// <summary>
    /// Returns the transformed vector obtained by multiplying the matrix by the vector (matrix–vector multiplication).
    /// We use homogeneous coordinates, so the vectors have their 4th coordinate equal to 0.
    /// </summary>
    /// <param name="t"></param>
    /// <param name="v"></param>
    /// <returns></returns>
    public static Vector operator *(in Transformation t, Vector v)
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
    /// Returns the transformed point obtained by multiplying the matrix by the point (matrix–vector multiplication).
    /// We use homogeneous coordinates, so the points have their 4th coordinate equal to 1.
    /// </summary>
    /// <param name="t"></param>
    /// <param name="p1"></param>
    /// <returns></returns>
    public static Point operator *(in Transformation t, Point p1)
    {
        Point p2 = new Point
        (
            t[0] * p1.X + t[1] * p1.Y + t[2] * p1.Z + t[3],
            t[4] * p1.X + t[5] * p1.Y + t[6] * p1.Z + t[7],
            t[8] * p1.X + t[9] * p1.Y + t[10] * p1.Z + t[11]
        );

        float w = t[12] * p1.X + t[13] * p1.Y + t[14] * p1.Z + t[15];
        if (w == 1.0f)
        {
            return p2;
        }
        else
        {
            return p2 * (1.0f / w);
        }
    }

    /// <summary>
    /// Returns the transformed normal,
    /// obtained multiplying the transpose of the inverse matrix by the normal.
    /// </summary>
    /// <param name="t"></param>
    /// <param name="n1"></param>
    /// <returns></returns>
    public static Normal operator *(in Transformation t, Normal n1)
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
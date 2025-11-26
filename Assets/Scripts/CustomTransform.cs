using UnityEngine;

public class CustomTransform : MonoBehaviour
{
    public Vector2 localPosition = Vector2.zero;
    public float localRotation = 0f; 
    public Vector2 localScale = Vector2.one;

    private Matrix4x4 transformMatrix;

    void Start()
    {
        localPosition = new Vector2(transform.position.x, transform.position.y);
        localRotation = transform.eulerAngles.z;
        localScale = new Vector2(transform.localScale.x, transform.localScale.y);

        UpdateMatrix();
    }

    public void Translate(Vector2 delta)
    {
        localPosition += delta;
        UpdateMatrix();
    }

    public void Rotate(float degrees)
    {
        localRotation += degrees;
        UpdateMatrix();
    }

    public void Scale(Vector2 factor)
    {
        localScale.x *= factor.x;
        localScale.y *= factor.y;
        UpdateMatrix();
    }

    Matrix4x4 CreateTranslationMatrix(Vector2 t)
    {
        Matrix4x4 matrix = Matrix4x4.identity;
        matrix.m03 = t.x;
        matrix.m13 = t.y;
        return matrix;
    }

    Matrix4x4 CreateRotationMatrix(float degrees)
    {
        float radians = degrees * Mathf.Deg2Rad;
        float cos = Mathf.Cos(radians);
        float sin = Mathf.Sin(radians);

        Matrix4x4 matrix = Matrix4x4.identity;
        matrix.m00 = cos;
        matrix.m01 = -sin;
        matrix.m10 = sin;
        matrix.m11 = cos;
        return matrix;
    }

    Matrix4x4 CreateScaleMatrix(Vector2 s)
    {
        Matrix4x4 matrix = Matrix4x4.identity;
        matrix.m00 = s.x;
        matrix.m11 = s.y;
        return matrix;
    }

    Matrix4x4 MultiplyMatrices(Matrix4x4 a, Matrix4x4 b)
    {
        Matrix4x4 result = new Matrix4x4();
        for (int row = 0; row < 4; row++)
        {
            for (int col = 0; col < 4; col++)
            {
                result[row, col] =
                    a[row, 0] * b[0, col] +
                    a[row, 1] * b[1, col] +
                    a[row, 2] * b[2, col] +
                    a[row, 3] * b[3, col];
            }
        }
        return result;
    }

    public void UpdateMatrix()
    {
        Matrix4x4 T = CreateTranslationMatrix(localPosition);
        Matrix4x4 R = CreateRotationMatrix(localRotation);
        Matrix4x4 S = CreateScaleMatrix(localScale);

        Matrix4x4 RS = MultiplyMatrices(R, S);
        transformMatrix = MultiplyMatrices(T, RS);

        transform.position = new Vector3(localPosition.x, localPosition.y, 0);
        transform.eulerAngles = new Vector3(0, 0, localRotation);
        transform.localScale = new Vector3(localScale.x, localScale.y, 1);
    }

    public Matrix4x4 GetMatrix()
    {
        return transformMatrix;
    }
}
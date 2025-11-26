using UnityEngine;

public class CustomTransform2D : MonoBehaviour
{
    public Vector2 localPosition;
    public float localRotation;
    public Vector2 localScale = Vector2.one;

    private Matrix4x4 transformMatrix;

    void Awake()
    {
        localPosition = transform.position;
        localRotation = 0f;
        localScale = transform.localScale;
        UpdateTransformMatrix();
    }

    public void SetPosition(Vector2 pos)
    {
        localPosition = pos;
        UpdateTransformMatrix();
    }

    public void Translate(Vector2 delta)
    {
        localPosition += delta;
        UpdateTransformMatrix();
    }

    public void SetRotation(float degrees)
    {
        localRotation = degrees;
        UpdateTransformMatrix();
    }

    public void Rotate(float degrees)
    {
        localRotation += degrees;
        UpdateTransformMatrix();
    }

    public void SetScale(Vector2 scale)
    {
        localScale = scale;
        UpdateTransformMatrix();
    }

    public void Scale(float multiplier)
    {
        localScale *= multiplier;
        UpdateTransformMatrix();
    }

    void UpdateTransformMatrix()
    {
        Matrix4x4 T = CreateTranslationMatrix(localPosition);
        Matrix4x4 R = CreateRotationMatrix(localRotation);
        Matrix4x4 S = CreateScaleMatrix(localScale);

        transformMatrix = MultiplyMatrices(T, MultiplyMatrices(R, S));

        // Sync Unity transform
        transform.position = localPosition;
        transform.rotation = Quaternion.Euler(0, 0, localRotation);
        transform.localScale = localScale;

        // Send to shader
        CustomRenderer cr = GetComponent<CustomRenderer>();
        if (cr != null)
            cr.SetTransformMatrix(transformMatrix);
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
            for (int col = 0; col < 4; col++)
                result[row, col] =
                    a[row, 0] * b[0, col] +
                    a[row, 1] * b[1, col] +
                    a[row, 2] * b[2, col] +
                    a[row, 3] * b[3, col];
        return result;
    }
}
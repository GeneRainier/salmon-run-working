using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public class UtilityFunctions
{
    public static int WrappedClamp (int n, int min, int max)
    {
        if (n > max) 
            return 0;
        if (n < min) 
            return max;
        return n;
    }
    
    public static float LerpNegative (float a, float b, float t)
    {
        return a + (b - a) * t;
    }

    public static Vector3 Vector3LerpNegative (Vector3 a, Vector3 b, float t)
    {
        Vector3 c = Vector3.zero;
        c.x = LerpNegative(a.x, b.x, t);
        c.y = LerpNegative(a.y, b.y, t);
        c.z = LerpNegative(a.z, b.z, t);
        return c;
    }

    public static float AngleDifference (float angle1, float angle2)
    {
        float diff = ( angle2 - angle1 + 180 ) % 360 - 180;
        return diff < -180 ? diff + 360 : diff;
    }
    
    #region Sha256 Hash
    // https://stackoverflow.com/questions/16999361/obtain-sha-256-string-of-a-string/17001289
    public static string Sha256 (string value)
    {
        StringBuilder sb = new StringBuilder();

        using (var hash = SHA256.Create())            
        {
            Encoding enc = Encoding.UTF8;
            Byte[] result = hash.ComputeHash(enc.GetBytes(value));

            foreach (Byte b in result)
                sb.Append(b.ToString("x2"));
        }

        return sb.ToString();
    }
    #endregion
    
    #region GUID Generation
    // https://stackoverflow.com/questions/4825907/convert-int-to-guid
    public static Guid Int2Guid (int value)
    {
        byte[] bytes = new byte[16];
        BitConverter.GetBytes(value).CopyTo(bytes, 0);
        return new Guid(bytes);
    }

    public static int Guid2Int (Guid value)
    {
        byte[] b = value.ToByteArray();
        int bint = BitConverter.ToInt32(b, 0);
        return bint;
    }
    #endregion
}
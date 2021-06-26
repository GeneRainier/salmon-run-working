using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

/*
 * A series of utility functions to make certain calculation processes easier throughout the project
 * 
 * Authors: Benjamin Person (Editor 2020)
 */
public class UtilityFunctions
{
    /*
     * Clamps a value, but instead of returning a min or max it returns up to a max or 0 otherwise
     * 
     * @param n The value we wish to clamp
     * @param min The minimum value to compare against
     * @param max The maximum value that n may equal
     */
    public static int WrappedClamp (int n, int min, int max)
    {
        if (n > max) 
            return 0;
        if (n < min) 
            return max;
        return n;
    }
    
    /*
     * Linearly interpolates between two values
     * 
     * @param a The start value
     * @param b The end value
     * @param t The percentage value between those values
     */
    public static float LerpNegative (float a, float b, float t)
    {
        return a + (b - a) * t;
    }

    /*
     * Linearly interpolates between two Vector3s by interpolating each component
     * 
     * @param a The start position
     * @param b The end position
     * @param t The percentage between those two positions
     */
    public static Vector3 Vector3LerpNegative (Vector3 a, Vector3 b, float t)
    {
        Vector3 c = Vector3.zero;
        c.x = LerpNegative(a.x, b.x, t);
        c.y = LerpNegative(a.y, b.y, t);
        c.z = LerpNegative(a.z, b.z, t);
        return c;
    }

    /*
     * The delta between two given angles
     * 
     * @param angle1 The first angle
     * @param angle2 The second angle
     */
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
﻿using System;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
namespace blockChainWebApp.API.BLL.HelpClasses
{
    public class Base58Encoding
    {
        public const int CheckSumSizeInBytes = 4;

        public static byte[] AddCheckSum(byte[] data)
        {
            var checkSum = GetCheckSum(data);
            var dataWithCheckSum = ArrayHelpers.ConcatArrays(data, checkSum);
            return dataWithCheckSum;
        }

        public static byte[] VerifyAndRemoveCheckSum(byte[] data)
        {
            var result = ArrayHelpers.SubArray(data, 0, data.Length - CheckSumSizeInBytes);
            var givenCheckSum = ArrayHelpers.SubArray(data, data.Length - CheckSumSizeInBytes);
            var correctCheckSum = GetCheckSum(result);
            return givenCheckSum.SequenceEqual(correctCheckSum) ? result : null;
        }

        private const string Digits = "123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz";

        public static string Encode(byte[] data)
        {
            // Decode byte[] to BigInteger
            var intData = data.Aggregate<byte, BigInteger>(0, (current, t) => current * 256 + t);

            // Encode BigInteger to Base58 string
            var result = "";
            while (intData > 0)
            {
                var remainder = (int)(intData % 58);
                intData /= 58;
                result = Digits[remainder] + result;
            }

            // Append `1` for each leading 0 byte
            for (int i = 0; i < data.Length && data[i] == 0; i++)
            {
                result = '1' + result;
            }
            return result;
        }

        public static string EncodeWithCheckSum(byte[] data)
        {
            return Encode(AddCheckSum(data));
        }

        public static byte[] Decode(string s)
        {
            // Decode Base58 string to BigInteger 
            BigInteger intData = 0;
            for (var i = 0; i < s.Length; i++)
            {
                var digit = Digits.IndexOf(s[i]); //Slow
                if (digit < 0)
                    throw new FormatException($"Invalid Base58 character `{s[i]}` at position {i}");
                intData = intData * 58 + digit;
            }

            // Encode BigInteger to byte[]
            // Leading zero bytes get encoded as leading `1` characters
            var leadingZeroCount = s.TakeWhile(c => c == '1').Count();
            var leadingZeros = Enumerable.Repeat((byte)0, leadingZeroCount);
            var bytesWithoutLeadingZeros =
                intData.ToByteArray()
                .Reverse()// to big endian
                .SkipWhile(b => b == 0);//strip sign byte
            var result = leadingZeros.Concat(bytesWithoutLeadingZeros).ToArray();
            return result;
        }

        // Throws `FormatException` if s is not a valid Base58 string, or the checksum is invalid
        public static byte[] DecodeWithCheckSum(string s)
        {
            var dataWithCheckSum = Decode(s);
            var dataWithoutCheckSum = VerifyAndRemoveCheckSum(dataWithCheckSum);
            if (dataWithoutCheckSum == null)
                throw new FormatException("Base58 checksum is invalid");
            return dataWithoutCheckSum;
        }

        private static byte[] GetCheckSum(byte[] data)
        {
            SHA256 sha256 = new SHA256Managed();
            var hash1 = sha256.ComputeHash(data);
            var hash2 = sha256.ComputeHash(hash1);

            var result = new byte[CheckSumSizeInBytes];
            Buffer.BlockCopy(hash2, 0, result, 0, result.Length);

            return result;
        }
    }

    public class ArrayHelpers
    {
        public static T[] ConcatArrays<T>(params T[][] arrays)
        {
            var result = new T[arrays.Sum(arr => arr.Length)];
            var offset = 0;
            foreach (var arr in arrays)
            {
                Buffer.BlockCopy(arr, 0, result, offset, arr.Length);
                offset += arr.Length;
            }
            return result;
        }

        public static T[] ConcatArrays<T>(T[] arr1, T[] arr2)
        {
            var result = new T[arr1.Length + arr2.Length];
            Buffer.BlockCopy(arr1, 0, result, 0, arr1.Length);
            Buffer.BlockCopy(arr2, 0, result, arr1.Length, arr2.Length);
            return result;
        }

        public static T[] SubArray<T>(T[] arr, int start, int length)
        {
            var result = new T[length];
            Buffer.BlockCopy(arr, start, result, 0, length);
            return result;
        }

        public static T[] SubArray<T>(T[] arr, int start)
        {
            return SubArray(arr, start, arr.Length - start);
        }
    }
}
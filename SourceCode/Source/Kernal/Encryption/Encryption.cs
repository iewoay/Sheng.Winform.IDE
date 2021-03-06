﻿//http://support.microsoft.com/kb/307010/zh-cn

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace Sheng.SailingEase.Kernal
{
    public class Encryption
    {
        /*
         * // For additional security pin the key.
            GCHandle gch = GCHandle.Alloc(key, GCHandleType.Pinned);
         * 
         *  // Remove the key from memory. 
            CSEncryptDecrypt.ZeroMemory(gch.AddrOfPinnedObject(), sSecretKey.Length * 2);
            gch.Free();
         */

        /// <summary>
        /// Call this function to remove the key from memory after use for security
        /// </summary>
        /// <param name="Destination"></param>
        /// <param name="Length"></param>
        /// <returns></returns>
        [System.Runtime.InteropServices.DllImport("KERNEL32.DLL", EntryPoint = "RtlZeroMemory")]
        public static extern bool ZeroMemory(IntPtr Destination, int Length);

        /// <summary>
        /// 生成密钥字符串
        /// Function to Generate a 64 bits Key.
        /// </summary>
        /// <returns></returns>
        public static string GenerateKey()
        {
            // Create an instance of Symetric Algorithm. Key and IV is generated automatically.
            DESCryptoServiceProvider desCrypto = (DESCryptoServiceProvider)DESCryptoServiceProvider.Create();

            // Use the Automatically generated key for Encryption. 
            return Convert.ToBase64String(desCrypto.Key);
        }

        /// <summary>
        /// 加密文件
        /// </summary>
        /// <param name="inputFile"></param>
        /// <param name="outputFile"></param>
        /// <param name="key"></param>
        public static void EncryptFile(string inFile, string outFile, string key)
        {
            if (File.Exists(inFile) == false)
                throw new FileNotFoundException(inFile);

            //Create the file streams to handle the input and output files.
            FileStream inStream = new FileStream(inFile, FileMode.Open, FileAccess.Read);
            FileStream outStream = new FileStream(outFile, FileMode.OpenOrCreate, FileAccess.Write);

            EncryptStream(inStream, outStream, key);

            //outStream.SetLength(0);
            ////Create variables to help with read and write.
            //byte[] buffer = new byte[2048]; //This is intermediate storage for the encryption.
            //long currentOffset = 0;              //This is the total number of bytes written.
            //int readLength;                     //This is the number of bytes to be written at a time.
            //DES des = new DESCryptoServiceProvider();
            //des.Key = Convert.FromBase64String(key);
            //des.IV = Convert.FromBase64String(key);
            //CryptoStream cryptoStream = new CryptoStream(outStream, des.CreateEncryptor(), CryptoStreamMode.Write);
            ////Read from the input file, then encrypt and write to the output file.
            //while (currentOffset < inStream.Length)
            //{
            //    readLength = inStream.Read(buffer, 0, 100);
            //    cryptoStream.Write(buffer, 0, readLength);
            //    currentOffset = currentOffset + readLength;
            //}
            //cryptoStream.Close();
            outStream.Close();
            outStream.Dispose();
            inStream.Close();
            inStream.Dispose();
        }

        /// <summary>
        /// 解密文件
        /// </summary>
        /// <param name="inputFile"></param>
        /// <param name="outputFile"></param>
        /// <param name="key"></param>
        public static void DecryptFile(string inFile, string outFile, string key)
        {
            if (File.Exists(inFile) == false)
                throw new FileNotFoundException(inFile);

            //Create the file streams to handle the input and output files.
            FileStream inStream = new FileStream(inFile, FileMode.Open, FileAccess.Read);
            FileStream outStream = new FileStream(outFile, FileMode.OpenOrCreate, FileAccess.Write);

            DecryptStream(inStream, outStream, key);

            //outStream.SetLength(0);
            ////Create variables to help with read and write.
            //byte[] buffer = new byte[2048]; //This is intermediate storage for the encryption.
            //long currentOffset = 0;              //This is the total number of bytes written.
            //int readLength;                     //This is the number of bytes to be written at a time.
            //DES des = new DESCryptoServiceProvider();
            //des.Key = Convert.FromBase64String(key);
            //des.IV = Convert.FromBase64String(key);
            //ICryptoTransform desdecrypt = des.CreateDecryptor();
            //CryptoStream cryptoStream = new CryptoStream(outStream, des.CreateDecryptor(), CryptoStreamMode.Write);
            ////Read from the input file, then encrypt and write to the output file.
            //while (currentOffset < inStream.Length)
            //{
            //    readLength = inStream.Read(buffer, 0, 100);
            //    cryptoStream.Write(buffer, 0, readLength);
            //    currentOffset = currentOffset + readLength;
            //}
            //cryptoStream.Close();
            outStream.Close();
            outStream.Dispose();
            inStream.Close();
            inStream.Dispose();
        }

        public static void EncryptDirectory(string inPath, string outPath, string key)
        {
            if (Directory.Exists(inPath) == false)
                throw new DirectoryNotFoundException(inPath);

            if (Directory.Exists(outPath) == false)
            {
                Directory.CreateDirectory(outPath);
            }

            DirectoryInfo inDir = new DirectoryInfo(inPath);

            foreach (FileInfo item in inDir.GetFiles())
            {
                EncryptFile(Path.Combine(inPath, item.Name), Path.Combine(outPath, item.Name), key);
            }

            foreach (DirectoryInfo item in inDir.GetDirectories())
            {
                EncryptDirectory(item.FullName, Path.Combine(outPath, item.Name), key);
            }
        }

        public static void DecryptDirectory(string inPath, string outPath, string key)
        {
            if (Directory.Exists(inPath) == false)
                throw new DirectoryNotFoundException(inPath);

            if (Directory.Exists(outPath) == false)
            {
                Directory.CreateDirectory(outPath);
            }

            DirectoryInfo inDir = new DirectoryInfo(inPath);

            foreach (FileInfo item in inDir.GetFiles())
            {
                DecryptFile(Path.Combine(inPath, item.Name), Path.Combine(outPath, item.Name), key);
            }

            foreach (DirectoryInfo item in inDir.GetDirectories())
            {
                DecryptDirectory(item.FullName, Path.Combine(outPath, item.Name), key);
            }
        }

        /// <summary>
        /// 加密数据流
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static void EncryptStream(Stream inStream, Stream outStream, string key)
        {
            byte[] buffer = new byte[2048];
            long currentOffset = 0;
            int readLength;

            DES des = new DESCryptoServiceProvider();
            des.Key = Convert.FromBase64String(key);
            des.IV = Convert.FromBase64String(key);

            outStream.SetLength(0);
            inStream.Seek(0, SeekOrigin.Begin);

            CryptoStream cryptoStream = new CryptoStream(outStream, des.CreateEncryptor(), CryptoStreamMode.Write);

            while (currentOffset < inStream.Length)
            {
                readLength = inStream.Read(buffer, 0, buffer.Length);
                cryptoStream.Write(buffer, 0, readLength);
                currentOffset = currentOffset + readLength;
            }

            cryptoStream.FlushFinalBlock();
            outStream.Seek(0, SeekOrigin.Begin);

            //这里的 cryptoStream 不能写 close 和 dispose
            //因为会连带的把 outStream 也给关闭

            return;
        }

        /// <summary>
        /// 解密数据流
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static void DecryptStream(Stream inStream, Stream outStream, string key)
        {
            byte[] buffer = new byte[2048];
            long currentOffset = 0;
            int readLength;

            DES des = new DESCryptoServiceProvider();
            des.Key = Convert.FromBase64String(key);
            des.IV = Convert.FromBase64String(key);

            outStream.SetLength(0);
            inStream.Seek(0, SeekOrigin.Begin);

           // ICryptoTransform desdecrypt = des.CreateDecryptor();
            CryptoStream cryptoStream = new CryptoStream(outStream, des.CreateDecryptor(), CryptoStreamMode.Write);

            while (currentOffset < inStream.Length)
            {
                readLength = inStream.Read(buffer, 0, buffer.Length);
                cryptoStream.Write(buffer, 0, readLength);
                currentOffset = currentOffset + readLength;
            }

            cryptoStream.FlushFinalBlock();
            outStream.Seek(0, SeekOrigin.Begin);

            return;
        }

        /// <summary>
        /// 从加密数据流中读取数据
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="key"></param>
        /// <param name="buffer"></param>
        /// <param name="offest"></param>
        /// <returns></returns>
        public static int ReadCryptStream(Stream stream, string key, byte[] buffer, int offest)
        {
            int readLength;

            MemoryStream outStream = new MemoryStream();

            DES des = new DESCryptoServiceProvider();
            des.Key = Convert.FromBase64String(key);
            des.IV = Convert.FromBase64String(key);

            ICryptoTransform desdecrypt = des.CreateDecryptor();
            CryptoStream cryptoStream = new CryptoStream(outStream, des.CreateDecryptor(), CryptoStreamMode.Write);

            readLength = stream.Read(buffer, offest, buffer.Length);
            cryptoStream.Write(buffer, 0, readLength);

            //不能这样写，这样写buffer就是一个新的字节数组了，而不是原buffer的引用了
            //buffer = outStream.ToArray();

            //TODO:要考虑超长的问题
            outStream.Read(buffer, 0, (int)outStream.Length);

            cryptoStream.Close();

            outStream.Close();
            outStream.Dispose();


            return readLength;
        }
    }
}

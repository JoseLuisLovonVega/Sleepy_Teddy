using System;
using System.Security.Cryptography;
using WindesHeartSDK.Helpers;

namespace WindesHeartSDK.Devices.MiBand4Device.Helpers
{
    public static class MiBand4ConversionHelper
    {

        public static byte[] CreateKey(byte[] value)
        {
            byte[] bytes = { 0x03, 0x00 };
            byte[] secretKey = { 0xa8, 0x0c, 0x62, 0x4b, 0xe3, 0xfa, 0x55, 0x6b, 0xc8, 0xb1, 0xab, 0xb0, 0xe1, 0x4b, 0x68, 0x8d }; // Your key here, See docs for more info

            value = ConversionHelper.CopyOfRange(value, 3, 19);
            byte[] buffer = EncryptBuff(secretKey, value);
            byte[] endBytes = new byte[18]; 
            Buffer.BlockCopy(bytes, 0, endBytes, 0, 2);
            Buffer.BlockCopy(buffer, 0, endBytes, 2, 16);
            return endBytes;
        }

        public static byte[] EncryptBuff(byte[] sessionKey, byte[] buffer)
        {
            AesManaged myAes = new AesManaged();

            myAes.Mode = CipherMode.ECB;
            myAes.Key = sessionKey;
            myAes.Padding = PaddingMode.None;

            ICryptoTransform encryptor = myAes.CreateEncryptor();
            return encryptor.TransformFinalBlock(buffer, 0, buffer.Length);
        }


    }
}

using Neo.Cryptography;
using System;
using System.Collections.Concurrent;
using System.Text;

namespace Neo.SmartContract
{
    public static class Helper
    {
        private static readonly ConcurrentDictionary<string, uint> MethodHashes
            = new ConcurrentDictionary<string, uint>();


        public static uint ToInteropMethodHash(this string method)
        {
            return MethodHashes.GetOrAdd(method, p => BitConverter.ToUInt32(Encoding.ASCII.GetBytes(p).Sha256(), 0));
        }

        public static UInt160 ToScriptHash(this byte[] script)
        {
            return new UInt160(Crypto.Default.Hash160(script));
        }


    }
}

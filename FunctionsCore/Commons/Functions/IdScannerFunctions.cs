using FunctionsCore.Commons.Entities;
using FunctionsCore.Contexts;
using FunctionsCore.Services;
using FunctionsCore.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using FunctionsCore.Enums;
using System;

namespace FunctionsCore.Commons.Functions
{
    public class IdScannerFunctions
    {
        private static readonly object LockObject = new object();

        public IdScannerModel ScanLicenceFront(int id)
        {
            return new IdScannerModel()
            {
                Nev = "Kovács Gábor",
                ErvenyessegVege = DateTime.Now.AddYears(2),
                OkmanyTipus = "IdCard",
                Kep = new byte[] {  0xFF, 0xD8, 0xFF, 0xE0, 0x00, 0x10, 0x4A, 0x46,
                                    0x49, 0x46, 0x00, 0x01, 0x01, 0x01, 0x01, 0x2C }
            };
        }
    }
}
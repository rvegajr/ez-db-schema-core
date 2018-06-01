using EzDbSchema.Core.Interfaces;
using EzDbSchema.Core.Objects;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Json;
using System.Linq;

namespace EzDbSchema.Core.Extentions.Json
{
    public static class JsonExtensions
    {
        private const string DOUBLE_QUOTE_SUB = @"_$$_";
        private const string DOUBLE_QUOTE = @"""""";

        public static string AsString(this JsonValue obj)
        {
            return obj.ToString().Replace(DOUBLE_QUOTE, DOUBLE_QUOTE_SUB).Replace("\"", "").Replace(DOUBLE_QUOTE_SUB, "\"");
        }
    }
}
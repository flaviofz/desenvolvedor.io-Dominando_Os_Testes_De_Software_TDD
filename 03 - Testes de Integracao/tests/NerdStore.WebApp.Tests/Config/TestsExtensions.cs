using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NerdStore.WebApp.Tests.Config
{
    public static class TestsExtensions
    {
        public static decimal ApenasNumeros(this string value)
            => Convert.ToDecimal(new string(value.Where(char.IsDigit).ToArray()));
    }
}

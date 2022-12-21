using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuasarCoreTimesheetsApp.Auth
{
    public class BoolToIntConverter : ValueConverter<bool, int>
    {
        public BoolToIntConverter() : base(
            boolVal => Convert.ToInt32(boolVal),
            intVal => Convert.ToBoolean(intVal)
        )
        {

        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Core.Business
{
    public static class Guard
    {
        [DebuggerStepThrough]
        public static T ThrowIfNull<T>(T argumentValue, string argumentName)
        {
            if (argumentValue is null) 
                throw new ArgumentNullException(argumentName);
            return argumentValue;
        }
    }
}

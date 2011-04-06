using System;
using System.Collections.Generic;
using System.Text;

namespace NBToolkit.Map.Utility
{
    public interface ICopyable <T>
    {
        T Copy ();
    }
}

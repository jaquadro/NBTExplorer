using System;
using System.Collections.Generic;
using System.Text;

namespace Substrate.Map.Utility
{
    public interface ICopyable <T>
    {
        T Copy ();
    }
}

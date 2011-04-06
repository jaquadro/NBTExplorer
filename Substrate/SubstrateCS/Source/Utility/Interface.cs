using System;
using System.Collections.Generic;
using System.Text;

namespace Substrate.Utility
{
    public interface ICopyable <T>
    {
        T Copy ();
    }
}

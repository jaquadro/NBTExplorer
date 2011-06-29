using System;
using System.Collections.Generic;
using System.Text;

namespace Substrate.Core
{
    public interface ICopyable <T>
    {
        T Copy ();
    }
}

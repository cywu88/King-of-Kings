﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FootStone
{
    public struct TypeComparer : IComparer<Type>
    {
        public int Compare(Type x, Type y)
        {
            return x.FullName.CompareTo(y.FullName);
        }
    }
}

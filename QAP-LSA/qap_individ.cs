﻿using System;
using System.Collections.Generic;
using System.Text;
using QAP_LSA;

namespace QAP
{
    public class individ
    {
        
        public void print()                                       //console out permutation
        {
            for (int i = 0; i < p.Count; i++)
                Console.Write(p[i] + " ");
            Console.WriteLine('\n');
        }
    }
}

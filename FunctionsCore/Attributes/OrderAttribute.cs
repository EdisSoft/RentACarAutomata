﻿using System;

namespace FunctionsCore.Attributes
{
    public class OrderAttribute : Attribute
    {
        public int Order { get; set; }

        public OrderAttribute(int order)
        {
            Order = order;
        }
    }
}

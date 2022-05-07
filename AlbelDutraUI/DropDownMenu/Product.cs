using System;
using System.Collections.Generic;
using System.Text;

namespace DropDownMenu
{
    public class Product
    {
        public string Name { get; private set; }
        public double Value { get; private set; }
        public string Image { get; private set; }
        public Product(string name, double value, string image)
        {
            Name = name;
            Value = value;
            Image = image;
        }
    }
}

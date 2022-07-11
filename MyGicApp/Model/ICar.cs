using System;
using System.Collections.Generic;
using System.Text;

namespace MyGicApp
{
    public interface ICar
    {
        string Color { get; set; }
        string MinSpeed { get; set; }
        string MaxSpeed { get; set; }

        void Accelerate(int speed=0);
    }
}

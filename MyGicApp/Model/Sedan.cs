using System;
using System.Collections.Generic;
using System.Text;

namespace MyGicApp
{
    public class Sedan : ICar
    {
        private double Speed { get; set; }
        public string Color { get ; set ; }
        public string MinSpeed { get ; set ; }
        public string MaxSpeed { get; set; }

        public void Accelerate(int speed = 0)
        {
            Console.WriteLine(Speed += speed);    
        }
    }
}

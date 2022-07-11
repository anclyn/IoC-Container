using System;
using System.Collections.Generic;
using System.Text;

namespace MyGicApp
{
    public class SUV : ICar
    {
        private readonly ITestCar _iTestCar;
        public SUV(ITestCar iTestCar)
        {
            _iTestCar = iTestCar;
        }
        public string Color { get; set; }
        public string MinSpeed { get; set; }
        public string MaxSpeed { get; set; }

        public void Accelerate(int speed = 0)
        {
            throw new NotImplementedException();
        }
    }
}

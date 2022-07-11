using System;
using System.Collections.Generic;
using System.Text;

namespace MyGicApp
{
    public class TestCar : ITestCar
    {
        readonly ICar _car;
        public TestCar(ICar car)
        {
            _car = car;
        }

        public void TestCarSpeed()
        {
            Console.WriteLine("Accelerating.....");

            for(int x=0; x <= 50; x++)
            {
                _car.Accelerate(x);
            }
        }

        public string Color()
        {
            return _car.Color;
        }
    }
}

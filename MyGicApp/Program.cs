using MyGic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MyGicApp
{
    class Program
    {
        static Container container = new Container();

        static void Main(string[] args)
        {
            try
            {

                container.Register<ICar, Sedan>(LifeCycleType.Transient);
                container.Register<ITestCar, TestCar>(LifeCycleType.Singleton);               


                FirstMethod();
                SecondMethod();


                ThirdMethod();

            }
            catch(Exception ex)
            {
                Console.WriteLine($"Wrong has happened: {ex.Message}");
            }

            Console.ReadKey();
        }

        static void FirstMethod()
        {
           

            var car = container.Resolve<ITestCar>();           
            car.TestCarSpeed();
        }

        static void SecondMethod()
        {
            var car2 = container.Resolve<ITestCar>();
            car2.TestCarSpeed();
        }

        static void ThirdMethod()
        {
            var car3 = container.Resolve<IAeroPlane>();
            car3.Fly();
        }
    }
}

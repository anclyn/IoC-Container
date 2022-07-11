using FluentAssertions;
using MyGic;
using MyGicApp;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace MyGicContainerTests
{
    public class ContainerShould
    {
        private readonly IContainer container;
        public ContainerShould()
        {
            container = new Container();
        }

        [Fact]
        public void Register_AConcateClassWithCorrectInheretedInterface_Should_Success()
        {
            //Act
            container.Register<ICar, Sedan>();

            //Assert
            Received.InOrder(() =>
            {
                container.Register<ITestCar, TestCar>();
            });
        }

        [Fact]
        public void Register_RegisteringAConcreteClassNotValidInterface_Should_Fail()
        {
            //Act
            void func() => container.Register<ICar, TestCar>();

            Assert.ThrowsAny<InvalidCastException>(func);

        }

        [Fact]
        public void Register_RegisterDuplicateType_Should_Fail()
        {
            container.Register<ICar, Sedan>();
            
            void func() => container.Register<ICar, Sedan>();

            Assert.ThrowsAny<Exception>(func);

        }

        [Fact]
        public void Resolve_AfterRegisteringValidTypeMustResolve_Should_Success()
        {
            //Arrange
            var sedan = new Sedan();
            
            //Act
            container.Register<ICar, Sedan>();
            var result = container.Resolve<ICar>();

            result.Should().BeEquivalentTo(sedan);
        }

        [Fact]
        public void Resolve_ResolvingUnregisteredType_Should_Fail()
        {
            container.Register<ICar, Sedan>();

            object func() => container.Resolve<ITestCar>();

            Assert.ThrowsAny<Exception>(func);
        }

        [Fact]
        public void Resolve_HavingRecyclicDependency_Should_Fail()
        {
            container.Register<ITestCar, TestCar>();
            container.Register<ICar, SUV>();

            object func() => container.Resolve<ITestCar>();

            Assert.ThrowsAny<Exception>(func);
        }

        [Fact]
        public void Resolve_ResolvingSingleTon_MustUseSameInstance_Should_Success()
        {
            container.Register<ICar, Sedan>(LifeCycleType.Singleton);
            container.Register<ITestCar, TestCar>();
          

            var car1 = container.Resolve<ITestCar>();
            var car2 = container.Resolve<ITestCar>();

            var singleTonCar = container.Resolve<ICar>();
            singleTonCar.Color = "Red";

            string car1Color = car1.Color();
            string car2Color = car2.Color();
            Assert.Equal(singleTonCar.Color, car1Color);
            Assert.Equal(singleTonCar.Color, car2Color);
        }
        
        [Fact]
        public void Resolve_ResolvingTransient_MustUseDifferentClassEverytime_Should_Success()
        {
            container.Register<ICar, Sedan>();
            container.Register<ITestCar, TestCar>();


            var car1 = container.Resolve<ITestCar>();
            var car2 = container.Resolve<ITestCar>();

            var singleTonCar = container.Resolve<ICar>();
            singleTonCar.Color = "Red";

            string car1Color = car1.Color();
            string car2Color = car2.Color();
            Assert.Null(car1Color);
            Assert.Null(car2Color);
        }


    }
}

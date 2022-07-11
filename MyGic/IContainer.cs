using System;
using System.Collections.Generic;
using System.Text;

namespace MyGic
{
    public interface IContainer
    {
        void Register<I, C>(LifeCycleType lifeCycleType = LifeCycleType.Transient) where I : class where C : class;
        T Resolve<T>();
       
    }
}

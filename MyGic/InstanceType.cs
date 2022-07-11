using System;
using System.Collections.Generic;
using System.Text;

namespace MyGic
{
    public class InstanceType
    {        
        public LifeCycleType LifeCyle { get; set; }

        public Type InterfaceType { get; set; }

        public Type ImplementingType { get; set; }

        public Object Instance { get; set; }
    }
}

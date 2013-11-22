using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class ObjectsThroughAppDomainBorder
    {
        public class NonSerializableObject
        {
            public NonSerializableObject ()
	        {
                I = 1;
	        }

            public int I { get; set; }
        }

        [Serializable]
        public class SerializableObject
        {
            public SerializableObject()
            {
                I = 1;
            }

            public int I { get; set; }
        }

        public class MarshalByRef : MarshalByRefObject
        {
            public MarshalByRef()
            {
                I = 1;
            }

            public int I { get; set; }
        }

        public void NonSerializable()
        {
            var o = new NonSerializableObject();
        }

        public void Serializable()
        {
            var o = new SerializableObject();
        }

        public void PassingMarshalByRef()
        {
            var o = new MarshalByRef();
        }
    }
}

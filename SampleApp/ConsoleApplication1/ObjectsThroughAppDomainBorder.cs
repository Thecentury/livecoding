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

            public NonSerializableObject Null { get; set; }

            public NonSerializableObject Me
            {
                get { return this; }
            }
        }

        [Serializable]
        public class SerializableObject
        {
            public SerializableObject()
            {
                I = 2;
            }

            public int I { get; set; }

            public SerializableObject Null { get; set; }

            public SerializableObject Me
            {
                get { return this; }
            }
        }

        public class MarshalByRef : MarshalByRefObject
        {
            public MarshalByRef()
            {
                I = 3;
            }

            public int I { get; set; }

            public MarshalByRef Null { get; set; }

            public MarshalByRef Me
            {
                get { return this; }
            }
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

using System;
using System.Runtime.Serialization;

namespace Identity.Dapper.Exceptions
{
    [Serializable]
    public class NoDapperIdentityNorConnectionStringsSectionException
        : Exception
    {
        public NoDapperIdentityNorConnectionStringsSectionException()
        {
        }

        public NoDapperIdentityNorConnectionStringsSectionException(string message)
            : base(message)
        {
        }

        public NoDapperIdentityNorConnectionStringsSectionException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected NoDapperIdentityNorConnectionStringsSectionException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }
    }
}

using System;
using System.Runtime.Serialization;

namespace Identity.Dapper.PostgreSQL.Exceptions
{
    [Serializable]
    public class NoDapperIdentityConnectionStringConfiguredException
        : Exception
    {
        public NoDapperIdentityConnectionStringConfiguredException()
            : base()
        {
        }

        public NoDapperIdentityConnectionStringConfiguredException(string message)
            : base(message)
        {
        }

        public NoDapperIdentityConnectionStringConfiguredException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected NoDapperIdentityConnectionStringConfiguredException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }
    }
}

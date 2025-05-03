namespace Auctions.Domain;

public class RowVersionException(string message, Exception innerException) 
    : Exception(message, innerException);
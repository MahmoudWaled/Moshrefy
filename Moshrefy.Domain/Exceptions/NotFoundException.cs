namespace Moshrefy.Domain.Exceptions
{
    public sealed class NotFoundException<TPropertyValue>(string entityName, string propertyName, TPropertyValue propertyValue) : Exception($"{entityName} with {propertyName} {propertyValue} not found.")
    {
    }
}

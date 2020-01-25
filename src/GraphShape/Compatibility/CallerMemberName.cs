#if !SUPPORTS_CALLER_NAME
namespace System.Runtime.CompilerServices
{
    [AttributeUsage(AttributeTargets.Parameter)]
    internal sealed class CallerMemberNameAttribute : Attribute
    {
    }
}
#endif
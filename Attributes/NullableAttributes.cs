// 静态分析用

namespace System.Diagnostics.CodeAnalysis;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = true, Inherited = false)]
internal sealed class MemberNotNullWhenAttribute: Attribute {
    public MemberNotNullWhenAttribute(bool returnValue, params string[] members) { }
}

namespace System;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Interface | AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Event | AttributeTargets.Delegate, Inherited = false)]
public sealed class ObsoleteAttribute : Attribute
{
	public string? Message { get; }
	public bool IsError { get; }
	public string? DiagnosticId { set; get; }
	public string? UrlFormat { set; get; }

	public ObsoleteAttribute() {}
	public ObsoleteAttribute(string? message) => this.Message = message;
	public ObsoleteAttribute(string? message, bool error) : this(message) => this.IsError = error;
}

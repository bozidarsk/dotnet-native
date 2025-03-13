global RhpFallbackFailFast

section .text

; void RhpFallbackFailFast(string? message, Exception? exception)
RhpFallbackFailFast:
	int3
	cli
	hlt

; global RhpInitialInterfaceDispatch
; global RhpInitialDynamicInterfaceDispatch
global RhpByRefAssignRef

; extern RhpCidResolve

section .text

; RhpInitialInterfaceDispatch:
; RhpInitialDynamicInterfaceDispatch:
; 	cmp byte [rdi], 0
; 	jmp RhpInterfaceDispatchSlow

; RhpInterfaceDispatchSlow:
; 	mov rsi, r10
; 	jmp RhpCidResolve

RhpByRefAssignRef:
	mov rax, [rsi]
	mov [rdi], rax
	add rdi, 8
	add rsi, 8
	ret

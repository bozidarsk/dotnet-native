global RhpInitialInterfaceDispatch
global RhpInitialDynamicInterfaceDispatch
global RhpByRefAssignRef

extern RhpCidResolve

section .text

RhpInitialInterfaceDispatch:
RhpInitialDynamicInterfaceDispatch:
	cmp byte [rdi], 0
	jmp RhpInterfaceDispatchSlow

RhpInterfaceDispatchSlow:
	mov rdi, [rsp + (0 + 8*16 + 8)] ; __PWTB_TransitionBlock
	mov rsi, r11
	jmp RhpCidResolve

RhpByRefAssignRef:
	mov rax, [rsi]
	mov [rdi], rax
	add rdi, 8
	add rsi, 8
	ret

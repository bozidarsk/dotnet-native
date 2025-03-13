global memset
global memcpy
global memmove

; nint memset(nint destination, int x, nuint count)
memset:
	mov rax, rdi
	test rdx, rdx
	jz .ret
	mov ebx, esi
	.loop:
	mov byte [rdi], bl
	test rdx, rdx
	inc rdi
	dec rdx
	jnz .loop
	.ret:
	ret

; nint memcpy(nint destination, nint source, nuint count)
memcpy:
	mov rax, rdi
	test rdx, rdx
	jz .ret
	.loop:
	mov bl, byte [rsi]
	mov byte [rdi], bl
	test rdx, rdx
	inc rdi
	inc rsi
	dec rdx
	jnz .loop
	.ret:
	ret

; nint memmove(nint destination, nint source, nuint count)
memmove:
	mov rax, rdi
	test rdx, rdx
	jz .ret

	push rbp
	mov rbp, rsp
	sub rsp, 24

	mov qword [rbp - 8], rdi
	mov qword [rbp - 16], rsi
	mov qword [rbp - 24], rdx

	sub rsp, rdx
	mov rdi, rsp
	call memcpy
	mov rdi, qword [rbp - 8]
	mov rsi, rax
	mov rdx, qword [rbp - 24]
	call memcpy
	add rsp, qword [rbp - 24]

	.ret:
	add rsp, 24
	pop rbp
	ret

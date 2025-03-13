global pow
global frexp
global exp
global log
global $abs
global floor
global ceil
global sqrt
global cbrt
global log2
global log10
global sin
global cos
global tan
global asin
global acos
global atan
global atan2
global sinh
global cosh
global tanh
global asinh
global acosh
global atanh
global fma
global modf

section .rodata

align 16
zero: dq 0x0000000000000000, 0
one: dq 0x3ff0000000000000, 0
two: dq 0x4000000000000000, 0
negone: dq 0xbff0000000000000, 0
half: dq 0x3fe0000000000000, 0
pi: dq 0x400921fb54442d18, 0
tau: dq 0x401921fb54442d18, 0
nan: dq 0x7ff8000000000000, 0
nnan: dq 0xfff8000000000000, 0
e: dq 0x4005bf0a8b145769, 0

section .text

retnan:
	movsd xmm0, [nan]
	ret

; bool isnan(double x)
isnan:
	xor eax, eax
	comisd xmm0, xmm0
	je .ret
	or eax, 1
	.ret:
	ret

; double normtau(double x)
normtau:
	push rbp
	mov rbp, rsp
	sub rsp, 2 * 8

	divsd xmm0, [tau]
	lea rdi, [rbp - 8]
	call modf
	mulsd xmm0, [tau]

	add rsp, 2 * 8
	pop rbp
	ret

; double intpow(double x, long a)
intpow:
	movsd xmm1, xmm0
	movsd xmm0, [one]
	cmp rdi, 0
	jnz .noreturn
	ret
	.noreturn:

	mov rax, rdi
	shr rax, 63
	jz .nonegate
	not rdi
	inc rdi
	.nonegate:

	.loop:
	mulsd xmm0, xmm1
	dec rdi
	cmp rdi, 0
	jnz .loop

	cmp rax, 0
	jz .noreciprocal
	movsd xmm1, [one]
	divsd xmm1, xmm0
	movsd xmm0, xmm1
	.noreciprocal:

	ret

; double pow(double x, double y)
pow:
	push rbp
	mov rbp, rsp
	sub rsp, 4 * 8

	comisd xmm0, [one]
	je .ret
	comisd xmm0, [zero]
	je .ret

	movsd [rbp - 8], xmm0
	movsd [rbp - 16], xmm1
	mov qword [rbp - 32], 0

	movsd xmm0, [rbp - 8]
	call isnan
	test eax, eax
	jnz .retnan
	movsd xmm0, [rbp - 16]
	call isnan
	test eax, eax
	jnz .retnan

	movsd xmm0, [rbp - 8]
	movq rax, xmm0
	shr rax, 63
	jz .noinvalidate
	lea rdi, [rbp - 32]
	movsd xmm0, [rbp - 16]
	call modf
	movq rax, xmm0
	shl rax, 1
	jz .noretnnan
	movsd xmm0, [nnan]
	jmp .ret
	.noretnnan:
	cvtsd2si rax, [rbp - 16]
	and rax, 1
	shl rax, 63
	mov qword [rbp - 32], rax
	mov rax, qword [rbp - 8]
	shl rax, 1
	shr rax, 1
	mov qword [rbp - 8], rax
	.noinvalidate:

	mov rax, qword [rbp - 16]
	mov qword [rbp - 24], rax
	shr qword [rbp - 24], 63
	shl rax, 1
	shr rax, 1
	mov qword [rbp - 16], rax

	movsd xmm0, [rbp - 8]
	call log
	mulsd xmm0, [rbp - 16]
	call exp

	cmp qword [rbp - 24], 0
	jz .noreciprocal
	movsd xmm1, [one]
	divsd xmm1, xmm0
	movsd xmm0, xmm1
	.noreciprocal:

	movq rax, xmm0
	shl rax, 1
	shr rax, 1
	or rax, qword [rbp - 32]
	movq xmm0, rax

	.ret:
	add rsp, 4 * 8
	pop rbp
	ret

	.retnan:
	movsd xmm0, [nan]
	jmp .ret

; double frexp(double x, out int expptr)
frexp:
	comisd xmm0, [zero]
	jne .noret
	mov qword [rdi], 0
	ret
	.noret:

	movq rax, xmm0
	shr rax, 52
	and rax, (1 << 11) - 1
	sub eax, 1023
	inc eax
	mov dword [rdi], eax

	movsxd rdi, eax
	movq rax, xmm0
	push rax
	movsd xmm0, [two]
	call intpow
	pop rax
	movq xmm1, rax
	divsd xmm1, xmm0
	movsd xmm0, xmm1

	ret

; double exp(double x)
exp:
	comisd xmm0, xmm0
	jp retnan

	push rbp
	mov rbp, rsp
	sub rsp, 2 * 8

	lea rdi, [rbp - 8]
	call modf
	movsd [rbp - 16], xmm0
	cvtsd2si rdi, [rbp - 8]
	movsd xmm0, [e]
	call intpow

	mov rsi, 1
	mov rdi, 7
	movsd xmm1, [one]
	movsd xmm10, [one]
	movsd xmm11, [one]
	.loop:
	mulsd xmm10, [rbp - 16]
	cvtsi2sd xmm7, rsi
	mulsd xmm11, xmm7
	movsd xmm9, xmm10
	divsd xmm9, xmm11
	addsd xmm1, xmm9
	inc rsi
	cmp rsi, rdi
	jbe .loop

	mulsd xmm0, xmm1

	add rsp, 2 * 8
	pop rbp
	ret

; double log(double x)
log:
	comisd xmm0, xmm0
	jp retnan

	push rbp
	mov rbp, rsp
	sub rsp, 2 * 8

	lea rdi, [rbp - 4]
	call frexp
	mov eax, dword [rbp - 4]
	movsxd rbx, eax
	cvtsi2sd xmm1, rbx
	movsd [rbp - 8], xmm1

	mov rsi, 1
	mov rdi, 200
	subsd xmm0, [one]
	movsd xmm1, xmm0
	movsd xmm10, [one]
	movsd xmm11, xmm1
	movsd xmm12, [one]
	.loop:
	mulsd xmm10, [negone]
	mulsd xmm11, xmm1
	addsd xmm12, [one]
	movsd xmm7, xmm10
	mulsd xmm7, xmm11
	divsd xmm7, xmm12
	addsd xmm0, xmm7
	inc rsi
	cmp rsi, rdi
	jbe .loop

	mov rax, 0x3ff71547652b82fe ; 1.4426950408889634 == 1 / ln2
	movq xmm1, rax
	mulsd xmm0, xmm1
	addsd xmm0, [rbp - 8]
	mov rax, 0x3fe62e42fefa39ef ; 0.6931471805599453 == 1 / log2(e)
	movq xmm1, rax
	mulsd xmm0, xmm1

	add rsp, 2 * 8
	pop rbp
	ret

; double modf(double x, out double interger)
modf:
	cvtsd2si rax, xmm0
	cvtsi2sd xmm1, rax
	subsd xmm0, xmm1
	movsd [rdi], xmm1
	ret

; double fma(double x, double y, double z)
fma:
	vfmadd132sd xmm0, xmm2, xmm1
	ret

; double $abs(double x)
$abs:
	movq rax, xmm0
	mov rbx, 0x7fffffffffffffff
	and rax, rbx
	movq xmm0, rax
	ret

; double floor(double x)
floor:
	movsd xmm1, xmm0
	cvttsd2si rax, xmm0
	cvtsi2sd xmm0, rax
	comisd xmm1, qword [zero]
	jae .ret
	subsd xmm0, qword [one]
	.ret:
	ret

; double ceil(double x)
ceil:
	movsd xmm1, xmm0
	call floor
	comisd xmm0, xmm1
	je .ret
	addsd xmm0, qword [one]
	comisd xmm0, qword [zero]
	jne .ret
	mov rax, 0x8000000000000000
	movq xmm0, rax
	.ret:
	ret

; double sqrt(double x)
sqrt:
	sqrtsd xmm0, xmm0
	ret

; double cbrt(double x)
cbrt:
	call log
	mov rax, 0x4008000000000000
	movq xmm1, rax
	divsd xmm0, xmm1
	call exp
	ret

; double log2(double x)
log2:
	call log
	mov rax, 0x3fe62e42fefa39ef
	movq xmm1, rax
	divsd xmm0, xmm1
	ret

; double log10(double x)
log10:
	call log
	mov rax, 0x40026bb1bbb55516
	movq xmm1, rax
	divsd xmm0, xmm1
	ret

; double sin(double x)
sin:
	comisd xmm0, xmm0
	jp retnan

	call normtau

	mov rdx, 3

	; rdi == max iterrations
	mov rdi, 20

	; rsi == i
	mov rsi, 1

	; xmm0 == x
	; xmm1 == sum
	movsd xmm1, xmm0

	; xmm2 == up
	movsd xmm2, xmm0
	mulsd xmm2, xmm0
	mulsd xmm2, xmm0

	; xmm3 == down
	mov rax, 6
	cvtsi2sd xmm3, rax

	; xmm4 == sign
	movsd xmm4, qword [negone]

	.loop:
	movsd xmm7, xmm4
	mulsd xmm7, xmm2
	divsd xmm7, xmm3
	addsd xmm1, xmm7

	mulsd xmm4, qword [negone]

	mulsd xmm2, xmm0
	mulsd xmm2, xmm0

	inc rdx
	cvtsi2sd xmm7, rdx
	mulsd xmm3, xmm7
	inc rdx
	cvtsi2sd xmm7, rdx
	mulsd xmm3, xmm7

	inc rsi
	cmp rsi, rdi
	jbe .loop

	movsd xmm0, xmm1
	ret

; double cos(double x)
cos:
	comisd xmm0, xmm0
	jp retnan

	call normtau

	mov rdx, 2

	; rdi == max iterrations
	mov rdi, 20

	; rsi == i
	mov rsi, 1

	; xmm0 == x
	; xmm1 == sum
	movsd xmm1, qword [one]

	; xmm2 == up
	movsd xmm2, xmm0
	mulsd xmm2, xmm0

	; xmm3 == down
	mov rax, 2
	cvtsi2sd xmm3, rax

	; xmm4 == sign
	movsd xmm4, qword [negone]

	.loop:
	movsd xmm7, xmm4
	mulsd xmm7, xmm2
	divsd xmm7, xmm3
	addsd xmm1, xmm7

	mulsd xmm4, qword [negone]

	mulsd xmm2, xmm0
	mulsd xmm2, xmm0

	inc rdx
	cvtsi2sd xmm7, rdx
	mulsd xmm3, xmm7
	inc rdx
	cvtsi2sd xmm7, rdx
	mulsd xmm3, xmm7

	inc rsi
	cmp rsi, rdi
	jbe .loop

	movsd xmm0, xmm1
	ret

; double tan(double x)
tan:
	push rbp
	mov rbp, rsp
	sub rsp, 16

	movsd qword [rbp - 8], xmm0
	call cos
	movsd qword [rbp - 16], xmm0
	movsd xmm0, qword [rbp - 8]
	call sin
	divsd xmm0, qword [rbp - 16]

	add rsp, 16
	pop rbp
	ret

; double asin(double x)
asin:
	comisd xmm0, qword [one]
	jp retnan
	ja retnan
	comisd xmm0, qword [negone]
	jb retnan

	mov rdx, 2

	; rdi == max iterrations
	mov rdi, 20

	; rsi == i
	mov rsi, 1

	; xmm0 == x
	; xmm1 == sum
	movsd xmm1, xmm0

	; xmm2 == xterm
	movsd xmm2, xmm0
	mulsd xmm2, xmm0
	mulsd xmm2, xmm0

	; xmm3 == 2factterm
	mov rax, 2
	cvtsi2sd xmm3, rax

	; xmm4 == 4term
	mov rax, 4
	cvtsi2sd xmm4, rax

	; xmm5 == factterm
	mov rax, 1
	cvtsi2sd xmm5, rax

	; xmm6 == constterm
	mov rax, 3
	cvtsi2sd xmm6, rax

	.loop:
	movsd xmm7, xmm2
	mulsd xmm7, xmm3
	divsd xmm7, xmm4
	divsd xmm7, xmm5
	divsd xmm7, xmm5
	divsd xmm7, xmm6
	addsd xmm1, xmm7

	mulsd xmm2, xmm0
	mulsd xmm2, xmm0

	mov rax, 4
	cvtsi2sd xmm7, rax
	mulsd xmm4, xmm7 

	mov rax, 2
	cvtsi2sd xmm7, rax
	addsd xmm6, xmm7

	inc rdx
	cvtsi2sd xmm7, rdx
	mulsd xmm3, xmm7
	inc rdx
	cvtsi2sd xmm7, rdx
	mulsd xmm3, xmm7

	inc rsi
	cvtsi2sd xmm7, rsi
	mulsd xmm5, xmm7

	cmp rsi, rdi
	jbe .loop

	movsd xmm0, xmm1
	ret

; double acos(double x)
acos:
	call asin
	movsd xmm1, qword [pi]
	mulsd xmm1, qword [half]
	subsd xmm1, xmm0
	movsd xmm0, xmm1
	ret

; double atan(double x)
atan:
	comisd xmm0, xmm0
	jp retnan

	comisd xmm0, qword [one]
	jae .above
	comisd xmm0, qword [negone]
	jbe .below

	jmp .series

	.above:
	movsd xmm1, xmm0
	movsd xmm0, qword [one]
	divsd xmm0, xmm1
	call .series
	movsd xmm1, qword [pi]
	mulsd xmm1, qword [half]
	subsd xmm1, xmm0
	movsd xmm0, xmm1
	ret

	.below:
	movsd xmm1, xmm0
	movsd xmm0, qword [one]
	divsd xmm0, xmm1
	call .series
	movsd xmm1, qword [pi]
	mulsd xmm1, qword [half]
	addsd xmm0, xmm1
	mulsd xmm0, qword [negone]
	ret

	.series:
	; rdi == max iterrations
	mov rdi, 70

	; rsi == i
	mov rsi, 1

	; xmm0 == x
	; xmm1 == sum
	movsd xmm1, xmm0

	; xmm2 == up
	movsd xmm2, xmm0
	mulsd xmm2, xmm0
	mulsd xmm2, xmm0

	; xmm3 == down
	mov rax, 3
	cvtsi2sd xmm3, rax

	; xmm4 == sign
	movsd xmm4, qword [negone]

	.loop:
	movsd xmm7, xmm4
	mulsd xmm7, xmm2
	divsd xmm7, xmm3
	addsd xmm1, xmm7

	mulsd xmm4, qword [negone]

	mulsd xmm2, xmm0
	mulsd xmm2, xmm0

	mov rax, 2
	cvtsi2sd xmm7, rax
	addsd xmm3, xmm7

	inc rsi
	cmp rsi, rdi
	jbe .loop

	movsd xmm0, xmm1
	ret

; double atan2(double y, double x)
atan2:
	comisd xmm1, qword [zero]
	jbe .continue0
	divsd xmm0, xmm1
	jmp atan
	.continue0:

	comisd xmm1, qword [zero]
	jae .continue1
	comisd xmm0, qword [zero]
	jb .continue1
	divsd xmm0, xmm1
	call atan
	addsd xmm0, qword [pi]
	ret
	.continue1:

	comisd xmm1, qword [zero]
	jae .continue2
	comisd xmm0, qword [zero]
	jae .continue2
	divsd xmm0, xmm1
	call atan
	subsd xmm0, qword [pi]
	ret
	.continue2:

	comisd xmm1, qword [zero]
	jne .continue3
	comisd xmm0, qword [zero]
	jbe .continue3
	movsd xmm0, qword [pi]
	mulsd xmm0, qword [half]
	ret
	.continue3:

	comisd xmm1, qword [zero]
	jne .continue4
	comisd xmm0, qword [zero]
	jae .continue4
	movsd xmm0, qword [pi]
	mulsd xmm0, qword [half]
	mulsd xmm0, qword [negone]
	ret
	.continue4:

	xorpd xmm0, xmm0
	ret

; double sinh(double x)
sinh:
	push rbp
	mov rbp, rsp
	sub rsp, 16

	movsd qword [rbp - 8], xmm0
	mulsd xmm0, qword [negone]
	call exp
	movsd qword [rbp - 16], xmm0
	movsd xmm0, qword [rbp - 8]
	call exp
	subsd xmm0, qword [rbp - 16]
	mulsd xmm0, qword [half]

	add rsp, 16
	pop rbp
	ret

; double cosh(double x)
cosh:
	push rbp
	mov rbp, rsp
	sub rsp, 16

	movsd qword [rbp - 8], xmm0
	mulsd xmm0, qword [negone]
	call exp
	movsd qword [rbp - 16], xmm0
	movsd xmm0, qword [rbp - 8]
	call exp
	addsd xmm0, qword [rbp - 16]
	mulsd xmm0, qword [half]

	add rsp, 16
	pop rbp
	ret

; double tanh(double x)
tanh:
	push rbp
	mov rbp, rsp
	sub rsp, 16

	movsd qword [rbp - 8], xmm0
	call cosh
	movsd qword [rbp - 16], xmm0
	movsd xmm0, qword [rbp - 8]
	call sinh
	divsd xmm0, qword [rbp - 16]

	add rsp, 16
	pop rbp
	ret

; double asinh(double x)
asinh:
	push rbp
	mov rbp, rsp
	sub rsp, 8

	movsd qword [rbp - 8], xmm0

	mulsd xmm0, xmm0
	addsd xmm0, qword [one]
	call sqrt
	addsd xmm0, qword [rbp - 8]
	call log

	add rsp, 8
	pop rbp
	ret

; double acosh(double x)
acosh:
	push rbp
	mov rbp, rsp
	sub rsp, 16

	movsd qword [rbp - 8], xmm0

	mulsd xmm0, xmm0
	subsd xmm0, qword [one]
	call sqrt
	addsd xmm0, qword [rbp - 8]
	call log

	add rsp, 16
	pop rbp
	ret

; double atanh(double x)
atanh:
	movsd xmm1, qword [one]
	subsd xmm1, xmm0
	addsd xmm0, qword [one]
	divsd xmm0, xmm1
	call log
	mulsd xmm0, qword [half]
	ret

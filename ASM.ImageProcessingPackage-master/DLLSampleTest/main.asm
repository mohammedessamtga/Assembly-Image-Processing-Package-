include irvine32.inc
.data
;no static data
sump dword 256 dup(0)
temp dword 0
tempeax dword 0
tempecx dword 0
Sind dword 0  
Sindx dword 0
Stemp dword 0
Sval dword 0 
acc dword 0 
Schoice dword 0 
temp_for_Mult sdword 0 
snheight dword 4
sx sdword 1,0,-1,2,0,-2,1,0,-1
sy sdword 1,2,1,0,0,0,-1,-2,-1
img dword 9 dup(0)
.code
;-----------------------------------------------------
;Sum PROC Calculates 2 unsigned integers
;Recieves: 2 DWord parametes number 1 and number 2
;Return: the sum of the 2 unsigned numbers into the EAX
;------------------------------------------------------
Sum PROC int1:DWORD, int2:DWORD
	mov eax, int1
	add eax, int2
	ret
Sum ENDP

;-----------------------------------------------------
;SumArr PROC Calculates Sum of an array
;Recieves: Offset and the size of an array
;Return: the sum of the array into the EAX
;------------------------------------------------------
SumArr PROC arr:PTR DWORD, sz:DWORD
	push esi
	push ecx

	mov esi, arr
	mov ecx, sz
	mov eax, 0
	sum_loop:
		add eax, DWORD PTR [esi]
		add esi, 4
	loop sum_loop
	
	pop ecx
	pop esi
	Ret
SumArr ENDP

;----------------------------------------------------------------
;Sum PROC convert an array of bytes from lower case to upper case
;Recieves: offset of byte array and it's size
;---------------------------------------------------------------
ToUpper PROC str1:PTR BYTE, sz:DWORD
	push esi
	push ecx
	
	mov esi, str1
	mov ecx, sz
	l1:
		;input validations (Limitation the char to be between a and z)
		cmp byte ptr [esi], 'a'
		jb skip
		cmp byte ptr [esi], 'z'
		ja skip

		and byte ptr [esi], 11011111b
		skip:
		inc esi
	loop l1
	
	pop ecx
	pop esi
	ret
ToUpper ENDP


;#######################################################
;#					Project Procedures					#
;#######################################################



Invert proc redChannel:PTR DWORD, greenChannel:PTR DWORD, blueChannel:PTR DWORD, imageSize: DWORD
	PUSHAD


	MOV ECX, IMAGESIZE
	MOV ESI, REDCHANNEL
	L1:
		MOV EBX, 255
		MOV EAX, [ESI]
		SUB EBX, EAX
		MOV EAX, EBX
		CMP EAX, 0
		JL NEGATIVEVAL1
		
		JMP SKIP1

		NEGATIVEVAL1:
		MOV EAX, 0


		SKIP1:
		MOV [ESI], EAX
		ADD ESI, 4
		
	LOOP L1

		MOV ECX, IMAGESIZE
	MOV ESI, GREENCHANNEL

	L2:
		MOV EBX, 255
		MOV EAX, [ESI]
		SUB EBX, EAX
		MOV EAX, EBX
		CMP EAX, 0
		JL NEGATIVEVAL2
		
		JMP SKIP2

		NEGATIVEVAL2:
		MOV EAX, 0


		SKIP2:
		MOV [ESI], EAX
		ADD ESI, 4
		
	LOOP L2

		MOV ECX, IMAGESIZE
	MOV ESI, BLUECHANNEL

	L3:
		MOV EBX, 255
		MOV EAX, [ESI]
		SUB EBX, EAX
		MOV EAX, EBX
		CMP EAX, 0
		JL NEGATIVEVAL3
		
		JMP SKIP3

		NEGATIVEVAL3:
		MOV EAX, 0


		SKIP3:
		MOV [ESI], EAX
		ADD ESI, 4
		
	LOOP L3

	POPAD
	RET
Invert endp

EqualizeHistogram proc Channel:PTR DWORD, imageSize: DWORD
pushad
mov ecx , imageSize 
mov esi ,  Channel
mov edi , offset sump 
L:
mov eax , [esi]
mov edx , 4 
mul edx 
mov ebx, [edi+eax]
inc ebx 
mov [edi+eax],ebx
add esi,4
loop L 

mov edi , offset sump
add edi ,4 
mov ecx , 255 
L1:
mov eax , [edi-4]
mov ebx , [edi]
add ebx,eax 
mov [edi],ebx
add edi,4
Loop L1 

mov ecx , 256
mov edi , offset sump 
L2:
mov eax , [edi]
mov ebx , 255 
mul ebx 
mov edx , 0 
mov ebx , imageSize 
div ebx 
cmp eax , 255 
jna LL 
mov eax , 255 
LL:
mov [edi],eax
add edi,4
loop L2


mov ecx,imageSize 
mov edi , offset sump 
mov esi , channel 
L3: 
mov eax , [esi]
mov edx , 4 
mul edx 
mov ebx , [edi+eax]
mov [esi],ebx
add esi , 4 
loop L3 

popad
ret
EqualizeHistogram ENDP

ImageToGray proc redChannel:PTR DWORD, greenChannel:PTR DWORD, blueChannel:PTR DWORD, imageSize: DWORD
pushad
mov edi , redchannel 
mov esi , greenchannel 
mov ebx , bluechannel 
mov ecx , imagesize 
Lg:
mov eax , [edi]
add eax , [esi]
add eax , [ebx]
mov edx , 0
mov temp , edi 
mov edi , 3 
div edi 
mov edi , temp 
mov [edi],eax
mov [esi],eax
mov [ebx],eax
add edi , 4
add esi , 4
add ebx , 4 
loop Lg
popad
ret
ImageToGray endp

AddImages proc Channel1:PTR DWORD, Channel2:PTR DWORD, imageSize: DWORD
pushad
mov esi , Channel1 
mov edi , Channel2 
mov ecx , imageSize 
LA:
mov eax , [esi]
mov ebx , [edi]
add eax , ebx 
cmp eax , 255 
jna LA1
mov eax , 255 
LA1:
mov [esi],eax 
add esi,4
add edi ,4
Loop LA 
popad
ret
AddImages endp

SubImages proc Channel1:PTR DWORD, Channel2:PTR DWORD, imageSize: DWORD
pushad
mov esi , Channel1 
mov edi , Channel2 
mov ecx , imageSize 
LS:
mov eax , [esi]
mov ebx , [edi] 
cmp ebx , eax
jna LS1
mov eax , 0
jmp LS2
LS1:
sub eax , ebx 
LS2:
mov [esi],eax 
add esi,4
add edi ,4
Loop LS 

popad
ret
SubImages endp

MatrixM PROC  

pushad 
mov ecx , Schoice 
cmp ecx , 1 
jne assign 
 mov esi , offset sx
 jmp Lassign
 assign: 
 mov esi , offset sy 
 Lassign:
 mov edi , offset img 
mov eax, 0 
mov acc,eax
mov ecx, 9
mov ebx, 0
; multiplication loop 3*3M1 * 3*3M2
L1:
	mov eax , [esi]
	mov temp_for_Mult, ebx
	mov ebx , [edi]
	imul ebx
	mov ebx , temp_for_Mult
	add ebx , eax 
	add edi, 4
	add esi, 4
Loop L1
cmp ebx , 0 
jge LM 
Neg ebx 
LM:
cmp ebx , 255 
jng LM3 
mov ebx , 255
LM3: 
mov acc , ebx 


popad

ret
MatrixM ENDP

init Proc 
mov edi , offset img 
sub eax , Snheight 
sub eax , 4 
mov edx, [ebx+eax]
mov [edi],edx
add edi , 4 

add eax , 4 
mov edx, [ebx+eax]
mov [edi],edx
add edi , 4 

add eax , 4 
mov edx, [ebx+eax]
mov [edi],edx
add edi , 4 

sub eax , 4  
add eax , snheight
sub eax , 4 
mov edx , [ebx+eax]
mov [edi],edx
add edi , 4 

add eax , 4 
mov edx , [ebx+eax]
mov [edi],edx 
add edi , 4 

add eax , 4 
mov edx , [ebx+eax]
mov [edi],edx
add edi , 4 

sub eax , 4 
add eax , snheight 
sub eax , 4 
mov edx , [ebx+eax]
mov [edi],edx
add edi , 4 

add eax , 4 
mov edx , [ebx+eax]
mov [edi],edx 
add edi , 4 

add eax , 4 
mov edx , [ebx+eax]
mov [edi],edx 
add edi , 4 
sub eax , 4 
sub eax , snheight 
ret 
init endp
Sobel PROC Channel1:PTR DWORD, Swidth: DWORD , height: DWORD , choice: Dword
pushad
mov ecx , choice 
mov schoice , ecx 
mov ecx , Swidth 
sub ecx , 2
 mov ebx , Channel1
 mov eax , 0 
 mov Sind , eax 
 mov eax , height 
 mov edi , snheight 
 mul edi 
 mov snheight , eax 
Ls: 
 inc Sind 
 mov edi , Sind 
 mov eax , height 
 mul edi 
 mov edi , 4 
 mul edi 
 mov Stemp , ecx 
 mov ecx , height 
 sub ecx , 2 
 Ls1: 
 add eax, 4 
 call init
 mov tempeax , eax
 mov tempecx ,ecx
 call MatrixM
 mov ecx , tempecx
 mov eax , tempeax 
 mov edx , acc 
 mov [ebx+eax],edx 
 loop Ls1 
 mov ecx , Stemp 
 Loop Ls 
popad 
ret 
Sobel ENDP 

; DllMain is required for any DLL
DllMain PROC hInstance:DWORD, fdwReason:DWORD, lpReserved:DWORD
mov eax, 1 ; Return true to caller.
ret
DllMain ENDP
END DllMain
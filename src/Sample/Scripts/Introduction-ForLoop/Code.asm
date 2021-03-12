
.entry:
    push    rbp
    mov     rbp, rsp
    xor     eax, eax
    xor     esi, esi
    
    test    edi, edi            ; count == 0?
    jle     .end
    
.inner:    
    add     eax, esi            ; sum = sum + x    
    
    inc     esi                 ; x++
    
    cmp     esi, edi            ; x < count
    jl      .inner
    
.end:
    pop     rbp
    ret     

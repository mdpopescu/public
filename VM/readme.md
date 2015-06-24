Write a virtual machine

Specifications:

  Word size: 16-bit
  Number of registers: 8 (16-bit registers) plus SP (stack pointer) and PC (program counter)
    Register 0 is the accumulator
    The stack grows downwards - PUSH decrements SP, POP increments it; SP starts at 0
  64KB of memory shared between program and data

Instructions (variable size):

  xxx                 any value 0 to 7
  reg                 register number, 0 to 7 (r0 to r7 in assembly)
  value               16-bit value in little-endian format
  addr                16-bit address in little-endian format


  Assembly            Machine code    Description
  ========            ============    ===========

  NOP                 00000xxx        Does nothing
  CLR reg             00001reg        Sets the register to zero
  INC reg             00010reg        Increments the value in the register
  DEC reg             00011reg        Decrements the value in the register
  NEG reg             00100reg        Negates the value in the register (1-complement)
  SET reg, value      00101reg value  Sets the register to the given value
  LOAD reg, addr      00110reg addr   Loads the register with the 16-bit value stored in RAM at the given address
  SAVE reg, addr      00111reg addr   Saves the 16-bit value in the register at the address
  ADD reg             01000reg        Adds the register to the accumulator
  SUB reg             01001reg        Subtracts the register from the accumulator
  AND reg             01010reg        Sets acc = acc AND reg (bitwise operation)
  OR reg              01011reg        Sets acc = acc OR reg (bitwise operation)
  SHR reg             01100reg        Shifts the register to the right one bit, losing the rightmost bit and setting the leftmost bit to 0
  SHL reg             01101reg        Shifts the register to the left one bit, losing the leftmost bit and setting the rightmost bit to 0
  ROTR reg            01110reg        Rotates the register to the right one bit, setting the leftmost bit to the value of the previous rightmost bit
  ROTL reg            01111reg        Rotates the register to the left one bit, setting the rightmost bit to the value of the previous leftmost bit

  JUMP addr           10000000 addr   Jumps to the given address (sets PC to addr)
  JZ addr             10000001 addr   Jumps to the given address if the accumulator is zero
  JNZ addr            10000010 addr   Jumps to the given address if the accumulator is not zero
  
                      10000011        Undefined; equivalent to NOP
  
  CALL addr           10000100 addr   Pushes the current value of PC to the stack and then jumps to the given address (sets PC to addr)
  RET                 10000101        Pops the value from the stack and sets PC to that value

  GETS addr           10000110 addr   Reads a line from the console and stores the string at the given address, zero-terminated
  PUTS addr           10000111 addr   Writes a line to the console starting at the given address until the first zero byte

  PUSH reg            10001reg        Pushes the value in the register to the stack (mem[--SP] = reg)
  POP reg             10010reg        Pops the value from the stack and sets the register to that value (reg = mem[SP++])

                      10011000
                      ........        Undefined; equivalent to NOP
                      11111110

  HALT                11111111        Halts the program

Assembly directives:

  xx                  any two-byte value in hex

  DB xx[, xx]...      saves the given bytes at the current address
  DW xxxx[, xxxx]...  saves the given words at the current address
  DS "string"         saves the given string, zero-terminated, at the current address

def multiply(a, b):
    result = 0
    while a >= 1:
        if a % 2 == 1:
            result += b

        a //= 2
        b *= 2

    return result

a = int(raw_input("Enter A:"))
b = int(raw_input("Enter B:"))
print "a * b = " + str(multiply(a, b))

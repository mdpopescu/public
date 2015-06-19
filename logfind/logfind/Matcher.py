from logfind.Logical import Logical

class Matcher:

    def __init__(self):
        """ initialization """

    def match(self, contents, words, operation):
        contains = [word for word in words if word in contents]

        if operation == Logical.And:
            return len(contains) == len(words)
        else:
            return len(contains) > 0
